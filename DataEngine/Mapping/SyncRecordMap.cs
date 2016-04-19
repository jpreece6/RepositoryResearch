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
    public class SyncRecordMap : ClassMap<SyncRecord>
    {
        public SyncRecordMap()
        {
            Id(x => x.Id);
            Map(x => x.Table_Id);
            Map(x => x.Local_Id);
            Map(x => x.Remote_Id);
        }
    }
}
