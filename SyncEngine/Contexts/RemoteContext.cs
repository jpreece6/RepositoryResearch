using DataEngine.Contexts;

namespace SyncEngine.Contexts
{
    /// <summary>
    /// Creates a new session context for remote connection only
    /// </summary>
    public class RemoteContext : SessionContext
    {
        public RemoteContext(ISessionFactoryManager sessionFactoryManager) : base(sessionFactoryManager)
        {
        }
    }
}
