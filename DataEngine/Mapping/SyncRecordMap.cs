using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using FluentNHibernate.Mapping;

namespace DataEngine.Mapping
{
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
