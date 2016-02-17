using System.Collections;
using System.Collections.Generic;

namespace DataEngine.RepositoryCore
{
    public interface IRepository
    {
        int Count();
        int Exists(int id);
        void Commit();
    }

    public interface IRepository<T> : IRepository where T : class
    {
        void Save(T tData);
        T Get<U>(U id);
        void Remove(T tData);
        IList<T> GetAll();
    }
}
