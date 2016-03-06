using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using SyncEngine;

namespace SyncConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var configReader = new ConfigReader(@"C:\repoSettings.xml");
            BaseConfig.Sources = configReader.GetAllInstancesOf("ConnectionString");

            // TODO local repo will still lose the original id of its object due to autonumber....
            SyncManager sync = new SyncManager();

            sync.OnSyncComplete += Sync_OnSyncComplete;
            sync.OnSyncFailure += Sync_OnSyncFailure;
            sync.OnUpdateStatus += Sync_OnUpdateStatus;
            sync.OnSyncStart += Sync_OnSyncStart;
            sync.OnCleanUp += Sync_OnCleanUp;

            sync.SyncAllTables();

            /*var sessionFactManager = new SessionFactoryManager();
            var readContext = new LocalContext(sessionFactManager);
            var writeContext = new RemoteContext(sessionFactManager);

            readContext.OpenContextSession();
            writeContext.OpenContextSession();

            var employeeLocalRepository = new EmployeeRepository<Employee>(readContext);
            var employeeRemoteRepository = new EmployeeRepository<Employee>(writeContext);

            Console.WriteLine("Num records : " + employeeLocalRepository.Count() + "\n");

            // Get all from ....
            foreach (var emply in employeeLocalRepository.GetAll())
            {
                var tag = "Inserting";

                // Do we need to insert or update?
                // TODO ID for local is autonumber this prevents updating properly....
                if (employeeRemoteRepository.Exists(emply.Id) == 0)
                {
                    var tmpEmply = new Employee();
                    tmpEmply.FirstName = emply.FirstName;
                    employeeRemoteRepository.Save(tmpEmply);
                }
                else
                {
                    tag = "Updating";
                    employeeRemoteRepository.Save(emply);
                }

                Console.WriteLine(tag + "..... {ID = " + emply.Id + "} {FirstName = " + emply.FirstName + "}\n");

                // Remove from local
                employeeLocalRepository.Remove(emply);
            }

            Console.WriteLine("Remote records : " + employeeRemoteRepository.Count());

            employeeRemoteRepository.Commit();
            employeeLocalRepository.Commit();*/

            Console.ReadLine();

            // Sync all to ....

        }

        private static void Sync_OnCleanUp(object sender, SyncEngine.Events.SyncCleanupArgs e)
        {
            Console.WriteLine("\nCleaning up - " + e.Status);
        }

        private static void Sync_OnSyncStart(object sender, SyncEngine.Events.SyncStartedArgs e)
        {
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
