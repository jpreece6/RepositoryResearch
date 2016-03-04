 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
 using Helpers;
 using RepoConsole.Views;

namespace RepoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //MainMenu main = new MainMenu();
            //main.Show();
            //var manager = new ViewManager();
            //manager.Start();

            // Load settings
            var configReader = new ConfigReader(@"C:\repoSettings.xml");
            BaseConfig.Sources = configReader.GetAllInstancesOf("ConnectionString");

            var main = new MainView();
            main.Show();

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
    }
}
