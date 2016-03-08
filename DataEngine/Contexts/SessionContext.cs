using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Helpers;
using NHibernate;
using NHibernate.Context;

namespace DataEngine.Contexts
{
    /// <summary>
    /// Handles the connection between the program and the database.
    /// Also manages the connection switch between local and remote.
    /// </summary>
    public class SessionContext : ISessionContext
    {
        public ISession Session { get; set; }
        protected readonly ISessionFactoryManager SessionFactoryManager;
        private bool _isLocal = false;

        public SessionContext(ISessionFactoryManager sessionFactoryManager)
        {
            SessionFactoryManager = sessionFactoryManager;
        }

        /// <summary>
        /// Opens a new session to the preferred (remote) database
        /// </summary>
        public virtual void OpenContextSession()
        {
            // If we're running on remote open remote session
            if (BaseConfig.IsLocal == false) // Lock state
            {
                try
                {
                    Session = SessionFactoryManager.GetPreferred()?.OpenSession();
                    _isLocal = false;
                }
                catch (Exception ex)
                {
                    if (!CheckConnectionException(ex))
                        throw;
                }
            }
            else
            {
                // Open local session!
                OpenSessionFallback();
            }
        }

        /// <summary>
        /// Opens a new session to the fallback (local) database
        /// </summary>
        public virtual void OpenSessionFallback()
        {
            try
            {
                Session = SessionFactoryManager.GetLocal()?.OpenSession();
                _isLocal = true;
            }
            catch (Exception ex)
            {
                if (!CheckConnectionException(ex))
                    throw;
            }
        }

        /// <summary>
        /// Saves or updates a entity within the session's persister
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="tData">Entity to save</param>
        public void SaveOrUpdate<T>(T tData)
        {
            try
            {
                Session.SaveOrUpdate(tData);
            }
            catch(Exception ex)
            {
                if (!CheckConnectionException(ex))
                    throw;
            }
        }

        /// <summary>
        /// Removes an entity from the session's persister
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="tData">Entity to remove</param>
        public void Remove<T>(T tData)
        {
            try
            {
                Session.Delete(tData);
            }
            catch(Exception ex)
            {
                if (!CheckConnectionException(ex))
                    throw;
            }
        }

        /// <summary>
        /// Pushes any entities in the session's persister to the database
        /// </summary>
        public void Commit()
        {
            try
            {
                using (var transaction = Session.BeginTransaction())
                {
                    transaction.Commit(); // Commit changes to database
                    Session.Clear(); // TODO FIND FIX!!!! FOR JEDRIVER AUTONUMBER
                }
            }
            catch (Exception ex)
            {
                if (!CheckConnectionException(ex))
                    throw;
            }
        }

        /// <summary>
        /// Checks to see if the exception was caused by
        /// a network related issue. If so we can assume the connection to the database
        /// has dropped so we switch the session over to the local database
        /// </summary>
        /// <param name="ex">Exception object to analyse</param>
        /// <returns>True if exception was caused by a network issue</returns>
        public bool CheckConnectionException(Exception ex)
        {
            // -__-
            // Forign HResult = -2146232832, ErrorCode = -2146232060
            // Con HResult = -2146232832, ErrorCode = -2146232060
            if (ex.InnerException != null)
            {
                if (ex.InnerException.Message.Contains("network-related"))
                {
                    // Fire event for failed attempt
                    Session = SessionFactoryManager.GetLocal().OpenSession();
                    BaseConfig.IsLocal = true;
                    _isLocal = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the state of the current session object
        /// </summary>
        /// <returns>True if session is working with the local DB</returns>
        public bool IsLocal()
        {
            return _isLocal;
        }
    }
}
