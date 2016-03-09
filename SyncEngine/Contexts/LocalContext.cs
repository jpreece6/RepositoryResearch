using DataEngine.Contexts;

namespace SyncEngine.Contexts
{
    /// <summary>
    /// Creates a new context for local only!
    /// </summary>
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
