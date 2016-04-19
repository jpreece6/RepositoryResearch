using System;
using NHibernate;

namespace DataEngine.Contexts
{
    /// <summary>
    /// Interface to expose session methods to the repositories
    /// </summary>
    public interface ISessionContext
    {
        ISession Session { get; }

        void Commit();
        void SaveOrUpdate<T>(T tData);
        void Remove<T>(T tData);
        bool CheckConnectionException(Exception ex);
    }
}
