using NHibernate;

namespace DataEngine.Contexts
{
    public interface ISessionContext
    {
        ISession Session { get; }

        void Commit();
        void SaveOrUpdate<T>(T tData);
        void Remove<T>(T tData);
    }
}
