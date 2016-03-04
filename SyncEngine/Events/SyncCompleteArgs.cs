using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncEngine.Events
{
    public class SyncCompleteArgs
    {
        public string Status { get; private set; }

        public SyncCompleteArgs(string status)
        {
            Status = status;
        }
    }
}
