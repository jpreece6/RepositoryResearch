using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncEngine.Events
{
    // REF http://stackoverflow.com/questions/6644247/simple-custom-event
    public class SyncFailedEventArgs
    {
        public string Status { get; private set; }
        public Exception ExceptionObject { get; set; }

        public SyncFailedEventArgs(Exception ex, string status)
        {
            Status = status;
            ExceptionObject = ex;
        }
    }
}
