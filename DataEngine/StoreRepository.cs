using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.RepositoryCore;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace DataEngine
{
    /// <summary>
    /// Store repository controls all CRUD operations
    /// of the Store table and provides Store specific queries
    /// </summary>
    /// <typeparam name="T">Must be of type Store</typeparam>
    public class StoreRepository<T> : Repository<T> where T : Store
    {
        public StoreRepository(ISessionContext sessionContext) : base(sessionContext)
        {
        }

        /// <summary>
        /// Performs a LIKE query to return all
        /// Stores that (semi)match the criteria
        /// </summary>
        /// <param name="name">Store name to search for</param>
        /// <returns>IList of Stores that match the criteria</returns>
        public IList<Store> GetWithName(string name)
        {
            var query = SessionContext.Session.QueryOver<Store>()
                                              .Where(Restrictions
                                              .On<Store>(x => x.StoreName)
                                              .IsLike(name)).List();
            IList<Store> stores = query;

            return stores;
        }
    }
}
