using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    public class StateChangeArgs : EventArgs
    {
        public string Status { get; set; }

        public StateChangeArgs(string status)
        {
            Status = status;
        }
    }
}
