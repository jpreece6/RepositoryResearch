using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using FluentNHibernate.Mapping;

namespace DataEngine.Mapping
{
    /// <summary>
    /// Tells the ORM which eneity and attributes to map to the employee table
    /// </summary>
    public class SaleMap : ClassMap<Sale>
    {
        public SaleMap()
        {
            Id(x => x.Id);
            Map(x => x.StoreId);
            Map(x => x.ProductId);
            Map(x => x.SaleTime);
        }
    }
}
