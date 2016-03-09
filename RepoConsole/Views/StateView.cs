using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    /// <summary>
    /// Abstract class that all view should inherit
    /// allows us to display the current session state to the
    /// console
    /// </summary>
    public abstract class StateView
    {
        /// <summary>
        /// Prints the current session state to the console
        /// </summary>
        public void DisplayStatus()
        {
            Console.Clear();

            var status = (BaseConfig.IsLocal) ? "Local" : "Remote";

            Console.WriteLine("## Current State: " + status + " Connection ##\n");
        }
    }
}
