using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.RepositoryCore;
using NHibernate.Event;
using NHibernate.Linq;

namespace DataEngine
{
    public class ProductRepository<T> : Repository<T> where T : Product
    {

        //public bool AllowLocalCreation = false;

        public ProductRepository(ISessionContext sessionContext) : base(sessionContext)
        {
            AllowLocalEdits = false;
        }

        public IList<Product> GetWithName(string name)
        {
            var query = SessionContext.Session.QueryOver<Product>().Where(x => x.Prod_Name == name).List();
            IList<Product> products = query;

            return products;
        }

        public IList<Product> GetWithPrice(float price)
        {
            var query = SessionContext.Session.QueryOver<Product>().Where(x => x.Price == price).List();
            IList<Product> products = query;

            return products;
        }
    }
}
