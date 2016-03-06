using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    public class SalePresenter : IPresenter
    {
        private readonly IViewSale _view;
        private readonly SessionContext _sessionContext;
        private readonly SaleRepository<Sale> _saleRepository;

        public SalePresenter(IViewSale view)
        {
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;
            _view.Edit += View_Edit;

            // Init page.. 
            var sessionFactManager = new SessionFactoryManager();
            _sessionContext = new SessionContext(sessionFactManager);

            _saleRepository = new SaleRepository<Sale>(_sessionContext);
        }

        private void View_Edit(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Sale sale;

            try
            {
                sale = _saleRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve Sale by ID\n");
                Console.WriteLine("Error: " + ex.Message);
                return;
            }

            if (sale == null)
            {
                Console.WriteLine("Could not find Sale with an ID of " + _view.Id);
                return;
            }

            Console.WriteLine("Edit: (ID: " + sale.Id + ")\n");
            Console.Write("Store ID: ");

            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                Console.WriteLine("\nPlease enter a valid Store ID");
                return;
            }

            sale.StoreId = result;

            Console.Write("Product ID: ");
            input = Console.ReadLine();

            if (int.TryParse(input, out result))
            {
                Console.WriteLine("\nPlease enter a valid Product ID");
                return;
            }

            sale.ProductId = result;

            Console.Write("\nUpdate Timestamp? [yes or no]: ");
            input = Console.ReadLine();

            if (input.ToLower().Equals("yes"))
                sale.Timestamp = DateTime.Now;

            try
            {
                _saleRepository.Save(sale);
                _saleRepository.Commit();
                Console.WriteLine("\nRecord updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not update record");
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void View_Remove(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Sale sale;
            try
            {
                sale = _saleRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not find Sale on remove");
                Console.WriteLine("Error: " + ex.Message);
                return;
            }

            if (sale == null)
            {
                Console.WriteLine("Could not find Sale with an Id of " + _view.Id);
                return;
            }

            try
            {
                _saleRepository.Remove(sale);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not remove Sale");
                Console.WriteLine("Error: " + ex.Message);
                return;
            }

            _saleRepository.Commit();
            Console.WriteLine("Sale Removed!");
        }

        private void View_GetAll(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            IList<Sale> sales = new List<Sale>();
            try
            {
                sales = _saleRepository.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve Sales");
                Console.WriteLine("Error: " + ex.Message);
                return;
            }

            if (sales.Count == 0)
            {
                Console.WriteLine("No Sales found.");
                return;
            }

            foreach (var sale in sales)
            {
                Console.WriteLine("ID: " + sale.Id);
                Console.WriteLine("Store ID: " + sale.StoreId);
                Console.WriteLine("Product ID: " + sale.ProductId);
                Console.WriteLine("Timestamp: " + sale.Timestamp);
                Console.WriteLine("");
            }
        }

        private void View_Get(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            if (_view.Id != 0)
            {
                var sale = new Sale();
                try
                {
                    sale = _saleRepository.Get(_view.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not retrieve Sale by ID\n");
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

                if (sale == null)
                {
                    Console.WriteLine("Could not find Sale with an ID of " + _view.Id);
                    return;
                }

                Console.WriteLine("ID: " + sale.Id);
                Console.WriteLine("Store ID: " + sale.StoreId);
                Console.WriteLine("Product ID: " + sale.ProductId);
                Console.WriteLine("Timestamp: " + sale.Timestamp);
                return;

            }

            IList<Sale> sales = new List<Sale>();
            try
            {
                if (_view.StoreId != 0)
                {
                    sales = _saleRepository.GetWithStoreID(_view.StoreId);
                } else if (_view.ProductId != 0)
                {
                    sales = _saleRepository.GetWithProductID(_view.ProductId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve Sale by Store ID\n");
                Console.WriteLine("Error: " + ex.Message);
                return;
            }

            if (sales.Count == 0)
            {
                Console.WriteLine("No Sales found with the Store ID of " + _view.StoreId);
                return;
            }

            foreach (var sale in sales)
            {
                Console.WriteLine("ID: " + sale.Id);
                Console.WriteLine("Store ID: " + sale.StoreId);
                Console.WriteLine("Product ID: " + sale.ProductId);
                Console.WriteLine("Timestamp: " + sale.Timestamp);
                Console.WriteLine("");
            }

        }

        private void View_Add(object sender, EventArgs e)
        {

            if (!OpenSession())
                return;

            var sale = new Sale();
            sale.StoreId = _view.StoreId;
            sale.ProductId = _view.ProductId;
            sale.Timestamp = DateTime.Now;

            try
            {
                _saleRepository.Save(sale);
                _saleRepository.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not add Sale\n");
                Console.WriteLine("Error: " + ex.Message);
                return;
            }

            Console.WriteLine("New sale created with ID of " + sale.Id);
        }

        private bool OpenSession()
        {
            Console.WriteLine("\nConnecting....");

            try
            {
                _sessionContext.OpenContextSession();
                Console.Clear();

                if (_sessionContext.IsLocal())
                {
                    Console.WriteLine("Could not connect to server!\n- Now using local DB for this operation!\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to server!");
                return false;
            }

            return true;
        }
    }
}
