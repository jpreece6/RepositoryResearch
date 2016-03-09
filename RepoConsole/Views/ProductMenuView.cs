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
    /// <summary>
    /// Handles printing to the console for the product menu
    /// </summary>
    public class ProductMenuView : StateView, IViewProduct
    {
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;

        public event EventHandler<UpdateInputArgs<Product>> Update;

        public int Id { get; set; }
        public string Name { get; set; }
        public float? Price { get; set; }

        private ProductPresenter _presenter;
        private bool _exit = false;

        /// <summary>
        /// Creates a new instance of product view
        /// </summary>
        public ProductMenuView()
        {
            // Binds to the presenter events
            _presenter = new ProductPresenter(this);
            _presenter.OnUpdateStatus += Presenter_OnUpdateStatus;
            _presenter.OnOperationFail += Presenter_OnOperationFail;
            _presenter.OnObjectGetAllReturned += Presenter_OnObjectGetAllReturned;
            _presenter.OnObjectGetReturned += Presenter_OnObjectGetReturned;
        }

        private void Presenter_OnObjectGetReturned(object sender, ObjectGetReturnedArgs<Product> e)
        {
            DisplayStatus();

            // If update is true then ask for input to update the object
            if (e.Update)
            {
                Console.WriteLine("Updating (ID: " + e.Entity.Id + ") " + e.Entity.Prod_Name);
                Console.WriteLine("NOTE: Leave blank if you don't want to update a field \n");
                Get_Name(null);
                Get_Price(null);

                // Tell the presenter to update
                Update?.Invoke(this, new UpdateInputArgs<Product>(e.Entity, e.IsLocal));
            }
            else
            {
                Console.WriteLine("ID: " + e.Entity.Id +
                                  "\nName: " + e.Entity.Prod_Name +
                                  "\nPrice: " + e.Entity.Price +
                                  "\n");
            }
        }

        /// <summary>
        /// Called when an object is returned from the database
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnObjectGetAllReturned(object sender,
            ObjectGetAllReturnedArgs<IList<DataEngine.Entities.Product>> e)
        {

            DisplayStatus();
            foreach (var product in e.RecordList)
            {
                Console.WriteLine("ID: " + product.Id +
                                  "\nName: " + product.Prod_Name +
                                  "\nPrice: " + product.Price +
                                  "\n");
            }

        }

        /// <summary>
        /// Called when an operation fails in the presenter
        /// </summary>
        /// <param name="sender">event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnOperationFail(object sender, Events.OperationFailedArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
            Console.WriteLine("Error: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
        }

        /// <summary>
        /// Called when the presenter needs to inform the user of a status change
        /// or provide the user with further instructions
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnUpdateStatus(object sender, Events.StatusUpdateArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
        }

        /// <summary>
        /// Determines the users choice
        /// on the menu screen and calls that action
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
        /// Tells the presenter to add a new product
        /// </summary>
        public void Show_Add()
        {
            DisplayStatus();
            Console.WriteLine("Add New Product\n");
            Get_Name(null);
            Get_Price(Add);

            Console.ReadLine();
        }

        /// <summary>
        /// Tells the presenter to edit/update a product
        /// </summary>
        public void Show_Edit()
        {
            DisplayStatus();
            Console.WriteLine("Edit Product\n");
            Get_ID(Edit);

            Console.ReadLine();
        }

        /// <summary>
        /// Displays all possible search options to the user
        /// and asks them for the criteria input after slection
        /// </summary>
        public void Show_Get()
        {
            DisplayStatus();
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
                        DisplayStatus();
                        Console.WriteLine("Enter Product ID\n");
                        Get_ID(Get);

                        break;
                    case 2:
                        DisplayStatus();
                        Console.WriteLine("Enter Product Name\n");
                        Get_Name(Get);

                        break;
                    case 3:
                        DisplayStatus();
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

        /// <summary>
        /// Tells the presenter to return all products
        /// </summary>
        public void Show_GetAll()
        {
            DisplayStatus();
            Console.WriteLine("List all Products\n");
            GetAll?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tells the presenter to remove a product
        /// </summary>
        public void Show_Remove()
        {
            DisplayStatus();
            Console.WriteLine("Remove Product\n");
            Get_ID(Remove);

            Console.Read();
        }

        /// <summary>
        /// Prints the product menu to the screen
        /// </summary>
        public void Show()
        {
            do
            {
                DisplayStatus();
                ClearProperties(); // Reset properties
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

            if (!int.TryParse(input, out result)) return;

            Id = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Asks the user to provide an price and then
        /// executes a specified event
        /// </summary>
        /// <param name="fireEvent">Event to call after input</param>
        private void Get_Price(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Price: ");
            var input = Console.ReadLine();
            float result;

            if (!float.TryParse(input, out result)) return;

            Price = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
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

            Name = input;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Clear view properties this is so that
        /// there are no lingering values between operations
        /// </summary>
        private void ClearProperties()
        {
            Id = 0;
            Name = null;
            Price = null;
        }
    }
}
