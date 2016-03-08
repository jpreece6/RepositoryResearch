using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;
using DataEngine.Entities;
using DataEngine.Mapping;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Helpers;
using NHibernate;

namespace DataEngine.Contexts
{
    /// <summary>
    /// Creates a new session object to connect to an appropriate database
    /// </summary>
    public class SessionFactoryManager : ISessionFactoryManager
    {
        private readonly SessionRepository<ISessionFactory> _sessionsRepository;

        public SessionFactoryManager()
        {
            _sessionsRepository = new SessionRepository<ISessionFactory>();
        }

        /// <summary>
        /// Gets the session related to the preferred (remote) DB
        /// </summary>
        /// <returns>Session factory</returns>
        public ISessionFactory GetPreferred()
        {
            _sessionsRepository.Clear();
            LoadSpecifiedDataSource("Preferred");

            return _sessionsRepository.GetCurrent();
        }

        /// <summary>
        /// Gets the session object related to the local database
        /// </summary>
        /// <returns>Session factory</returns>
        public ISessionFactory GetLocal()
        {
            _sessionsRepository.Clear();
            LoadSpecifiedDataSource("Local");

            return _sessionsRepository.GetCurrent();
        }

        /// <summary>
        /// Gets a specific factory from the repository
        /// </summary>
        /// <param name="index">Index of factory to return</param>
        /// <returns>Session factory</returns>
        public ISessionFactory Get(int index)
        {
            return _sessionsRepository.Get(index);
        }

        /// <summary>
        /// Loads a connection string from the XML settings file
        /// and populates the session repository with the new
        /// session factory
        /// </summary>
        /// <param name="strMode">XML element to load</param>
        public void LoadSpecifiedDataSource(string strMode)
        {
            var dataSources = BaseConfig.Sources;
            foreach (var dataSource in dataSources)
            {
                var mode = (string) dataSource.Attribute("mode");
                if (mode.Equals(strMode.ToLower()))
                {

                    var type = (string) dataSource.Attribute("type");
                    var conMethod = (ConnectionMethod) type;
                    IPersistenceConfigurer connectionString = null;

                    // Get the correct PersistenceConfigurer based on our method
                    if (conMethod == ConnectionMethod.MsSql2012)
                    {
                        connectionString = MsSqlConfiguration.MsSql2012.ConnectionString((string) dataSource);

                    }
                    else if (conMethod == ConnectionMethod.JetProvider)
                    {
                        connectionString = JetDriverConfiguration.Standard.ConnectionString((string) dataSource);
                    }

                    // Store in local repository
                    _sessionsRepository.Save(CreateSessionFactory(connectionString));

                    break;
                }
            }
            
        }

        /// <summary>
        /// Creates a new session factory
        /// </summary>
        /// <param name="persistenceConfigurer">Method of connection (includes connection string)</param>
        /// <returns>New session factory</returns>
        public ISessionFactory CreateSessionFactory(IPersistenceConfigurer persistenceConfigurer)
        {
            // NOTE JetDriver fails to load copy .dll to console or main app output dir
            var config = new StoreConfiguration();

            return Fluently.Configure()
                .Database(persistenceConfigurer)
                .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Employee>(config)))
                .BuildSessionFactory();
        }
    }
}
