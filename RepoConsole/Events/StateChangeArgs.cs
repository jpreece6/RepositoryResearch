using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    /// <summary>
    /// Resturns a state message when the network state changes
    /// </summary>
    public class StateChangeArgs : EventArgs
    {
        public string Status { get; set; }

        public StateChangeArgs(string status)
        {
            Status = status;
        }
    }
}
