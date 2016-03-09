using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    /// <summary>
    /// Holds an exception object that is created during an exception
    /// that has occured during a CRUD operation. This allows us to print out
    /// an error message to the user.
    /// </summary>
    public class OperationFailedArgs : EventArgs
    {
        /// <summary>
        /// Exception object containing the error information
        /// of the exception that has occured
        /// </summary>
        public Exception ExceptionObject { get; set; }

        /// <summary>
        /// Status provides extra error information to the user if
        /// we know what the general cause of the error was...
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Creates a new instance of OperationFailedArgs with an exception object
        /// </summary>
        /// <param name="ex">Exception object</param>
        public OperationFailedArgs(Exception ex)
        {
            ExceptionObject = ex;
        }

        /// <summary>
        /// Creates a new instance of OperationFailedArgs with an exception object,
        /// with a status message for extra info
        /// </summary>
        /// <param name="ex">Exception object</param>
        /// <param name="status">Status of the error that just occured</param>
        public OperationFailedArgs(Exception ex, string status)
        {
            ExceptionObject = ex;
            Status = status;
        }
        
    }
}
