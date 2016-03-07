 using System;
using System.Collections.Generic;
using System.Linq;
 using System.Runtime.InteropServices;
 using System.Text;
using System.Threading.Tasks;
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
        static void Main(string[] args)
        {
            // Load settings
            var configReader = new ConfigReader(@"C:\repoSettings.xml");
            BaseConfig.Sources = configReader.GetAllInstancesOf("ConnectionString");

            var sync = new SyncManager();

            sync.OnSyncComplete += Sync_OnSyncComplete;
            sync.OnSyncFailure += Sync_OnSyncFailure;
            sync.OnUpdateStatus += Sync_OnUpdateStatus;
            sync.OnSyncStart += Sync_OnSyncStart;
            sync.OnCleanUp += Sync_OnCleanUp;

            sync.SyncAllTables();

            var main = new MainView();
            main.Show();

            sync.SyncAllTables();

            /*
            // Load settings
            var configReader = new ConfigReader(@"C:\repoSettings.xml");
            BaseConfig.Sources = configReader.GetAllInstancesOf("ConnectionString");

            // Init page.. 
            var sessionFactManager = new SessionFactoryManager();
            var sessionContext = new SessionContext(sessionFactManager);
            sessionContext.OpenContextSession();

            var index = 0;


            EmployeeRepository<Employee> repo = new EmployeeRepository<Employee>(sessionContext);

            while (index != 3)
            {
   
                // Create button + assign values...
                var josh = new Employee {FirstName = "Josh " + index};

                try
                {
                    // Save button
                    repo.Save(josh);
                    repo.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Jump");
                }

                Console.WriteLine("Write " + index);
                if (index == 0)
                    Console.WriteLine("Shutdown SQL server now...");
                Console.ReadLine();
                index++;
            }


            /*foreach (var employ in repo.GetAll())
            {
                Console.WriteLine("Name : " + employ.FirstName);
            }*/

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void Sync_OnCleanUp(object sender, SyncEngine.Events.SyncCleanupArgs e)
        {
            Console.WriteLine("\nCleaning up - " + e.Status);
        }

        private static void Sync_OnSyncStart(object sender, SyncEngine.Events.SyncStartedArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n" + e.Status);
        }

        private static void Sync_OnUpdateStatus(object sender, SyncEngine.Events.ProgressEventArgs e)
        {
            Console.WriteLine(e.Status);
        }

        private static void Sync_OnSyncFailure(object sender, SyncEngine.Events.SyncFailedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n" + e.Status);
            Console.WriteLine("Error: " + e.ExceptionObject.Message);
        }

        private static void Sync_OnSyncComplete(object sender, SyncEngine.Events.SyncCompleteArgs e)
        {
            Console.WriteLine("\n" + e.Status);
        }
    }
}
