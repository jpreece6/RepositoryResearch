using System.Collections.Generic;
using System.Linq;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.RepositoryCore;
using NHibernate.Event;
using NHibernate.Linq;

namespace DataEngine
{
    public class ProductRepository<T> : Repository<T> where T : class
    {
        public ProductRepository(ISessionContext sessionContext) : base(sessionContext)
        {

        }

        public IList<Product> GetWithName(string name)
        {
            var query = SessionContext.Session.Query<Product>().ToList();
            IList<Product> products = query;

            return products;
        }
    }
}
