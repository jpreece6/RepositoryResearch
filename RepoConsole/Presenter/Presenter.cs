using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Contexts;
using Helpers;
using RepoConsole.Events;

namespace RepoConsole.Presenter
{
    /// <summary>
    /// Base Presenter class contains all the events that a presenter
    /// needs to communicate with a view. Also contains the open session method
    /// to ensure a connection is possible.
    /// </summary>
    public abstract class Presenter
    {
        protected SessionContext SessionContext;

        #region Events
        public delegate void StatusUpdateHandler(object sender, StatusUpdateArgs e);
        public event StatusUpdateHandler OnUpdateStatus;

        public delegate void OperationFailedHandler(object sender, OperationFailedArgs e);
        public event OperationFailedHandler OnOperationFail;
        #endregion

        /// <summary>
        /// Fired when a CRUD operation fails, passes back the
        /// exception object to print out the exception details
        /// </summary>
        /// <param name="ex">Exception object for processing</param>
        protected void OperationFailed(Exception ex)
        {
            if (OnOperationFail == null) return;
            
            var failArgs = new OperationFailedArgs(ex);
            OnOperationFail(this, failArgs);
        }

        /// <summary>
        /// Fired when a CRUD operation fails, passes back the
        /// exception object to print out the exception details. Also
        /// contains a status message for a more specific error message
        /// </summary>
        /// <param name="ex">Exception object to process</param>
        /// <param name="status">Status string</param>
        protected void OperationFailed(Exception ex, string status)
        {
            if (OnOperationFail == null) return;

            var failArgs = new OperationFailedArgs(ex, status);
            OnOperationFail(this, failArgs);
        }

        /// <summary>
        /// Fired when a task of signifigence has been run,
        /// gives extra details to the user
        /// </summary>
        /// <param name="status">String message, detailing current status</param>
        protected void UpdateStatus(string status)
        {
            if (OnUpdateStatus == null) return;

            var statArgs = new StatusUpdateArgs(status);
            OnUpdateStatus(this, statArgs);
        }

        /// <summary>
        /// Attempts to open a new session
        /// </summary>
        /// <returns>True if connection established, false if local and remote are unavailable</returns>
        protected virtual bool OpenSession()
        {
            UpdateStatus("\nConnecting....");
            bool connectionState = BaseConfig.IsLocal; // remember the connection state

            try
            {
                SessionContext.OpenContextSession();
                Console.Clear();

                // If we're running on local inform the user
                // only show if we were running remote then changed to local
                if (SessionContext.IsLocal() && connectionState == false)
                {
                    OperationFailed(new Exception("Could not connect to server!"), "- Now using local DB for all operations until restart!");
                }
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not connect to server!");
                return false;
            }

            return true;
        }
    }
}
