using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEngine.Entities
{
    /// <summary>
    /// Store eneity, relates to the Store table
    /// </summary>
    public class Store : IEntity
    {
        public virtual int Id { get; protected set; }
        public virtual String StoreName { get; set; }
    }
}
