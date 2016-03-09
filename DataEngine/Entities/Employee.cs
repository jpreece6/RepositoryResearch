using System.Data;

namespace DataEngine.Entities
{

    /// <summary>
    /// Employee entity, relates to the Employee table
    /// </summary>
    public class Employee : IEntity
    {
        public virtual int Id { get; protected set; }
        public virtual string FirstName { get; set; }
        public virtual int StoreId { get; set; }
    }
}
