using DataEngine.Contexts;

namespace SyncEngine.Contexts
{
    public class LocalContext : SessionContext
    {
        public LocalContext(ISessionFactoryManager sessionFactoryManager) : base(sessionFactoryManager)
        {
        }

        public override void OpenContextSession()
        {
            OpenSessionFallback();
        }
    }
}
