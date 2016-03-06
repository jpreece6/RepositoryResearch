using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.RepositoryCore;
using NHibernate.Criterion;

namespace DataEngine
{
    public class SyncRecordRepository<T> : Repository<T> where T : SyncRecord
    {
        public SyncRecordRepository(ISessionContext sessionContext) : base(sessionContext)
        {
            
        }

        public SyncRecord GetByLocalId(int id, string table)
        {
            var query = SessionContext.Session.QueryOver<SyncRecord>().Where(x => x.Local_Id == id)
                                                                      .And(x => x.Table_Id == table)
                                                                      .SingleOrDefault();
            SyncRecord record = query;

            return record;
        }

        public void DeleteAll()
        {
            SessionContext.Session.CreateSQLQuery("DELETE FROM SyncRecord").ExecuteUpdate();
        }
    }
}
