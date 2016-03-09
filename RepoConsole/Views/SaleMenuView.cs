using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using RepoConsole.Events;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    /// <summary>
    /// Handles printing to the console for the sale menu
    /// </summary>
    public class SaleMenuView : StateView, IViewSale
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;

        public event EventHandler<UpdateInputArgs<Sale>> Update; 

        public int Id { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public DateTime? Timestamp { get; set; }

        private SalePresenter _presenter;
        private bool _exit = false;

        /// <summary>
        /// Creates a new instance of sale menu
        /// </summary>
        public SaleMenuView()
        {
            // Bind to all presenter events
            _presenter = new SalePresenter(this);
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
            Console.WriteLine(e.Status);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void Presenter_OnObjectGetReturned(object sender, ObjectGetReturnedArgs<Sale> e)
        {
            DisplayStatus();

            // If we need to update we ask the user to provide input
            if (e.Update)
            {
                Console.WriteLine("Update (ID: " + e.Entity.Id);
                Console.WriteLine("NOTE: Leave blank if you don't want to update a field \n");
                Get_StoreID(null);
                Get_ProductID(null);
                Get_Timestamp(null);

                // Tell the presenter to update
                Update?.Invoke(this, new UpdateInputArgs<Sale>(e.Entity, e.IsLocal));
            }
            else
            {
                Console.WriteLine("ID: " + e.Entity.Id +
                                  "\nStore ID: " + e.Entity.StoreId +
                                  "\nProduct ID: " + e.Entity.ProductId +
                                  "\nTimestamp: " + e.Entity.Timestamp +
                                  "\n");
            }
        }

        /// <summary>
        /// Called when an object is returned from the DB
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnObjectGetAllReturned(object sender, ObjectGetAllReturnedArgs<IList<DataEngine.Entities.Sale>> e)
        {

            DisplayStatus();
            foreach (var sale in e.RecordList)
            {
                Console.WriteLine("ID: " + sale.Id +
                                  "\nStore ID: " + sale.StoreId +
                                  "\nProduct ID: " + sale.ProductId +
                                  "\nTimestamp: " + sale.Timestamp +
                                  "\n");
            }
        }

        /// <summary>
        /// Called when an operation fails in the presenter
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnOperationFail(object sender, Events.OperationFailedArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
            Console.WriteLine("Error: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
            Console.Write("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Called when the presenter needs to inform the user of a state change
        /// or provide further instrcution
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnUpdateStatus(object sender, Events.StatusUpdateArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
        }

        /// <summary>
        /// Prints the sale menu to the console
        /// </summary>
        public void Show()
        {
            do
            {
                DisplayStatus();
                ClearProperties(); // Reset properties
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

        /// <summary>
        /// Tells the presenter to add a new sale
        /// </summary>
        public void Show_Add()
        {
            DisplayStatus();
            Console.WriteLine("Add Sale\n");
            Get_StoreID(null);
            Get_ProductID(Add);

            Console.ReadLine();
        }

        /// <summary>
        /// Tells the presenter to edit/update a sale
        /// </summary>
        public void Show_Edit()
        {
            DisplayStatus();
            Console.WriteLine("Edit Store\n");
            Get_ID(Edit);

            Console.ReadLine();
        }

        /// <summary>
        /// Prints all search options to the user
        /// and asks for criteria for the selected search option
        /// </summary>
        public void Show_Get()
        {
            DisplayStatus();
            Console.WriteLine("Find a Sale\n");
            Console.WriteLine("1: Search by ID");
            Console.WriteLine("2: Search by Store ID");
            Console.WriteLine("3: Search by Product ID");
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
                        Console.WriteLine("Enter Sale ID\n");
                        Get_ID(Get);
                        break;
                    case 2:
                        DisplayStatus();
                        Console.WriteLine("Enter Store ID\n");
                        Get_StoreID(Get);
                        break;
                    case 3:
                        DisplayStatus();
                        Console.WriteLine("Enter Product ID\n");
                        Get_ProductID(Get);
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

        /// <summary>
        /// Tells the presenter to return all sales
        /// </summary>
        public void Show_GetAll()
        {
            DisplayStatus();
            GetAll?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tells the presenter to remove a sale
        /// </summary>
        public void Show_Remove()
        {
            DisplayStatus();
            Console.WriteLine("Remove Sale\n");
            Get_ID(Remove);

            Console.ReadLine();
        }

        /// <summary>
        /// Determines the users choice on the menu
        /// and shows that operation
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
        /// Asks the user to provide an store ID and then
        /// executes a specified event
        /// </summary>
        /// <param name="fireEvent">Event to call after input</param>
        private void Get_StoreID(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Store ID: ");
            var input = Console.ReadLine();
            int result;

            if (!int.TryParse(input, out result)) return;

            StoreId = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Asks the user to provide an product ID and then
        /// executes a specified event
        /// </summary>
        /// <param name="fireEvent">Event to call after input</param>
        private void Get_ProductID(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Product ID: ");
            var input = Console.ReadLine();
            int result;

            if (!int.TryParse(input, out result)) return;

            ProductId = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Asks the user to provide an timestamp and then
        /// executes a specified event
        /// </summary>
        /// <param name="fireEvent">Event to call after input</param>
        private void Get_Timestamp(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Update timestamp? [yes or no]:");
            var input = Console.ReadLine();

            if (input != null && input.ToLower().Trim().Equals("yes"))
            {
                Timestamp = DateTime.Now;
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
            ProductId = 0;
        }
    }
}
