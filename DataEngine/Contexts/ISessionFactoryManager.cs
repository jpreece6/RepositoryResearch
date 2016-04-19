using NHibernate;

namespace DataEngine.Contexts
{
    /// <summary>
    /// Interface to expose factory methods to the session context
    /// </summary>
    public interface ISessionFactoryManager
    {
        ISessionFactory GetPreferred();
        ISessionFactory GetLocal();
        ISessionFactory Get(int i);
    }
}
