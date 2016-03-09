using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using SyncEngine;

namespace SyncConsole
{
    /// <summary>
    /// Sync program is a standalone test program to demonstrate syncing from local to remote
    /// databases.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var configReader = new ConfigReader(@"C:\repoSettings.xml");
            BaseConfig.Sources = configReader.GetAllInstancesOf("ConnectionString");

            SyncManager sync = new SyncManager();

            // Bind to all sync events
            sync.OnSyncComplete += Sync_OnSyncComplete;
            sync.OnSyncFailure += Sync_OnSyncFailure;
            sync.OnUpdateStatus += Sync_OnUpdateStatus;
            sync.OnSyncStart += Sync_OnSyncStart;
            sync.OnCleanUp += Sync_OnCleanUp;

            // SYNC!
            sync.SyncAllTables();

        }

        #region EventListeners
        /// <summary>
        /// Called by sync manager when cleaning up SyncRecords
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Sync_OnCleanUp(object sender, SyncEngine.Events.SyncCleanupArgs e)
        {
            Console.WriteLine("\nCleaning up - " + e.Status);
        }

        /// <summary>
        /// Called by sync manager when starting a new table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Sync_OnSyncStart(object sender, SyncEngine.Events.SyncStartedArgs e)
        {
            Console.WriteLine("\nSyncing... " + e.Status);
        }

        /// <summary>
        /// Called by sync manager when a record is synced
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Sync_OnUpdateStatus(object sender, SyncEngine.Events.ProgressEventArgs e)
        {
            Console.WriteLine(e.Status);
        }

        /// <summary>
        /// Called by sync when a table/entity failes to sync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Sync_OnSyncFailure(object sender, SyncEngine.Events.SyncFailedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n" + e.Status);
            Console.WriteLine("Error: " + e.ExceptionObject.Message);
        }

        /// <summary>
        /// Called by sync when all tables have been sync successfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Sync_OnSyncComplete(object sender, SyncEngine.Events.SyncCompleteArgs e)
        {
            Console.WriteLine("\n" + e.Status);
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        #endregion
    }
}
