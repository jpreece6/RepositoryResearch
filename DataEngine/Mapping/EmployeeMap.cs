using FluentNHibernate.Mapping;
using Employee = DataEngine.Entities.Employee;

namespace DataEngine.Mapping
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.Id);
            Map(x => x.FirstName);
        }
    }
}
