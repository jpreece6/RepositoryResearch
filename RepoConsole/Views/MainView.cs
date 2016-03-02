using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    public class MainView : IView
    {
        private Presenter.IPresenter _presenter;
        private bool _exit = false;

        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;

        public MainView()
        {
            _presenter = new MainMenuPresenter(this);
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
                        var stor = new StoreMenuView();
                        stor.Show();
                        break;
                    case 2:
                        var emp = new EmployeeMenuView();
                        emp.Show();
                        break;
                    case 3:
                        var prod = new ProductMenuView();
                        prod.Show();
                        break;
                    case 4:

                        break;
                    case 5:
                        _exit = true;
                        break;
                }
            }
        }

        public void Show()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Main Menu\n");
                Console.WriteLine("1: Stores");
                Console.WriteLine("2: Employees");
                Console.WriteLine("3: Products");
                Console.WriteLine("4: ...");
                Console.WriteLine("5: Exit");
                Console.Write("\nChoice: ");
                WaitForInput();
            } while(_exit == false);
        }

        public void Show_Add()
        {
            throw new NotImplementedException();
        }

        public void Show_Get()
        {
            throw new NotImplementedException();
        }

        public void Show_GetAll()
        {
            throw new NotImplementedException();
        }

        public void Show_Remove()
        {
            throw new NotImplementedException();
        }
    }
}
