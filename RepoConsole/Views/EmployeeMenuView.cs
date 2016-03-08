using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using DataEngine.Entities;
using RepoConsole.Events;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    internal class EmployeeMenuView : StateView, IViewEmployee
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> Remove;
        public event EventHandler<EventArgs> GetAll;

        public event EventHandler<UpdateInputArgs<IList<DataEngine.Entities.Employee>>> Update; 

        public int Id { get; set; }
        public string FirstName { get; set; }
        public int StoreId { get; set; }

        private EmployeePresenter _presenter;
        private bool _exit = false;

        public EmployeeMenuView()
        {
            _presenter = new EmployeePresenter(this);
            _presenter.OnUpdateStatus += Presenter_OnUpdateStatus;
            _presenter.OnOperationFail += Presenter_OnOperationFail;
            _presenter.OnObjectReturned += Presenter_OnObjectReturned;
        }

        private void Presenter_OnObjectReturned(object sender, ObjectReturnedArgs<IList<DataEngine.Entities.Employee>> e)
        {
            if (e.Update)
            {
                DisplayStatus();
                Console.WriteLine("Update (ID: " + e.RecordList[0].Id + ") " + e.RecordList[0].FirstName);
                Console.WriteLine("NOTE: Leave blank if you don't want to update a field \n");
                Get_Name(null);
                Get_StoreID(null);
                Update?.Invoke(this, new UpdateInputArgs<IList<Employee>>(e.RecordList, e.IsLocal));
            }
            else
            {
                DisplayStatus();
                foreach (var employee in e.RecordList)
                {
                    Console.WriteLine("ID: " + employee.Id + 
                                      "\nName: " + employee.FirstName +
                                      "\nStore ID " + employee.StoreId +
                                      "\n");
                }
            }
        }

        private void Presenter_OnOperationFail(object sender, Events.OperationFailedArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
            Console.WriteLine("Error: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
        }

        private void Presenter_OnUpdateStatus(object sender, Events.StatusUpdateArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
        }

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

        public void Show_GetAll()
        {
            DisplayStatus();
            GetAll?.Invoke(this, EventArgs.Empty);
        }

        public void Show_Remove()
        {
            DisplayStatus();
            Console.WriteLine("Remove Employee\n");
            Get_ID(Remove);

            Console.ReadLine();
        }

        public void Show_Edit()
        {
            DisplayStatus();
            Console.WriteLine("Edit Employee\n");
            Get_ID(Edit);

            Console.ReadLine();
        }

        public void Show_Get()
        {
            DisplayStatus();
            Console.WriteLine("Employee Info\n");
            Console.WriteLine("1: Search by ID");
            Console.WriteLine("2: Search by Name");
            Console.WriteLine("3: Search by Store");
            Console.WriteLine("4: Get All");
            Console.WriteLine("5: Back");
            Console.Write("\nChoice: ");

            var input = Console.ReadLine();
            int result;

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
                        GetAll(this, EventArgs.Empty);
                        break;
                    case 5:
                        break;
                }
            }
            Console.ReadLine();
        }

        public void Show_Add()
        {
            DisplayStatus();
            Console.WriteLine("Add new Employee\n");
            Get_Name(null);
            Get_StoreID(Add);

            Console.ReadLine();
        }

        public void Show()
        {
            do
            {
                DisplayStatus();
                Console.WriteLine("Employee Menu\n");
                Console.WriteLine("1: Add new employee");
                Console.WriteLine("2: Edit employee");
                Console.WriteLine("3: Find Employee(s)");
                Console.WriteLine("4: Remove employee");
                Console.WriteLine("5: Back");
                Console.Write("\nChoice: ");
                WaitForInput();
            } while (_exit == false);
        }

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
    }
}