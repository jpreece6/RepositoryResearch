using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace DataEngine.Mapping
{
    /// <summary>
    /// Tells the ORM which eneity and attributes to map to the employee table
    /// </summary>
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Id(x => x.Id);
            Map(x => x.Prod_Name);
            Map(x => x.Price);
        }
    }
}
