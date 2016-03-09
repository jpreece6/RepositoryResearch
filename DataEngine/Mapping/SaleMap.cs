using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using FluentNHibernate.Mapping;

namespace DataEngine.Mapping
{
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
