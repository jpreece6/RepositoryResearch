using System;
using System.Globalization;
using System.Runtime.InteropServices;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    internal class EmployeeMenuView : IViewEmployee
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> Remove;
        public event EventHandler<EventArgs> GetAll;

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
        }

        private void Presenter_OnOperationFail(object sender, Events.OperationFailedArgs e)
        {
            Console.WriteLine(e.Status);
            Console.WriteLine("Error: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
        }

        private void Presenter_OnUpdateStatus(object sender, Events.StatusUpdateArgs e)
        {
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
            Console.Clear();
            GetAll(this, EventArgs.Empty);
        }

        public void Show_Remove()
        {
            Console.Clear();
            Console.WriteLine("Remove Employee\n");
            Get_ID(Remove);

            Console.ReadLine();
        }

        public void Show_Edit()
        {
            Console.Clear();
            Console.WriteLine("Edit Employee\n");
            Get_ID(Edit);

            Console.ReadLine();
        }

        public void Show_Get()
        {
            Console.Clear();
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
                        Console.Clear();
                        Console.WriteLine("Enter Employee ID\n");
                        Get_ID(Get);
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Enter Employee Name\n");
                        Console.Write("Name: ");
                        FirstName = Console.ReadLine();
                        Get(this, EventArgs.Empty);
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Enter Store ID\n");
                        Console.Write("Store ID: ");

                        input = Console.ReadLine();
                        if (int.TryParse(input, out result))
                        {
                            StoreId = result;
                            Get(this, EventArgs.Empty);
                        }
                        break;
                    case 4:
                        Console.Clear();
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
            Console.Clear();
            Console.WriteLine("Add new Employee\n");
            Console.Write("Name: ");
            FirstName = Console.ReadLine();

            Console.Write("Store ID: ");
            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                StoreId = result;
            }

            if (Add != null) Add(this, EventArgs.Empty);
            

            Console.ReadLine();
        }

        public void Show()
        {
            do
            {
                Console.Clear();
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
                if (fireEvent == null) return;
                fireEvent(this, EventArgs.Empty);
            }
        }
    }
}