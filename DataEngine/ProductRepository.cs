using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.RepositoryCore;
using NHibernate.Criterion;
using NHibernate.Event;
using NHibernate.Linq;

namespace DataEngine
{
    /// <summary>
    /// Product repository controls the CRUD operations for the
    /// Product table and provides specific query functions
    /// </summary>
    /// <typeparam name="T">Entity type, must be of Product type</typeparam>
    public class ProductRepository<T> : Repository<T> where T : Product
    {

        public ProductRepository(ISessionContext sessionContext) : base(sessionContext)
        {
        }

        /// <summary>
        /// Performs a LIKE query to return all Products that (semi)match the
        /// specified name
        /// </summary>
        /// <param name="name">String name to search for</param>
        /// <returns>IList of Products that match the criteria</returns>
        public IList<Product> GetWithName(string name)
        {
            var query = SessionContext.Session.QueryOver<Product>()
                                              .Where(Restrictions
                                              .On<Product>(x => x.Prod_Name)
                                              .IsLike(name)).List();
            IList<Product> products = query;

            return products;
        }

        /// <summary>
        /// Performs an EQUALS query to return all Products that match
        /// the given price criteria
        /// </summary>
        /// <param name="price">Price to search for</param>
        /// <returns>IList of all Products that match the criteria</returns>
        public IList<Product> GetWithPrice(float price)
        {
            var query = SessionContext.Session.QueryOver<Product>().Where(x => x.Price == price).List();
            IList<Product> products = query;

            return products;
        }
    }
}
