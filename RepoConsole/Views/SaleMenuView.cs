using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    public class SaleMenuView : IViewSale
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;

        public int Id { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }

        private Presenter.IPresenter _presenter;
        private bool _exit = false;

        public SaleMenuView()
        {
            _presenter = new SalePresenter(this);
        }

        public void Show()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Sales Menu\n");
                Console.WriteLine("1: Add Sale");
                Console.WriteLine("2: Edit Sale");
                Console.WriteLine("3: Find Sale(s)");
                Console.WriteLine("4: Remove Sale");
                Console.WriteLine("5: Back");
                Console.Write("\nChoice: ");
                WaitForInput();

            } while (_exit == false);
        }

        public void Show_Add()
        {
            Console.Clear();
            Console.WriteLine("Add Sale\n");
            Console.Write("Store ID: ");

            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                StoreId = result;
                Console.Write("Product ID: ");
                input = Console.ReadLine();

                if (int.TryParse(input, out result))
                {
                    ProductId = result;
                    Add(this, EventArgs.Empty);
                }
            }

            Console.ReadLine();
        }

        public void Show_Edit()
        {
            Console.Clear();
            Console.WriteLine("Edit Store\n");
            Get_ID(Edit);
        }

        public void Show_Get()
        {
            Console.Clear();
            Console.WriteLine("Find a Sale\n");
            Console.WriteLine("1: Search by ID");
            Console.WriteLine("2: Search by Store ID");
            Console.WriteLine("3: Search by Prodict ID");
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
                        Console.WriteLine("Enter Sale ID\n");
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
                        Console.WriteLine("Enter Store ID\n");
                        Console.Write("ID: ");
                        input = Console.ReadLine();
                        if (int.TryParse(input, out result))
                        {
                            StoreId = result;
                            Get(this, EventArgs.Empty);
                        }
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Enter Product ID\n");
                        Console.Write("ID: ");
                        input = Console.ReadLine();
                        if (int.TryParse(input, out result))
                        {
                            ProductId = result;
                            Get(this, EventArgs.Empty);
                        }
                        break;
                    case 4:
                        Show_GetAll();
                        break;
                    case 5:
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
            Console.WriteLine("Remove Sale\n");
            Console.Write("Sale ID: ");

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

        private void Get_ID(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("ID: ");
            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                Id = result;
                fireEvent(this, EventArgs.Empty);
            }
        }
    }
}
