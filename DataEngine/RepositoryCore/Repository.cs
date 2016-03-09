using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DataEngine.Contexts;
using DataEngine.Entities;
using NHibernate.Criterion;

namespace DataEngine.RepositoryCore
{
    /// <summary>
    /// Main repo difinition
    /// http://nhibernate.info/doc/nhibernate-reference/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ISessionContext SessionContext;

        protected Repository()
        {
        }

        protected Repository(ISessionContext sessionContext)
        {
            SessionContext = sessionContext;
        }

        /// <summary>
        /// Add a new object to the DAL
        /// </summary>
        /// <param name="tData">Object to store</param>
        public virtual void Save(T tData)
        {
            SessionContext.SaveOrUpdate(tData);
        }

        /// <summary>
        /// Removes an object from the DAL
        /// </summary>
        /// <param name="tData">Object to remove</param>
        public virtual void Remove(T tData)
        {
            SessionContext.Remove(tData);
        }

        /// <summary>
        /// Push cahnges to the database
        /// </summary>
        public void Commit()
        {
            SessionContext.Commit();
        }

        /// <summary>
        /// Retrieve an object by id from the DAL
        /// </summary>
        /// <param name="id">id of object to retrieve</param>
        /// <returns>Object from DAL</returns>
        public virtual T Get<U>(U id)
        {
            return (T) SessionContext.Session.Get(typeof(T), id);
        }

        /// <summary>
        /// Get all data from an entity table
        /// </summary>
        /// <returns></returns>
        public virtual IList<T> GetAll()
        {
            try
            {
                return SessionContext.Session.CreateCriteria<T>().List<T>();
            }
            catch (Exception ex)
            {
                if (!SessionContext.CheckConnectionException(ex))
                    throw;
                return new List<T>();
            }
        }

        /// <summary>
        /// Count all records in a table
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return
                SessionContext.Session.CreateCriteria<T>()
                    .SetProjection(Projections.Count(Projections.Id()))
                    .UniqueResult<int>();
        }

        /// <summary>
        /// Check if a record exists in the databae
        /// </summary>
        /// <param name="id">Id to check</param>
        /// <returns>Int number of records that match</returns>
        public virtual int Exists(int id)
        {
            return
                SessionContext.Session.CreateCriteria<T>()
                    .Add(Restrictions.Eq("id", id))
                    .SetProjection(Projections.Count(Projections.Id()))
                    .FutureValue<int>().Value;
        }
    }
}
