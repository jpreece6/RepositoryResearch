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
                CheckConnectionException(ex);
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
                CheckConnectionException(ex);
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
                CheckConnectionException(ex);
            }
        }

        private void CheckConnectionException(Exception ex)
        {
            if (ex.InnerException.HResult == -2146232060)
            {
                // Fire event for failed attempt
                Session = SessionFactoryManager.GetLocal().OpenSession();
                _isLocal = true;
            }
        }

        public bool IsLocal()
        {
            return _isLocal;
        }
    }
}
