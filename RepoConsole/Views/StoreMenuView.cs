using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataEngine.Entities;
using RepoConsole.Events;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    /// <summary>
    /// Handles printing to the console for the store menu
    /// </summary>
    public class StoreMenuView : StateView, IViewStore
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;

        public event EventHandler<UpdateInputArgs<Store>> Update;

        public int Id { get; set; }
        public string StoreName { get; set; }

        private bool _exit = false;
        private StorePresenter _presenter;

        /// <summary>
        /// Creates a new instance of store menu
        /// </summary>
        public StoreMenuView()
        {
            // Bind to all presenter events
            _presenter = new StorePresenter(this);
            _presenter.OnUpdateStatus += Presenter_OnUpdateStatus;
            _presenter.OnOperationFail += Presenter_OnOperationFail;
            _presenter.OnObjectGetAllReturned += Presenter_OnObjectGetAllReturned;
            _presenter.OnObjectGetReturned += Presenter_OnObjectGetReturned;
        }

        private void Presenter_OnObjectGetReturned(object sender, ObjectGetReturnedArgs<Store> e)
        {
            DisplayStatus();

            // If we're updating then ask the user for input
            if (e.Update)
            {
                Console.WriteLine("Update (ID: " + e.Entity.Id + ") - " + e.Entity.StoreName);
                Console.WriteLine("NOTE: Leave blank if you don't want to update a field \n");
                Get_Name(null);

                // Tell the presenter to update
                Update?.Invoke(this, new UpdateInputArgs<Store>(e.Entity, e.IsLocal));
            }
            else
            {
                Console.WriteLine("ID: " + e.Entity.Id +
                                    "\nName: " + e.Entity.StoreName +
                                    "\n");
            }
        }

        /// <summary>
        /// Called when an object is returned from the DB
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnObjectGetAllReturned(object sender, Events.ObjectGetAllReturnedArgs<IList<Store>> e)
        {
            DisplayStatus();
            foreach (var store in e.RecordList)
            {
                Console.WriteLine("ID: " + store.Id +
                                    "\nName: " + store.StoreName +
                                    "\n");
            }
        }

        /// <summary>
        /// Called when the presenter fails an operation
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnOperationFail(object sender, Events.OperationFailedArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
            Console.WriteLine("\nError: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
        }

        /// <summary>
        /// Called when the presenter wants to inform the user of a state change or
        /// provides the user with further instrction
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void Presenter_OnUpdateStatus(object sender, Events.StatusUpdateArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
        }

        /// <summary>
        /// Prints the store menu to console
        /// </summary>
        public void Show()
        {
            do
            {
                DisplayStatus();
                ClearProperties(); // Reset properties
                Console.WriteLine("Store Menu\n");
                Console.WriteLine("1: Add new store");
                Console.WriteLine("2: Edit store");
                Console.WriteLine("3: Find store(s)");
                Console.WriteLine("4: Remove store");
                Console.WriteLine("5: Back");
                Console.Write("\nChoice: ");
                WaitForInput();
            } while (_exit == false);
        }

        /// <summary>
        /// Tells the presenter to add a new store
        /// </summary>
        public void Show_Add()
        {
            DisplayStatus();
            Console.WriteLine("Add New Store\n");
            Get_Name(Add);

            Console.ReadLine();
        }

        /// <summary>
        /// Displays all search options available
        /// and asks the user to provide criteria after selection
        /// </summary>
        public void Show_Get()
        {
            DisplayStatus();
            Console.WriteLine("Find Store\n");
            Console.WriteLine("1: Search by ID");
            Console.WriteLine("2: Search by Name");
            Console.WriteLine("3: Get All");
            Console.WriteLine("4: Back");
            Console.Write("\nChoice: ");

            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                switch (result)
                {
                    case 1:
                        DisplayStatus();
                        Console.WriteLine("Filter by Id\n");
                        Get_ID(Get);
                        break;
                    case 2:
                        DisplayStatus();
                        Console.WriteLine("Filter by Name\n");
                        Get_Name(Get);
                        break;
                    case 3:
                        Show_GetAll();
                        break;
                    case 4:
                        break;
                }
            }

            if (result != 4) Console.ReadLine();
        }

        /// <summary>
        /// Tells the presenter to return all stores
        /// </summary>
        public void Show_GetAll()
        {
            DisplayStatus();
            GetAll?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tells the presenter to remove a store
        /// </summary>
        public void Show_Remove()
        {
            DisplayStatus();
            Console.WriteLine("Remove Store\n");
            Get_ID(Remove);

            Console.ReadLine();
        }

        /// <summary>
        /// Tells the presenter to edit/update a store
        /// </summary>
        public void Show_Edit()
        {
            DisplayStatus();
            Console.WriteLine("Edit Store\n");
            Get_ID(Edit);

            Console.ReadLine();
        }

        /// <summary>
        /// Determines the users choice on the main menu
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
                        //Action<EventHandler<EventArgs>> getAction = x => x.Invoke(this, EventArgs.Empty);
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

            if (int.TryParse(input, out result))
            {
                Id = result;
                fireEvent?.Invoke(this, EventArgs.Empty);
            }
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

            if (input == "") return;

            StoreName = input;
            fireEvent?.Invoke(this,EventArgs.Empty); 
        }

        /// <summary>
        /// Clear view properties this is so that
        /// there are no lingering values between operations
        /// </summary>
        private void ClearProperties()
        {
            Id = 0;
            StoreName = null;
        }
    }
}
