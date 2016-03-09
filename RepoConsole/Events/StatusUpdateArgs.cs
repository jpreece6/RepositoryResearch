using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{

    /// <summary>
    /// Returns a message to the user to indicate progress of an
    /// operation or further instrcutions
    /// </summary>
    public class StatusUpdateArgs : EventArgs
    {
        /// <summary>
        /// Status message to be printed to the user
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Creates a new instance of StatusUpdateArgs with a custom
        /// message provided
        /// </summary>
        /// <param name="status">String status</param>
        public StatusUpdateArgs(string status)
        {
            Status = status;
        }
    }
}
