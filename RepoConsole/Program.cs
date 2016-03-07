 using System;
using System.Collections.Generic;
 using System.Data;
 using System.Linq;
 using System.Runtime.InteropServices;
 using System.Text;
using System.Threading.Tasks;
 using System.Xml.Linq;
 using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
 using Helpers;
 using RepoConsole.Views;
 using SyncEngine;

namespace RepoConsole
{
    class Program
    {
        private static SyncManager _sync;
        private static ConfigReader _configReader;

        static void Main(string[] args)
        {
            // Load settings
            _configReader = new ConfigReader(@"C:\repoSettings.xml");
            BaseConfig.Sources = _configReader.GetAllInstancesOf("ConnectionString");

            // Register sync events
            _sync = new SyncManager();
            _sync.OnSyncComplete += Sync_OnSyncComplete;
            _sync.OnSyncFailure += Sync_OnSyncFailure;
            _sync.OnUpdateStatus += Sync_OnUpdateStatus;
            _sync.OnSyncStart += Sync_OnSyncStart;
            _sync.OnCleanUp += Sync_OnCleanUp;

            Sync();
            BaseConfig.IsLocal = false;

            var main = new MainView();
            main.Show();

            BaseConfig.IsLocal = false;
            Sync();

            Console.ReadKey();
        }

        private static void Sync()
        {
            Console.Clear();
            _sync.SyncAllTables();
        }

        private static void GetLastRun_Status()
        {
            var elLastRun = _configReader.GetAllInstancesOf("LastRun");
            var status = elLastRun[0].Element("StatusLocal")?.Value;

            bool value;
            if (bool.TryParse(status, out value))
            {
                BaseConfig.IsLocal = value;
            }
        }

        private static void SaveRun_Status()
        {
            var elLastRun = _configReader.GetAllInstancesOf("LastRun");
            var status = elLastRun[0].Element("StatusLocal");
            status?.SetValue(BaseConfig.IsLocal.ToString());
        }

        #region EventListeners
        private static void Sync_OnCleanUp(object sender, SyncEngine.Events.SyncCleanupArgs e)
        {
            Console.WriteLine("\nCleaning up - " + e.Status);
        }

        private static void Sync_OnSyncStart(object sender, SyncEngine.Events.SyncStartedArgs e)
        {
            Console.WriteLine("\nSyncing... " + e.Status);
        }

        private static void Sync_OnUpdateStatus(object sender, SyncEngine.Events.ProgressEventArgs e)
        {
            Console.WriteLine(e.Status);
        }

        private static void Sync_OnSyncFailure(object sender, SyncEngine.Events.SyncFailedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n" + e.Status);
            Console.WriteLine("Error: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
            Console.ReadLine();
        }

        private static void Sync_OnSyncComplete(object sender, SyncEngine.Events.SyncCompleteArgs e)
        {
            Console.WriteLine("\n" + e.Status);
        }
        #endregion
    }
}
