using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Contexts;
using RepoConsole.Events;

namespace RepoConsole.Presenter
{
    public abstract class Presenter
    {
        protected SessionContext SessionContext;

        public delegate void StatusUpdateHandler(object sender, StatusUpdateArgs e);
        public event StatusUpdateHandler OnUpdateStatus;

        public delegate void OperationFailedHandler(object sender, OperationFailedArgs e);
        public event OperationFailedHandler OnOperationFail;

        protected void OperationFailed(Exception ex)
        {
            if (OnOperationFail == null) return;
            
            var failArgs = new OperationFailedArgs(ex);
            OnOperationFail(this, failArgs);
        }

        protected void OperationFailed(Exception ex, string status)
        {
            if (OnOperationFail == null) return;

            var failArgs = new OperationFailedArgs(ex, status);
            OnOperationFail(this, failArgs);
        }

        protected void UpdateStatus(string status)
        {
            if (OnUpdateStatus == null) return;

            var statArgs = new StatusUpdateArgs(status);
            OnUpdateStatus(this, statArgs);
        }

        protected virtual bool OpenSession()
        {
            Console.WriteLine("\nConnecting....");

            try
            {
                SessionContext.OpenContextSession();
                Console.Clear();

                if (SessionContext.IsLocal())
                {
                    Console.WriteLine("Could not connect to server!\n- Now using local DB for this operation!\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to server!");
                return false;
            }

            return true;
        }
    }
}
