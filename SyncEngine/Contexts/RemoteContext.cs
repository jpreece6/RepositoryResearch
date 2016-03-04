using DataEngine.Contexts;

namespace SyncEngine.Contexts
{
    public class RemoteContext : SessionContext
    {
        public RemoteContext(ISessionFactoryManager sessionFactoryManager) : base(sessionFactoryManager)
        {
        }

        public override void OpenContextSession()
        {
            //Session = SessionFactoryManager.GetPreferred().OpenSession();
        }
    }
}
