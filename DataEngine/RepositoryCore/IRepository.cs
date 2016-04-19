using System.Collections;
using System.Collections.Generic;

namespace DataEngine.RepositoryCore
{
    /// <summary>
    /// Base repository interface def
    /// </summary>
    public interface IRepository
    {
        int Count();
        int Exists(int id);
        void Commit();
    }

    /// <summary>
    /// Extends the base repository interface with a generic version
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IRepository where T : class
    {
        void Save(T tData);
        T Get<U>(U id);
        void Remove(T tData);
        IList<T> GetAll();
    }
}
