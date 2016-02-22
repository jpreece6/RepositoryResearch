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
    // TODO TALK ABOUT GENERIC ISSUES WHERE YOU CANT HAVE DIFFERENT TYPES EVEN INTERFACES
    public class SyncManager : ISyncManager
    {
        public bool Active { get; set; }

        private SessionContext _remoteSessionContext;
        private SessionContext _localSessionContext;

        private readonly SessionFactoryManager _factoryManager;

        public SyncManager()
        {
            Active = false;
            _factoryManager = new SessionFactoryManager();

            CreateContexts();
        }

        public void CreateContexts()
        {
            _remoteSessionContext = new RemoteContext(_factoryManager);
            _localSessionContext = new LocalContext(_factoryManager);
        }

        public void SyncTable_Employee()
        {
            var localTable = new EmployeeRepository<Employee>(_localSessionContext);
            var remoteTable = new EmployeeRepository<Employee>(_remoteSessionContext);

            #region SyncCode
            foreach (var row in localTable.GetAll())
            {
                // Get the id of the current object row 
                var rowId = (int)row.GetType().GetProperty("Id", typeof(int)).GetValue(row);
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
            #endregion

        }

        public void SyncTable_Product()
        {
            var localTable = new ProductRepository<Product>(_localSessionContext);
            var remoteTable = new ProductRepository<Product>(_remoteSessionContext);

            #region SyncCode
            foreach (var row in localTable.GetAll())
            {
                // Get the id of the current object row 
                var rowId = (int)row.GetType().GetProperty("Id", typeof(int)).GetValue(row);
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
            #endregion

        }

        public void SyncAllTables()
        {
            _localSessionContext.OpenContextSession();
            _remoteSessionContext.OpenContextSession();

            SyncTable_Employee();
            SyncTable_Product();
        }
    }
}
