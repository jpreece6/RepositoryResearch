using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using RepoConsole.Events;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    /// <summary>
    /// Controls the business logic for the Employee view operations
    /// </summary>
    class EmployeePresenter : Presenter
    {
        private readonly IViewEmployee _view;
        private readonly EmployeeRepository<Employee> _employeeRepository;

        public event EventHandler<ObjectGetReturnedArgs<Employee>> OnObjectGetReturned;
        public event EventHandler<ObjectGetAllReturnedArgs<IList<Employee>>> OnObjectGetAllReturned;

        /// <summary>
        /// Create a new instance of the EmployeePresenter
        /// </summary>
        /// <param name="view">View to bind to</param>
        public EmployeePresenter(IViewEmployee view)
        {
            // Bind to all view events
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;
            _view.Edit += View_Edit;
            _view.Update += View_Update;

            // Configure a new repository
            var sessionFactManager = new SessionFactoryManager();
            SessionContext = new SessionContext(sessionFactManager);

            _employeeRepository = new EmployeeRepository<Employee>(SessionContext);
        }

        /// <summary>
        /// Called when the user wants to update a Employee entity
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event Args</param>
        private void View_Update(object sender, Events.UpdateInputArgs<Employee> e)
        {
            // Make sure we can open a session
            if (!OpenSession())
                return;

            // Ensure network state hasn't changed!
            // If it has we cannot ensure a safe update so we cannot continue the process
            if ((e.IsLocal && SessionContext.IsLocal()) ||
                (e.IsLocal == false && SessionContext.IsLocal() == false))
            {

                var isDirty = false;

                // We should at least have 1 object to update
                if (e.Entity == null)
                {
                    OperationFailed(new Exception("No object available!"),
                        "Object to update was missing from the collection!");
                    return;
                }

                if (_view.FirstName != "" || _view.FirstName != null)
                {
                    e.Entity.FirstName = _view.FirstName;
                    isDirty = true;
                }

                if (_view.StoreId != 0)
                {
                    e.Entity.StoreId = _view.StoreId;
                    isDirty = true;
                }

                // No properties we updated inform the user
                // and do not execute further
                if (isDirty == false)
                {
                    UpdateStatus("Record not updated, no values changed!");
                    return;
                }

                try
                {
                    // Update record in DB
                    _employeeRepository.Save(e.Entity);
                    _employeeRepository.Commit();

                    UpdateStatus("\nRecord successfully updated!");
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not update Employee\n");
                }
            }
            else
            {
                OperationFailed(new Exception("Network state changed mid update!"), "Could not update record!");
            }
        }

        /// <summary>
        /// Called when a user wants to edit then update a record
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
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

            // Inform the user we didn't find an entity with their criteria
            if (emp == null)
            {
                UpdateStatus("No employee found with ID of " + _view.Id);
                return;
            }

            // Package the Employee into a list so we can return it
            var output = new List<Employee>();
            output.Add(emp);

            // Return the Employee object to the view
            // update flag = true so we want to update it when we get it back
            // Connection status is recorded so we can check again on update
            OnObjectGetReturned?.Invoke(this, new ObjectGetReturnedArgs<Employee>(emp, true, SessionContext.IsLocal()));

        }

        /// <summary>
        /// Called when a user wants to retrieve all Employees
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
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

            // Inform the user we couldn't find any Employees
            if (emps.Count <= 0)
            {
                UpdateStatus("No employees found.");
                return;
            }

            // Return the list of employees to the view
            OnObjectGetAllReturned?.Invoke(this, new ObjectGetAllReturnedArgs<IList<Employee>>(emps));
        }

        /// <summary>
        /// Called when a user wants to remove an employee
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
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

            // Inform the user we couldn't find the employee
            // with their criteria, so stop removing
            if (emp == null)
            {
                UpdateStatus("No employee found with ID of " + _view.Id);
                return;
            }

            try
            {
                // Remove from the DB
                _employeeRepository.Remove(emp);
                _employeeRepository.Commit();

                UpdateStatus("Employee Removed!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not remove employee");
            }
        }

        /// <summary>
        /// Called when a user wants to add an employee to the DB
        /// </summary>
        /// <param name="sender">Even owner</param>
        /// <param name="e">Event args</param>
        private void View_Add(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            // Create a new object and assign the values
            // The input values should be checked before getting here
            var emp = new Employee();
            emp.FirstName = _view.FirstName;
            emp.StoreId = _view.StoreId;

            try
            {
                // Save to the DB
                _employeeRepository.Save(emp);
                _employeeRepository.Commit();

                UpdateStatus("New employee created with ID of " + emp.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not save employee\n");
            }
        }

        /// <summary>
        /// Called when the user wants to find one or more employees
        /// with a specified criteria
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Get(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            // Search by ID if we have one
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

                // Inform the use  we couldn't find an employee with their criteria
                if (emp == null)
                {
                    UpdateStatus("No employee found with ID of " + _view.Id);
                    return;
                }

                // Output our result and stop further execution
                UpdateStatus("First Name: " + emp.FirstName);
                return;
            }

            // Search by name or store ID if we have one
            if (_view.FirstName != null || _view.StoreId != 0)
            {
                IList<Employee> emps = new List<Employee>();

                try
                {
                    // Perform the desiered search
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

                // Inform the user we couldn't find any employees with their criteria
                if (emps.Count == 0)
                {
                    UpdateStatus("No employee(s) found with name " + _view.FirstName);
                    return;
                }

                // Return the list of employees to the view so we can display it
                OnObjectGetAllReturned?.Invoke(this, new ObjectGetAllReturnedArgs<IList<Employee>>(emps));
            }
        }
    }
}
