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
    /// <summary>
    /// Main run application which demonstrates a full database
    /// CRUD application that reacts to network failures
    /// </summary>
    class Program
    {
        private static SyncManager _sync;
        private static ConfigReader _configReader;

        private const string SettingsPath = @"C:\repoSettings.xml";

        static void Main(string[] args)
        {
            // Load settings
            _configReader = new ConfigReader(SettingsPath);
            BaseConfig.Sources = _configReader.GetAllInstancesOf("ConnectionString");

            // Register sync events
            _sync = new SyncManager();
            _sync.OnSyncComplete += Sync_OnSyncComplete;
            _sync.OnSyncFailure += Sync_OnSyncFailure;
            _sync.OnUpdateStatus += Sync_OnUpdateStatus;
            _sync.OnSyncStart += Sync_OnSyncStart;
            _sync.OnCleanUp += Sync_OnCleanUp;

            if (WasLastRun_Local())
            {
                BaseConfig.IsLocal = false;
                Sync();
            }

            var main = new MainView();
            main.Show();

            SaveRun_Status();
            if (BaseConfig.IsLocal)
            {
                BaseConfig.IsLocal = false;
                Sync();
            }

            //Console.ReadKey();
        }

        /// <summary>
        /// Clears the screen and calls sync
        /// </summary>
        private static void Sync()
        {
            Console.Clear();
            _sync.SyncAllTables();
        }

        /// <summary>
        /// Checks the settings file for the last run connection status
        /// </summary>
        /// <returns>True = remote connection, false = local connection</returns>
        private static bool WasLastRun_Local()
        {
            var elLastRun = _configReader.GetAllInstancesOf("LastRun");
            var status = elLastRun[0].Element("StatusLocal")?.Value;

            bool value;
            if (bool.TryParse(status, out value) == false)
                throw new Exception("Could not read last run status!");

            return value;
        }

        /// <summary>
        /// Saves the current connection status to the XML settings file
        /// </summary>
        private static void SaveRun_Status()
        {
            var elLastRun = _configReader.GetAllInstancesOf("LastRun");
            var status = elLastRun[0].Element("StatusLocal");
            status?.SetValue(BaseConfig.IsLocal.ToString());
            _configReader.SaveChanges(SettingsPath);
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
            Console.WriteLine("Error: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Called by sync when all tables have been sync successfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Sync_OnSyncComplete(object sender, SyncEngine.Events.SyncCompleteArgs e)
        {
            SaveRun_Status();

            Console.WriteLine("\n" + e.Status);
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        #endregion
    }
}
