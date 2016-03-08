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
    public class SaleMenuView : StateView, IViewSale
    {
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Edit;
        public event EventHandler<EventArgs> Get;
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;

        public event EventHandler<UpdateInputArgs<IList<DataEngine.Entities.Sale>>> Update; 

        public int Id { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public DateTime? Timestamp { get; set; }

        private SalePresenter _presenter;
        private bool _exit = false;

        public SaleMenuView()
        {
            _presenter = new SalePresenter(this);
            _presenter.OnUpdateStatus += Presenter_OnUpdateStatus;
            _presenter.OnOperationFail += Presenter_OnOperationFail;
            _presenter.OnObjectReturned += Presenter_OnObjectReturned;
        }

        private void Presenter_OnObjectReturned(object sender, ObjectReturnedArgs<IList<DataEngine.Entities.Sale>> e)
        {
            if (e.Update)
            {
                DisplayStatus();
                Console.WriteLine("Update (ID: " + e.RecordList[0].Id);
                Console.WriteLine("NOTE: Leave blank if you don't want to update a field \n");
                Get_StoreID(null);
                Get_ProductID(null);
                Get_Timestamp(null);
                Update?.Invoke(this, new UpdateInputArgs<IList<Sale>>(e.RecordList, e.IsLocal));
            }
            else
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
        }

        private void Presenter_OnOperationFail(object sender, Events.OperationFailedArgs e)
        {
            DisplayStatus();
            Console.WriteLine(e.Status);
            Console.WriteLine("Error: " + (e.ExceptionObject.InnerException?.Message ?? e.ExceptionObject.Message));
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
            DisplayStatus();
            Console.WriteLine("Add Sale\n");
            Get_StoreID(null);
            Get_ProductID(Add);

            Console.ReadLine();
        }

        public void Show_Edit()
        {
            DisplayStatus();
            Console.WriteLine("Edit Store\n");
            Get_ID(Edit);

            Console.ReadLine();
        }

        public void Show_Get()
        {
            DisplayStatus();
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

        public void Show_GetAll()
        {
            DisplayStatus();
            GetAll?.Invoke(this, EventArgs.Empty);
        }

        public void Show_Remove()
        {
            DisplayStatus();
            Console.WriteLine("Remove Sale\n");
            Get_ID(Remove);

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

            if (!int.TryParse(input, out result)) return;

            Id = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Get_StoreID(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Store ID: ");
            var input = Console.ReadLine();
            int result;

            if (!int.TryParse(input, out result)) return;

            StoreId = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Get_ProductID(EventHandler<EventArgs> fireEvent)
        {
            Console.Write("Product ID: ");
            var input = Console.ReadLine();
            int result;

            if (!int.TryParse(input, out result)) return;

            ProductId = result;
            fireEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Get_Timestamp(EventHandler<EventArgs> fireevent)
        {
            Console.Write("Update timestamp? [yes or no]:");
            var input = Console.ReadLine();

            if (input != null && input.ToLower().Trim().Equals("yes"))
            {
                Timestamp = DateTime.Now;
                fireevent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
