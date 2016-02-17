using System.Collections.Generic;
using DataEngine.RepositoryCore;
using NHibernate;

namespace DataEngine.Contexts
{
    public class SessionRepository<T> : Repository<T> where T : class, ISessionFactory
    {
        private readonly IList<ISessionFactory> _sessionFactories;

        public SessionRepository()
        {
            _sessionFactories = new List<ISessionFactory>();
        }

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

        public ISessionFactory Get(int index)
        {
            return _sessionFactories[index];
        }

        public void Clear()
        {
            _sessionFactories.Clear();
        }
    }
}
