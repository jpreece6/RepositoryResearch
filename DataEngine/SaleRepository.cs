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
    public class SaleRepository<T> : Repository<T> where T : Sale
    {
        public SaleRepository(ISessionContext sessionContext) : base(sessionContext)
        {
            
        }

        public IList<Sale> GetWithStoreID(int id)
        {
            var query = SessionContext.Session.QueryOver<Sale>().Where(x => x.StoreId == id).List();
            IList<Sale> sales = query;

            return sales;
        }

        public IList<Sale> GetWithProductID(int id)
        {
            var query = SessionContext.Session.QueryOver<Sale>().Where(x => x.ProductId == id).List();
            IList<Sale> sales = query;

            return sales;
        }
    }
}
