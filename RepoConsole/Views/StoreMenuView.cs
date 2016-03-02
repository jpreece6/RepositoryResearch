using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    public class StoreMenuView : IViewStore
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;

        public int Id { get; set; }
        public string StoreName { get; set; }

        private bool _exit = false;
        private StorePresenter _presenter;

        public StoreMenuView()
        {
            _presenter = new StorePresenter(this);
        }

        public void Show()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Store Menu\n");
                Console.WriteLine("1: Add new store");
                Console.WriteLine("2: Find store(s)");
                Console.WriteLine("3: Remove store");
                Console.WriteLine("4: Back");
                Console.Write("\nChoice: ");
                WaitForInput();
            } while (_exit == false);
        }

        public void Show_Add()
        {
            Console.Clear();
            Console.WriteLine("Add New Store\n");
            Console.Write("Name: ");
            StoreName = Console.ReadLine();
            if (StoreName != null) Add(this, EventArgs.Empty);

            Console.ReadLine();
        }

        public void Show_Get()
        {
            Console.Clear();
            Console.WriteLine("Find Store\n");
            Console.WriteLine("1: Search by ID");
            Console.WriteLine("2: Search by Name");
            Console.WriteLine("3: Get All");
            Console.Write("\nChoice: ");

            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                switch (result)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Filter by Id\n");
                        Console.Write("Id: ");
                        input = Console.ReadLine();

                        if (int.TryParse(input, out result))
                        {
                            Id = result;
                            Get(this, EventArgs.Empty);
                        } 
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Filter by Name\n");
                        Console.Write("Name: ");
                        StoreName = Console.ReadLine();
                        Get(this, EventArgs.Empty);
                        break;
                    case 3:
                        Show_GetAll();
                        break;
                }
            }
            Console.ReadLine();
        }

        public void Show_GetAll()
        {
            Console.Clear();
            GetAll(this, EventArgs.Empty);
        }

        public void Show_Remove()
        {
            Console.Clear();
            Console.WriteLine("Remove Store\n");
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
                        Show_Remove();
                        break;
                    case 4:
                        _exit = true;
                        break;
                }
            }
        }
    }
}
