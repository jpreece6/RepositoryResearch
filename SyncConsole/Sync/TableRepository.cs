using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Entities;
using DataEngine.RepositoryCore;

namespace SyncConsole.Sync
{
    public class TableRepository<T> : Repository<T> where T : class
    {
        protected IList<T> TableRepositories;

        public TableRepository()
        {
            TableRepositories = new List<T>();
        }

        public override int Count()
        {
            return TableRepositories.Count;
        }

        public override void Save(T tData)
        {
            TableRepositories.Add(tData);
        }

        public override void Remove(T tData)
        {
            TableRepositories.Remove(tData);
        }

        public override IList<T> GetAll()
        {
            return TableRepositories;
        }
    }
}
