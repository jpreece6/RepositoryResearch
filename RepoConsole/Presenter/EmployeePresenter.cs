using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    class EmployeePresenter : IPresenter
    {
        private readonly IViewEmployee _view;
        private readonly EmployeeRepository<Employee> _employeeRepository;
        private readonly SessionContext _sessionContext;
         
        public EmployeePresenter(IViewEmployee view)
        {
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;

            // Init page.. 
            var sessionFactManager = new SessionFactoryManager();
            _sessionContext = new SessionContext(sessionFactManager);

            _employeeRepository = new EmployeeRepository<Employee>(_sessionContext);
        }

        private void View_GetAll(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            IList<Employee> emps;

            try
            {
                emps = _employeeRepository.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve employees\n");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            if (emps.Count <= 0)
            {
                Console.WriteLine("no employees found.");
                return;
            }

            foreach (var employee in emps)
            {
                Console.WriteLine("ID: " + employee.Id);
                Console.WriteLine("First Name: " + employee.FirstName);
                Console.WriteLine("Store ID: " + employee.StoreId);
                Console.WriteLine("");
            }
        }

        private void View_Remove(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Employee emp;

            try
            {
                emp = _employeeRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve Employee");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            if (emp == null)
            {
                Console.WriteLine("No employee found with ID of " + _view.Id);
                return;
            }

            try
            {
                _employeeRepository.Remove(emp);
                _employeeRepository.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not remove employee");
                Console.WriteLine("Error: " + ex.InnerException?.Message ?? ex.Message);
                return;
            }

            Console.WriteLine("Employee Removed!");
        }

        private void View_Add(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            var emp = new Employee();
            emp.FirstName = _view.FirstName;
            emp.StoreId = _view.StoreId;

            try
            {
                _employeeRepository.Save(emp);
                _employeeRepository.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not save employee\n\n");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            Console.WriteLine("New employee created with ID of " + emp.Id);
        }

        private void View_Get(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            if (_view.Id != 0)
            {
                Employee emp;

                try
                {
                    emp = _employeeRepository.Get(_view.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not retrieve Employee in Get");
                    Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                    return;
                }

                if (emp == null)
                {
                    Console.WriteLine("No employee found with ID of " + _view.Id);
                    return;
                }

                Console.WriteLine("First Name: " + emp.FirstName);
                return;
            }

            if (_view.FirstName != null || _view.StoreId != 0)
            {
                IList<Employee> emps = new List<Employee>();

                try
                {
                    if (_view.FirstName != null)
                    {
                        emps = _employeeRepository.GetWithName(_view.FirstName);
                    } else if (_view.StoreId != 0)
                    {
                        emps = _employeeRepository.GetWithStoreId(_view.StoreId);
                    }

                } catch (Exception ex)
                {
                    Console.WriteLine("Could not retrieve Employee in by Name or Store ID");
                    Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                    return;
                }

                if (emps.Count == 0)
                {
                    Console.WriteLine("No employee(s) found with name " + _view.FirstName);
                    return;
                }

                foreach (var employee in emps)
                {
                    Console.WriteLine("ID: " + employee.Id);
                    Console.WriteLine("Name: " + employee.FirstName);
                    Console.WriteLine("Store ID: " + employee.StoreId);
                    Console.WriteLine("");
                }
            }
        }

        private bool OpenSession()
        {
            Console.WriteLine("\nConnecting....");

            try
            {
                _sessionContext.OpenContextSession();
                Console.Clear();

                if (_sessionContext.IsLocal())
                {
                    Console.WriteLine("Could not connect to server!\n- Now using local DB for this operation!\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to server!");
                return false;
            }

            return true;
        }
    }
}
