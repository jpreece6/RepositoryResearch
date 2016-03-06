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
    public class SalePresenter : Presenter
    {
        private readonly IViewSale _view;
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
            SessionContext = new SessionContext(sessionFactManager);

            _saleRepository = new SaleRepository<Sale>(SessionContext);
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
                OperationFailed(ex, "Could not retrieve Sale by ID\n");
                return;
            }

            if (sale == null)
            {
                UpdateStatus("Could not find Sale with an ID of " + _view.Id);
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
                UpdateStatus("\nRecord updated successfully!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not update record\n");
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
                OperationFailed(ex, "Could not find Sale on remove\n");
                return;
            }

            if (sale == null)
            {
                UpdateStatus("Could not find Sale with an Id of " + _view.Id);
                return;
            }

            try
            {
                _saleRepository.Remove(sale);
                _saleRepository.Commit();
                UpdateStatus("Sale Removed!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not remove Sale");
            }
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
                OperationFailed(ex, "Could not retrieve Sales\n");
                return;
            }

            if (sales.Count == 0)
            {
                UpdateStatus("No Sales found.");
                return;
            }

            foreach (var sale in sales)
            {
                UpdateStatus("ID: " + sale.Id +
                             "\nStore ID: " + sale.StoreId +
                             "\nProduct ID: " + sale.ProductId +
                             "\nTimestamp: " + sale.Timestamp +
                             "\n");
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
                    OperationFailed(ex, "Could not retrieve Sale by ID\n");
                    return;
                }

                if (sale == null)
                {
                    UpdateStatus("Could not find Sale with an ID of " + _view.Id);
                    return;
                }

                UpdateStatus("ID: " + sale.Id +
                             "\nStore ID: " + sale.StoreId +
                             "\nProduct ID: " + sale.ProductId +
                             "\nTimestamp: " + sale.Timestamp);

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
                OperationFailed(ex, "Could not retrieve Sale by Store ID\n");
                return;
            }

            if (sales.Count == 0)
            {
                UpdateStatus("No Sales found with the Store ID of " + _view.StoreId);
                return;
            }

            foreach (var sale in sales)
            {
                UpdateStatus("ID: " + sale.Id +
                             "\nStore ID: " + sale.StoreId +
                             "\nProduct ID: " + sale.ProductId +
                             "\nTimestamp: " + sale.Timestamp +
                             "\n");
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
                OperationFailed(ex, "Could not add Sale\n");
                return;
            }

            UpdateStatus("New sale created with ID of " + sale.Id);
        }
    }
}
