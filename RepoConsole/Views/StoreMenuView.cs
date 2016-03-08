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
    public class StoreMenuView : StateView, IViewStore
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;

        public event EventHandler<UpdateInputArgs<IList<DataEngine.Entities.Store>>> Update;

        public int Id { get; set; }
        public string StoreName { get; set; }

        private bool _exit = false;
        private StorePresenter _presenter;

        public StoreMenuView()
        {
            _presenter = new StorePresenter(this);
            _presenter.OnUpdateStatus += Presenter_OnUpdateStatus;
            _presenter.OnOperationFail += Presenter_OnOperationFail;
            _presenter.OnObjectReturned += Presenter_OnObjectReturned;
        }

        private void Presenter_OnObjectReturned(object sender, Events.ObjectReturnedArgs<IList<DataEngine.Entities.Store>> e)
        {
            if (e.Update)
            {
                DisplayStatus();
                Console.WriteLine("Update (ID: " + e.RecordList[0].Id + ") - " + e.RecordList[0].StoreName);
                Console.WriteLine("NOTE: Leave blank if you don't want to update a field \n");
                Get_Name(null);
                Update?.Invoke(this, new UpdateInputArgs<IList<Store>>(e.RecordList, e.IsLocal));
            }
            else
            {
                DisplayStatus();
                foreach (var store in e.RecordList)
                {
                    Console.WriteLine("ID: " + store.Id +
                                      "\nName: " + store.StoreName +
                                      "\n");
                }
            }
        }

        private void Presenter_OnOperationFail(object sender, Events.OperationFailedArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
            Console.WriteLine("\nError: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
        }

        private void Presenter_OnUpdateStatus(object sender, Events.StatusUpdateArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
        }

        public void Show()
        {
            do
            {
                DisplayStatus();
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

        public void Show_Add()
        {
            DisplayStatus();
            Console.WriteLine("Add New Store\n");
            Get_Name(Add);

            Console.ReadLine();
        }

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

        public void Show_GetAll()
        {
            DisplayStatus();
            GetAll?.Invoke(this, EventArgs.Empty);
        }

        public void Show_Remove()
        {
            DisplayStatus();
            Console.WriteLine("Remove Store\n");
            Get_ID(Remove);

            Console.ReadLine();
        }

        public void Show_Edit()
        {
            DisplayStatus();
            Console.WriteLine("Edit Store\n");
            Get_ID(Edit);

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

        private void Get_Name(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Name: ");
            var input = Console.ReadLine();

            if (input == "") return;

            StoreName = input;
            fireEvent?.Invoke(this,EventArgs.Empty); 
        }
    }
}
