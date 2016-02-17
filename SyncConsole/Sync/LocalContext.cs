using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Contexts;
using NHibernate;

namespace SyncConsole.Sync
{
    public class LocalContext : SessionContext
    {
        public LocalContext(ISessionFactoryManager sessionFactoryManager) : base(sessionFactoryManager)
        {
        }

        public override void OpenContextSession()
        {
            Session = SessionFactoryManager.GetLocal().OpenSession();
        }
    }
}
