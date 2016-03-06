using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncEngine.Events
{
    public class SyncCleanupArgs : EventArgs
    {
        public string Status { get; set; }

        public SyncCleanupArgs(string status)
        {
            Status = status;
        }
    }
}
