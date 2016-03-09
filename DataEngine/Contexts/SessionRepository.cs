using System.Collections.Generic;
using DataEngine.RepositoryCore;
using NHibernate;

namespace DataEngine.Contexts
{
    /// <summary>
    /// Repository to store session factories
    /// </summary>
    /// <typeparam name="T">Session factory type</typeparam>
    public class SessionRepository<T> : Repository<T> where T : class, ISessionFactory
    {
        private readonly IList<ISessionFactory> _sessionFactories;

        public SessionRepository()
        {
            _sessionFactories = new List<ISessionFactory>();
        }

        /// <summary>
        /// Add a new factory to the repository
        /// </summary>
        /// <param name="tData">Factory to save</param>
        public override void Save(T tData)
        {
            _sessionFactories.Add(tData);
        }

        /// <summary>
        /// Returns all session factories stored here
        /// </summary>
        /// <returns>Dictionary containing all session factories</returns>
        public IList<ISessionFactory> GetAll()
        {
            return _sessionFactories;
        }

        /// <summary>
        /// Returns the preferred session factory, this is the first connection string in the config
        /// </summary>
        /// <returns>Preferred session factory</returns>
        public ISessionFactory GetCurrent()
        {
            return _sessionFactories.Count > 0 ? _sessionFactories[0] : null;
        }

        /// <summary>
        /// Returns a  specific factory
        /// </summary>
        /// <param name="index">Index of factory to return</param>
        /// <returns>Session factory</returns>
        public ISessionFactory Get(int index)
        {
            return _sessionFactories[index];
        }

        /// <summary>
        /// Clears all items from the repository
        /// </summary>
        public void Clear()
        {
            _sessionFactories.Clear();
        }
    }
}
