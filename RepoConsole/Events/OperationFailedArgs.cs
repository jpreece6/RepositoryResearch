using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    public class OperationFailedArgs : EventArgs
    {
        public Exception ExceptionObject { get; set; }
        public string Status { get; set; }

        public OperationFailedArgs(Exception ex)
        {
            ExceptionObject = ex;
        }

        public OperationFailedArgs(Exception ex, string status)
        {
            ExceptionObject = ex;
            Status = status;
        }
        
    }
}
