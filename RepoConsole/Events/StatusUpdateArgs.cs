using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    public class StatusUpdateArgs : EventArgs
    {
        public string Status { get; set; }

        public StatusUpdateArgs(string status)
        {
            Status = status;
        }
    }
}
