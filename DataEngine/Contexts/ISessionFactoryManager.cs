using NHibernate;

namespace DataEngine.Contexts
{
    public interface ISessionFactoryManager
    {
        ISessionFactory GetPreferred();
        ISessionFactory GetLocal();
        ISessionFactory Get(int i);
    }
}
