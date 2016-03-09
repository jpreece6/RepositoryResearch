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
    /// <summary>
    /// SyncRecord repository controls all CRUD operations
    /// for the SyncRecord table also provides SyncRecord specific queries.
    /// 
    /// The SyncRecord table is used during sync. When a record is synced to the
    /// remote server the new remote record ID is stored within this table and its previous
    /// local ID this is so we can update any foreign keys that are present before syncing those
    /// records to the server. Without this function local records that have an incorrect foreign key ID
    /// after sync will break the sync function and data integrity. 
    /// </summary>
    /// <typeparam name="T">Must be of type SyncRecord</typeparam>
    public class SyncRecordRepository<T> : Repository<T> where T : SyncRecord
    {
        public SyncRecordRepository(ISessionContext sessionContext) : base(sessionContext)
        {
            
        }

        /// <summary>
        /// Perfoms a EQUALS query to find all records that
        /// match the local ID and table name provided.
        /// </summary>
        /// <param name="id">Local ID criteria</param>
        /// <param name="table">Table name criteria</param>
        /// <returns>Returns a single SyncRecord if found otherwise null if none found...</returns>
        public SyncRecord GetByLocalId(int id, string table)
        {
            var query = SessionContext.Session.QueryOver<SyncRecord>().Where(x => x.Local_Id == id)
                                                                      .And(x => x.Table_Id == table)
                                                                      .SingleOrDefault();
            SyncRecord record = query;

            return record;
        }

        /// <summary>
        /// Removes all records from the SyncRecord table. We need to empty this
        /// table after every sync as we don't want conflicts occuring during future syncs.
        /// Furthermore all records should be sync successfully this means that we no longer need
        /// this data anyways.
        /// </summary>
        public void DeleteAll()
        {
            SessionContext.Session.CreateSQLQuery("DELETE FROM SyncRecord").ExecuteUpdate();
        }
    }
}
