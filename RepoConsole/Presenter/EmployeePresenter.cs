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
    class EmployeePresenter : Presenter
    {
        private readonly IViewEmployee _view;
        private readonly EmployeeRepository<Employee> _employeeRepository;
         
        public EmployeePresenter(IViewEmployee view)
        {
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;
            _view.Edit += View_Edit;

            // Init page.. 
            var sessionFactManager = new SessionFactoryManager();
            SessionContext = new SessionContext(sessionFactManager);

            _employeeRepository = new EmployeeRepository<Employee>(SessionContext);
        }

        private void View_Edit(object sender, EventArgs e)
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
                OperationFailed(ex, "Could not retrieve Employee in Get");
                return;
            }

            if (emp == null)
            {
                UpdateStatus("No employee found with ID of " + _view.Id);
                return;
            }

            Console.WriteLine("Edit: (ID: " + emp.Id + ") " + emp.FirstName);
            Console.Write("Name: ");
            var input = Console.ReadLine();

            if (input == "")
            {
                UpdateStatus("Please enter a valid name");
                return;
            }

            emp.FirstName = input;

            Console.Write("Store ID: ");
            input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result) == false)
            {
                UpdateStatus("Please enter a valid store ID");
                return;
            }

            emp.StoreId = result;

            try
            {
                _employeeRepository.Save(emp);
                _employeeRepository.Commit();
                UpdateStatus("\nRecord successfully updated!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not update Employee\n");
            }
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
                OperationFailed(ex, "Could not retrieve employees\n");
                return;
            }

            if (emps.Count <= 0)
            {
                UpdateStatus("No employees found.");
                return;
            }

            foreach (var employee in emps)
            {
                UpdateStatus("ID: " + employee.Id +
                             "\nFirst Name: " + employee.FirstName + 
                             "\nStore ID: " + employee.StoreId +
                             "\n");
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
                OperationFailed(ex, "Could not retrieve Employee");
                return;
            }

            if (emp == null)
            {
                UpdateStatus("No employee found with ID of " + _view.Id);
                return;
            }

            try
            {
                _employeeRepository.Remove(emp);
                _employeeRepository.Commit();
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not remove employee");
                return;
            }

            UpdateStatus("Employee Removed!");
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
                OperationFailed(ex, "Could not save employee\n");
                return;
            }

            UpdateStatus("New employee created with ID of " + emp.Id);
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
                    OperationFailed(ex, "Could not retrieve Employee in Get");
                    return;
                }

                if (emp == null)
                {
                    UpdateStatus("No employee found with ID of " + _view.Id);
                    return;
                }

                UpdateStatus("First Name: " + emp.FirstName);
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
                    OperationFailed(ex, "Could not retrieve Employee in by Name or Store ID");
                    return;
                }

                if (emps.Count == 0)
                {
                    UpdateStatus("No employee(s) found with name " + _view.FirstName);
                    return;
                }

                foreach (var employee in emps)
                {
                    UpdateStatus("ID: " + employee.Id +
                                 "\nName: " + employee.FirstName + 
                                 "\nStore ID: " + employee.StoreId +
                                 "\n");
                }
            }
        }
    }
}
