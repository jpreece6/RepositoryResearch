using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.RepositoryCore;
using NHibernate.Linq;

namespace DataEngine
{
    /// <summary>
    /// Sale repository controls the CRUD operations of
    /// the Sale table and provides Sale specific queries.
    /// </summary>
    /// <typeparam name="T">Entity Type, myst be of type Sale</typeparam>
    public class SaleRepository<T> : Repository<T> where T : Sale
    {
        public SaleRepository(ISessionContext sessionContext) : base(sessionContext)
        {
            AllowLocalEdits = true;
        }

        /// <summary>
        /// Perfoms a EQUALS query to return all Sales that match the 
        /// store ID given
        /// </summary>
        /// <param name="id">Store id to match</param>
        /// <returns>IList of Sales that match the criteria</returns>
        public IList<Sale> GetWithStoreId(int id)
        {
            var query = SessionContext.Session.QueryOver<Sale>().Where(x => x.StoreId == id).List();
            IList<Sale> sales = query;

            return sales;
        }

        /// <summary>
        /// Perfoms a EQUALS query to return all Sales that match the
        /// product id criteria
        /// </summary>
        /// <param name="id">Product id to match</param>
        /// <returns>IList of Sales that match the criteria</returns>
        public IList<Sale> GetWithProductId(int id)
        {
            var query = SessionContext.Session.QueryOver<Sale>().Where(x => x.ProductId == id).List();
            IList<Sale> sales = query;

            return sales;
        }
    }
}
