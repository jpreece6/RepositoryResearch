using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEngine.Entities
{
    public class SyncRecord : IEntity
    {
        public virtual int Id { get; protected set; }
        public virtual string Table_Id { get; set; }
        public virtual int Local_Id { get; set; }
        public virtual int Remote_Id { get; set; }
    }
}
