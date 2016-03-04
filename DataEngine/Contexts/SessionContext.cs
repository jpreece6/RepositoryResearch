using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using NHibernate;
using NHibernate.Context;

namespace DataEngine.Contexts
{
    public class SessionContext : ISessionContext
    {
        public ISession Session { get; set; }
        protected readonly ISessionFactoryManager SessionFactoryManager;
        private bool _isLocal = false;

        public SessionContext(ISessionFactoryManager sessionFactoryManager)
        {
            SessionFactoryManager = sessionFactoryManager;
        }

        public virtual void OpenContextSession()
        {
            try
            {
                Session = SessionFactoryManager.GetPreferred()?.OpenSession();
                _isLocal = false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException)
                {
                    OpenSessionFallback();
                }
            }
        }

        public virtual void OpenSessionFallback()
        {
            try
            {
                Session = SessionFactoryManager.GetLocal()?.OpenSession();
                _isLocal = true;
                // Fire event to sync local if possible.....
            }
            catch (Exception ex)
            {
                // Somethig went horribly wrong....
            }
        }

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

        public bool CheckConnectionException(Exception ex)
        {
            // -__-
            // Forign HResult = -2146232832, ErrorCode = -2146232060
            // Con HResult = -2146232832, ErrorCode = -2146232060
            if (ex.InnerException.Message.Contains("network-related"))
            {
                // Fire event for failed attempt
                Session = SessionFactoryManager.GetLocal().OpenSession();
                _isLocal = true;
                return true;
            }

            /*if (Session.Connection.State == ConnectionState.Closed || Session.Connection == null)
            {
                // Fire event for failed attempt
                Session = SessionFactoryManager.GetLocal().OpenSession();
                _isLocal = true;
                return true;
            }*/
            return false;
        }

        public bool IsLocal()
        {
            return _isLocal;
        }
    }
}
