using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.Helpers;
using DataEngine.RepositoryCore;
using NHibernate.Event;

namespace SyncConsole.Sync
{
    public class SyncManager : ISyncManager
    {
        public bool Active { get; set; }

        private TableRepository<IRepository<IEntity>> _remoteTableRepository;
        private TableRepository<IRepository<IEntity>> _localTableRepository;

        private SessionContext _remoteSessionContext;
        private SessionContext _localSessionContext;

        private readonly SessionFactoryManager _factoryManager;

        public SyncManager()
        {
            Active = false;
            _factoryManager = new SessionFactoryManager();

            CreateContexts();
            CreateRemoteRepositories();
            CreateLocalRepositories();
        }

        public void CreateContexts()
        {
            _remoteSessionContext = new RemoteContext(_factoryManager);
            _localSessionContext = new LocalContext(_factoryManager);
        }

        public void CreateRemoteRepositories()
        {
            _remoteTableRepository = new TableRepository<IRepository<IEntity>>();
            _remoteTableRepository.Save( new EmployeeRepository<IEntity>(_remoteSessionContext));
        }

        public void CreateLocalRepositories()
        {
            _localTableRepository = new TableRepository<IRepository<IEntity>>();
            _localTableRepository.Save( new EmployeeRepository<IEntity>(_localSessionContext));

        }

        public void SyncTable(IRepository<IEntity> localTable, IRepository<IEntity> remoteTable)
        {
            foreach (var row in localTable.GetAll())
            {
                // Get the id of the current object row 
                var rowId = (int) row.GetType().GetProperty("Id", typeof (int)).GetValue(row);
                if (remoteTable.Exists(rowId) == 0)
                {
                    var newObject = ObjectCopy.Copy(row);
                    remoteTable.Save(newObject);
                }
                else
                {
                    remoteTable.Save(row);
                }

                // Save changes to remote
                remoteTable.Commit();
                
                // Remove after successful remove
                localTable.Remove(row);
                localTable.Commit();
            }
        }

        public void SyncAllTables()
        {
            _localSessionContext.OpenContextSession();
            _remoteSessionContext.OpenContextSession();

            foreach (var localTable in _localTableRepository.GetAll()) // Get a local table
            {
                foreach (var remoteTable in _remoteTableRepository.GetAll()) // Get a remote table
                { 
                    SyncTable(localTable, remoteTable);
                }
            }
        }
    }
}
