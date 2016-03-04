using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncEngine.Events
{
    public class SyncStartedArgs
    {
        public string Status { get; private set; }

        public SyncStartedArgs(string status)
        {
            Status = status;
        }
    }
}
