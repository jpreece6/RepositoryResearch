using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using RepoConsole.Events;
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

        public event EventHandler<UpdateInputArgs<IList<DataEngine.Entities.Product>>> Update;

        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        private ProductPresenter _presenter;
        private bool _exit = false;

        public ProductMenuView()
        {
            _presenter = new ProductPresenter(this);
            _presenter.OnUpdateStatus += Presenter_OnUpdateStatus;
            _presenter.OnOperationFail += Presenter_OnOperationFail;
            _presenter.OnObjectReturned += Presenter_OnObjectReturned;
        }

        private void Presenter_OnObjectReturned(object sender, ObjectReturnedArgs<IList<DataEngine.Entities.Product>> e)
        {
            if (e.Update)
            {
                Console.WriteLine("Updating (ID: " + e.RecordList[0].Id + ") " + e.RecordList[0].Prod_Name + "\n");
                Get_Name(null);
                Get_Price(null);
                Update?.Invoke(this, new UpdateInputArgs<IList<Product>>(e.RecordList));
            }
            else
            {
                foreach (var product in e.RecordList)
                {
                    Console.WriteLine("ID: " + product.Id +
                                      "\nName: " + product.Prod_Name +
                                      "\nPrice: " + product.Price +
                                      "\n");
                }
            }
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

        public void Show_Add()
        {
            Console.Clear();
            Console.WriteLine("Add New Product\n");
            Get_Name(null);
            Get_Price(Add);

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
                        Console.WriteLine("Enter Product Name\n");
                        Get_Name(Get);

                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Enter Product Price\n");
                        Get_Price(Get);
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
            GetAll?.Invoke(this, EventArgs.Empty);
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

        public void GetEditInput()
        {
            
        }

        private void Get_ID(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("ID: ");
            var input = Console.ReadLine();
            int result;

            if (!int.TryParse(input, out result)) return;

            Id = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Get_Price(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Price: ");
            var input = Console.ReadLine();
            float result;

            if (!float.TryParse(input, out result)) return;

            Price = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Get_Name(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Name: ");
            var input = Console.ReadLine();

            Name = input;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
