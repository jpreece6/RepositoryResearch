using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Contexts;

namespace SyncConsole.Sync
{
    public class RemoteContext : SessionContext
    {
        public RemoteContext(ISessionFactoryManager sessionFactoryManager) : base(sessionFactoryManager)
        {
        }

        public override void OpenContextSession()
        {
            Session = SessionFactoryManager.GetPreferred().OpenSession();
        }
    }
}
