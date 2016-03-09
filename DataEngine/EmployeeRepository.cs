using System.Collections.Generic;
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
    /// Repository to store Employee entities also includes 
    /// queries specific to the Employee entity
    /// </summary>
    /// <typeparam name="T">Entity type must be of an Employee type</typeparam>
    public class EmployeeRepository<T> : Repository<T> where T : Employee
    {
        public EmployeeRepository(ISessionContext sessionContext) : base(sessionContext)
        {
            AllowLocalEdits = true;
        }

        /// <summary>
        /// Performs a LIKE query to find all Employees who (semi)match
        /// the name provided
        /// </summary>
        /// <param name="name">String name to search for</param>
        /// <returns>IList of employee's that match the criteria</returns>
        public IList<Employee> GetWithName(string name)
        {
            var query = SessionContext.Session.QueryOver<Employee>()
                                              .Where(Restrictions.On<Employee>(x => x.FirstName)
                                              .IsLike(name)).List();
            IList<Employee> employees = query;

            return employees;
        }

        /// <summary>
        /// Performs a EQUALS query to find all Employees that match
        /// the given store ID
        /// </summary>
        /// <param name="id">Store ID to match</param>
        /// <returns>IList of Employees that match the criteria</returns>
        public IList<Employee> GetWithStoreId(int id)
        {
            var query = SessionContext.Session.QueryOver<Employee>().Where(x => x.StoreId == id).List();
            IList<Employee> employees = query;

            return employees;
        }
    }
}
