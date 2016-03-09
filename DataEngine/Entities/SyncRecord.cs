using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEngine.Entities
{
    /// <summary>
    /// SyncRecord entity, relates to the SyncRecord table
    /// Keeps track of the remote IDs assigned to new records on commit and rembers the local
    /// ID this allows us to update all foreign keys in the local DB before syncing
    /// </summary>
    public class SyncRecord : IEntity
    {
        public virtual int Id { get; protected set; }
        public virtual string Table_Id { get; set; }
        public virtual int Local_Id { get; set; }
        public virtual int Remote_Id { get; set; }
    }
}
