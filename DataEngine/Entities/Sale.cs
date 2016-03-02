using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEngine.Entities
{
    public class Sale : IEntity
    {
        public virtual int Id { get; protected set; }
        public virtual int StoreId { get; set; }
        public virtual int ProductId { get; set; }
        public virtual DateTime Timestamp { get; set; }
    }
}
