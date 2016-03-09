using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEngine.Entities
{
    /// <summary>
    /// Product eneity, relates to the Product table
    /// </summary>
    public class Product : IEntity
    {
        public virtual int Id { get; protected set; }
        public virtual string Prod_Name { get; set; }
        public virtual float Price { get; set; }
    }
}
