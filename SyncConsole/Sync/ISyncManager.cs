using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using DataEngine.RepositoryCore;

namespace SyncConsole.Sync
{
    public interface ISyncManager
    {
        void CreateContexts();
        void SyncAllTables();
    }
}
