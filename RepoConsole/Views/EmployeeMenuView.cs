using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using DataEngine.Entities;
using RepoConsole.Events;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    /// <summary>
    /// Handles printing to the console screen for the employee menu
    /// </summary>
    internal class EmployeeMenuView : StateView, IViewEmployee
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> Remove;
        public event EventHandler<EventArgs> GetAll;

        public event EventHandler<UpdateInputArgs<DataEngine.Entities.Employee>> Update; 

        public int Id { get; set; }
        public string FirstName { get; set; }
        public int StoreId { get; set; }

        private EmployeePresenter _presenter;
        private bool _exit = false;

        /// <summary>
        /// Create a new instance of the employee view
        /// </summary>
        public EmployeeMenuView()
        {
            // Bind to all presenter events
            _presenter = new EmployeePresenter(this);
            _presenter.OnUpdateStatus += Presenter_OnUpdateStatus;
            _presenter.OnOperationFail += Presenter_OnOperationFail;
            _presenter.OnObjectGetAllReturned += Presenter_OnObjectGetAllReturned;
            _presenter.OnObjectGetReturned += Presenter_OnObjectGetReturned;
            _presenter.OnStateChange += Presenter_OnStateChange;
        }

        private void Presenter_OnStateChange(object sender, StateChangeArgs e)
        {
            Console.Clear();
            DisplayStatus();
            Console.WriteLine($"{e.Status}\nPress any key to continue...");
            Console.ReadKey();
        }

        private void Presenter_OnObjectGetReturned(object sender, ObjectGetReturnedArgs<Employee> e)
        {
            DisplayStatus();

            // If we're updating then we need to request user input
            if (e.Update)
            {
                Console.WriteLine($"Update (ID: {e.Entity.Id}) {e.Entity.FirstName}");
                Console.WriteLine("NOTE: Leave blank if you don't want to update a field \n");
                Get_Name(null);
                Get_StoreID(null);
                Update?.Invoke(this, new UpdateInputArgs<Employee>(e.Entity, e.IsLocal));
            }
            else
            {
                Console.WriteLine($"ID: {e.Entity.Id}" +
                                  $"\nName: {e.Entity.FirstName}" +
                                  $"\nStore ID: {e.Entity.StoreId}\n");
            }
        }

        /// <summary>
        /// Called when an object was returned from the DB
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnObjectGetAllReturned(object sender, ObjectGetAllReturnedArgs<IList<DataEngine.Entities.Employee>> e)
        {
            DisplayStatus();
            var index = 0;

            // Loop through all returned employees and print them
            foreach (var employee in e.RecordList)
            {
                Console.WriteLine($"ID: {employee.Id}" +
                                  $"\nName: {employee.FirstName}" +
                                  $"\nStore ID: {employee.StoreId}\n");
                if (++index == 20)
                {
                    Console.Write("More...");
                    index = 0;
                    Console.ReadKey();
                }

            }
            Console.Write("Done...");
        }

        /// <summary>
        /// Called when a operation fails in the presenter
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">event args</param>
        private void Presenter_OnOperationFail(object sender, Events.OperationFailedArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
            Console.WriteLine($"Error: {(e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message)}");
            Console.Write("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Called when the presenter needs to inform the user of a status change
        /// or provide the user with further instruction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Presenter_OnUpdateStatus(object sender, Events.StatusUpdateArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
        }

        /// <summary>
        /// Main input logic this determines what choice the
        /// user made on the menu
        /// </summary>
        public void WaitForInput()
        {
            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                switch (result)
                {
                    case 1:
                        Show_Add();
                        break;
                    case 2:
                        Show_Edit();
                        break;
                    case 3:
                        Show_Get();
                        break;
                    case 4:
                        Show_Remove();
                        break;
                    case 5:
                        _exit = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Tells the presenter to get all employee's
        /// from the DB using the get all event
        /// </summary>
        public void Show_GetAll()
        {
            DisplayStatus();
            GetAll?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tells the presenter to remove an employee
        /// </summary>
        public void Show_Remove()
        {
            DisplayStatus();
            Console.WriteLine("Remove Employee\n");
            Get_ID(Remove);

            Console.ReadKey();
        }

        /// <summary>
        /// Tells the presenter the user wants to edit/update
        /// an employee
        /// </summary>
        public void Show_Edit()
        {
            DisplayStatus();
            Console.WriteLine("Edit Employee\n");
            Get_ID(Edit);

            Console.ReadKey();
        }

        /// <summary>
        /// Gives the user an option to search for an employee
        /// with a number of criteria. Once the user selects 
        /// one it asks for that criteria to be inputted
        /// </summary>
        public void Show_Get()
        {
            DisplayStatus();
            Console.WriteLine("Find Employee(s)\n");
            Console.WriteLine("1: Search by ID");
            Console.WriteLine("2: Search by Name");
            Console.WriteLine("3: Search by Store");
            Console.WriteLine("4: Get All");
            Console.WriteLine("5: Back");
            Console.Write("\nChoice: ");

            var input = Console.ReadLine();
            int result;

            // Determine choice
            if (int.TryParse(input, out result))
            {
                switch (result)
                {
                    case 1:
                        DisplayStatus();
                        Console.WriteLine("Enter Employee ID\n");
                        Get_ID(Get);
                        break;
                    case 2:
                        DisplayStatus();
                        Console.WriteLine("Enter Employee Name\n");
                        Get_Name(Get);
                        break;
                    case 3:
                        DisplayStatus();
                        Console.WriteLine("Enter Store ID\n");
                        Get_StoreID(Get);
                        break;
                    case 4:
                        DisplayStatus();
                        GetAll?.Invoke(this, EventArgs.Empty);
                        break;
                    case 5:
                        break;
                }
            }

            if (result != 5) Console.ReadKey();
        }

        /// <summary>
        /// Tells the presenter to add a new employee
        /// </summary>
        public void Show_Add()
        {
            DisplayStatus();
            Console.WriteLine("Add new Employee\n");
            Get_Name(null);
            Get_StoreID(Add);

            Console.ReadKey();
        }

        /// <summary>
        /// Shows the main employee menu to the user.
        /// Keeps displaying until _exit = true
        /// </summary>
        public void Show()
        {
            do
            {
                DisplayStatus();
                ClearProperties(); // Reset properties
                Console.WriteLine("Employee Menu\n");
                Console.WriteLine("1: Add new Employee");
                Console.WriteLine("2: Edit Employee");
                Console.WriteLine("3: Find Employee(s)");
                Console.WriteLine("4: Remove Employee");
                Console.WriteLine("5: Back");
                Console.Write("\nChoice: ");
                WaitForInput();
            } while (_exit == false);
        }

        /// <summary>
        /// Asks the user to provide an ID and then
        /// executes a specified event
        /// </summary>
        /// <param name="fireEvent">Event to call after input</param>
        private void Get_ID(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("ID: ");
            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                Id = result;
                fireEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Asks the user to provide an store ID and then
        /// executes a specified event
        /// </summary>
        /// <param name="fireEvent">Event to call after input</param>
        private void Get_StoreID(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Store ID: ");
            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                StoreId = result;
                fireEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Asks the user to provide an name and then
        /// executes a specified event
        /// </summary>
        /// <param name="fireEvent">Event to call after input</param>
        private void Get_Name(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Name: ");
            var input = Console.ReadLine();

            if (input != "")
            {
                FirstName = input;
                fireEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Clear view properties this is so that
        /// there are no lingering values between operations
        /// </summary>
        private void ClearProperties()
        {
            Id = 0;
            StoreId = 0;
            FirstName = null;
        }
    }
}