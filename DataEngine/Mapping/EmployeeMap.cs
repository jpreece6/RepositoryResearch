using FluentNHibernate.Mapping;
using Employee = DataEngine.Entities.Employee;

namespace DataEngine.Mapping
{
    /// <summary>
    /// Tells the ORM which eneity and attributes to map to the employee table
    /// </summary>
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.Id);
            Map(x => x.FirstName);
            Map(x => x.StoreId);
        }
    }
}
