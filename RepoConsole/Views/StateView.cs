using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    public abstract class StateView
    {
        public void DisplayStatus()
        {
            Console.Clear();

            var status = (BaseConfig.IsLocal) ? "Local" : "Remote";

            Console.WriteLine("## Current State: " + status + " Connection ##\n");
        }
    }
}
