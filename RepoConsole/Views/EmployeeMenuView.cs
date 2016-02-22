using System;
using System.Runtime.InteropServices;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    internal class EmployeeMenuView : IViewEmployee
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> Remove;
        public event EventHandler<EventArgs> GetAll;

        public int Id { get; set; }
        public string FirstName { get; set; }

        private Presenter.IPresenter _presenter;
        private bool _exit = false;

        public EmployeeMenuView()
        {
            _presenter = new EmployeePresenter(this);
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
                        Show_Get();
                        break;
                    case 3:
                        Show_GetAll();
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
            Console.ReadLine();
        }

        public void Show_Remove()
        {
            Console.Clear();
            Console.WriteLine("Remove Employee");
            Console.Write("ID: ");

            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                Id = result;
                Remove(this, EventArgs.Empty);
            }
            Console.ReadLine();
        }

        public void Show_Get()
        {
            Console.Clear();
            Console.WriteLine("Employee Info\n");
            Console.WriteLine("1: Search by ID");
            Console.WriteLine("2: Search by Name");
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
                        Console.Write("ID: ");

                        input = Console.ReadLine();
                        if (int.TryParse(input, out result))
                        {
                            Id = result;
                            Get(this, EventArgs.Empty);
                        }
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Enter Employee Name\n");
                        Console.Write("Name: ");
                        FirstName = Console.ReadLine();
                        Get(this, EventArgs.Empty);
                        break;
                }
            }
        }

        public void Show_Add()
        {
            Console.Clear();
            Console.WriteLine("Add new Employee\n");
            Console.Write("Name: ");
            FirstName = Console.ReadLine();
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
                Console.WriteLine("2: Get employee (filtered)");
                Console.WriteLine("3: Get All");
                Console.WriteLine("4: Remove employee");
                Console.WriteLine("5: Back");
                Console.Write("\nChoice: ");
                WaitForInput();
            } while (_exit == false);
        }
    }
}