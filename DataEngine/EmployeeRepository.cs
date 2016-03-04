using System.Collections.Generic;
using System.Linq;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.RepositoryCore;
using NHibernate.Event;
using NHibernate.Linq;

namespace DataEngine
{
    public class EmployeeRepository<T> : Repository<T> where T : Employee
    {
        public EmployeeRepository(ISessionContext sessionContext) : base(sessionContext)
        {

        }

        public IList<Employee> GetWithName(string name)
        {
            var query = SessionContext.Session.QueryOver<Employee>().Where(x => x.FirstName == name).List();
            IList<Employee> employees = query;

            return employees;
        }

        public IList<Employee> GetWithStoreId(int id)
        {
            var query = SessionContext.Session.QueryOver<Employee>().Where(x => x.StoreId == id).List();
            IList<Employee> employees = query;

            return employees;
        }
    }
}
