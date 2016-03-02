using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEngine.Entities
{
    public class Store : IEntity
    {
        public virtual int Id { get; protected set; }
        public virtual String StoreName { get; set; }
    }
}
