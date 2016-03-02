using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.Helpers;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    class EmployeePresenter : IPresenter
    {
        private readonly IViewEmployee _view;
        private EmployeeRepository<Employee> _employeeRepository; 
         
        public EmployeePresenter(IViewEmployee view)
        {
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;
            Initialise();
        }

        private void View_GetAll(object sender, EventArgs e)
        {
            Console.WriteLine("");
            var emps = _employeeRepository.GetAll();

            if (emps.Count <= 0)
            {
                Console.WriteLine("no employees found.");
                return;
            }

            foreach (var employee in emps)
            {
                Console.WriteLine("ID: " + employee.Id);
                Console.WriteLine("First Name: " + employee.FirstName);
                Console.WriteLine("");
            }
        }

        private void View_Remove(object sender, EventArgs e)
        {
            Console.WriteLine("");
            var emp = _employeeRepository.Get(_view.Id);

            if (emp == null)
            {
                Console.WriteLine("No employee found with ID of " + _view.Id);
                return;
            }

            _employeeRepository.Remove(emp);
            _employeeRepository.Commit();

            Console.WriteLine("Removed");
        }

        private void View_Add(object sender, EventArgs e)
        {
            Console.WriteLine("");
            var emp = new Employee();
            emp.FirstName = _view.FirstName;
            emp.StoreId = _view.StoreId;

            _employeeRepository.Save(emp);
            _employeeRepository.Commit();

            Console.WriteLine("New employee created with ID of " + emp.Id);
        }

        private void View_Get(object sender, EventArgs e)
        {
            if (_view.Id != 0)
            {
                var emp = _employeeRepository.Get(_view.Id);
                if (emp == null)
                {
                    Console.WriteLine("No employee found with ID of " + _view.Id);
                    return;
                }

                Console.WriteLine("First Name: " + emp.FirstName);
            }

            if (_view.FirstName != null)
            {
                var emps = _employeeRepository.GetWithName(_view.FirstName);
                if (emps.Count == 0)
                {
                    Console.WriteLine("No employee(s) found with name " + _view.FirstName);
                    return;
                }

                foreach (var employee in emps)
                {
                    Console.WriteLine("ID: " + employee.Id);
                    Console.WriteLine("Name: " + employee.FirstName);
                    Console.WriteLine("");
                }
            }
        }

        public void Initialise()
        {
            // Load settings
            var configReader = new ConfigReader(@"C:\repoSettings.xml");
            BaseConfig.Sources = configReader.GetAllInstancesOf("ConnectionString");

            // Init page.. 
            var sessionFactManager = new SessionFactoryManager();
            var sessionContext = new SessionContext(sessionFactManager);
            sessionContext.OpenContextSession();

            _employeeRepository = new EmployeeRepository<Employee>(sessionContext);
        }
    }
}
