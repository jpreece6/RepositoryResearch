using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    class ProductMenuView : IViewProduct
    {
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        private Presenter.IPresenter _presenter;
        private bool _exit = false;

        public ProductMenuView()
        {
            _presenter = new ProductPresenter(this);
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

        public void Show_Add()
        {
            Console.Clear();
            Console.WriteLine("Add New Product\n");
            Console.Write("Name: ");
            Name = Console.ReadLine();
            Console.Write("Price: ");

            var input = Console.ReadLine();
            float result;

            if (float.TryParse(input, out result))
            {
                Price = result;
            }

            if (Add != null) Add(this, EventArgs.Empty);

            Console.ReadLine();
        }

        public void Show_Edit()
        {
            Console.Clear();
            Console.WriteLine("Edit Product\n");
            Get_ID(Edit);

            Console.ReadLine();
        }

        public void Show_Get()
        {
            Console.Clear();
            Console.WriteLine("Find Product(s)\n");
            Console.WriteLine("1: Search by ID");
            Console.WriteLine("2: Search by Name");
            Console.WriteLine("3: Search by Price");
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
                        Console.WriteLine("Enter Product ID\n");
                        Get_ID(Get);
                        break;
                    case 2:
                        Console.Clear();
                        Console.Write("Name: ");
                        Name = Console.ReadLine();
                        Get(this, EventArgs.Empty);

                        break;
                    case 3:
                        Console.Clear();
                        Console.Write("Price: ");
                        input = Console.ReadLine();
                        float dResult;

                        if (float.TryParse(input, out dResult))
                        {
                            Price = dResult;
                            Get(this,EventArgs.Empty);
                        }
                        break;
                    case 4:
                        Show_GetAll();
                        break;
                    case 5:
                        break;
                }
            }

            Console.Read();
        }

        public void Show_GetAll()
        {
            Console.Clear();
            Console.WriteLine("List all Products\n");
            GetAll(this, EventArgs.Empty);
        }

        public void Show_Remove()
        {
            Console.Clear();
            Console.WriteLine("Remove Employee\n");
            Get_ID(Remove);

            Console.Read();
        }

        public void Show()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Products Menu\n");
                Console.WriteLine("1: Add new product");
                Console.WriteLine("2: Edit product");
                Console.WriteLine("3: Find Product(s)");
                Console.WriteLine("4: Remove product");
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
