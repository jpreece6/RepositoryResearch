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
using RepoConsole.Events;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    /// <summary>
    /// Handles the DB logic for the sales menu
    /// </summary>
    public class SalePresenter : Presenter
    {
        private readonly IViewSale _view;
        private readonly SaleRepository<Sale> _saleRepository;

        public event EventHandler<ObjectReturnedArgs<IList<Sale>>> OnObjectReturned;

        /// <summary>
        /// Creates new instance of sale presenter
        /// </summary>
        /// <param name="view">View to bind to</param>
        public SalePresenter(IViewSale view)
        {
            // Bind to all view events
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;
            _view.Edit += View_Edit;
            _view.Update += View_Update;

            // Create a new repository
            var sessionFactManager = new SessionFactoryManager();
            SessionContext = new SessionContext(sessionFactManager);

            _saleRepository = new SaleRepository<Sale>(SessionContext);
        }

        /// <summary>
        /// Called when the user wants to update a sale 
        /// </summary>
        /// <param name="sender">event owner</param>
        /// <param name="e">Event args</param>
        private void View_Update(object sender, UpdateInputArgs<IList<Sale>> e)
        {
            if (!OpenSession())
                return;

            // Ensure network state hasn't changed!
            // If it has we cannot ensure a safe update so we cannot continue the process
            if ((e.IsLocal && SessionContext.IsLocal()) ||
                (e.IsLocal == false && SessionContext.IsLocal() == false))
            {

                var isDirty = false;
                if (e.Record.Count == 0)
                {
                    OperationFailed(new Exception("No object available!"),
                        "Object to update was missing from the collection!");
                    return;
                }

                if (_view.StoreId != 0)
                {
                    e.Record[0].StoreId = _view.StoreId;
                    isDirty = true;
                }

                if (_view.ProductId != 0)
                {
                    e.Record[0].ProductId = _view.ProductId;
                    isDirty = true;
                }

                if (_view.Timestamp.HasValue)
                {
                    e.Record[0].Timestamp = _view.Timestamp.Value;
                    isDirty = true;
                }

                // No properties were changed inform the user
                // and do not continue with the update
                if (isDirty == false)
                {
                    UpdateStatus("Record not updated, no values changed!");
                    return;
                }

                try
                {
                    // Update the record
                    _saleRepository.Save(e.Record[0]);
                    _saleRepository.Commit();

                    UpdateStatus("\nRecord updated successfully!");
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not update record\n");
                }
            }
            else
            {
                OperationFailed(new Exception("Network state changed mid update!"), "Could not update record!");
            }
        }

        /// <summary>
        /// Called when the user wants to edit a sale
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Edit(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Sale sale;

            try
            {
                // Search by ID
                sale = _saleRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Sale by ID\n");
                return;
            }

            // None found inform the user
            if (sale == null)
            {
                UpdateStatus("Could not find Sale with an ID of " + _view.Id);
                return;
            }

            // Package the sale into a list so we can process it
            var output = new List<Sale>();
            output.Add(sale);

            // Return the object to the view
            OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Sale>>(output, true, SessionContext.IsLocal()));

        }

        /// <summary>
        /// Called when a user wants to remove a sale
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Remove(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Sale sale;
            try
            {
                // Search by ID
                sale = _saleRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not find Sale on remove\n");
                return;
            }

            // None found inform the user
            if (sale == null)
            {
                UpdateStatus("Could not find Sale with an Id of " + _view.Id);
                return;
            }

            try
            {
                // Remove from the DB
                _saleRepository.Remove(sale);
                _saleRepository.Commit();

                UpdateStatus("Sale Removed!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not remove Sale");
            }
        }

        /// <summary>
        /// Called when the user wants to return all sales
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_GetAll(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            IList<Sale> sales = new List<Sale>();
            try
            {
                // Find all
                sales = _saleRepository.GetAll();
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Sales\n");
                return;
            }

            // None found inform the user
            if (sales.Count == 0)
            {
                UpdateStatus("No Sales found.");
                return;
            }

            // Return the list of sales to the view
            OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Sale>>(sales, false));
        }

        /// <summary>
        /// Called when the user wants to find one or more sales
        /// using a specfied criteria
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Get(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            // Search by ID if we have one
            if (_view.Id != 0)
            {
                var sale = new Sale();
                try
                {
                    // Search by ID
                    sale = _saleRepository.Get(_view.Id);
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not retrieve Sale by ID\n");
                    return;
                }

                // Nothing found inform the user
                if (sale == null)
                {
                    UpdateStatus("Could not find Sale with an ID of " + _view.Id);
                    return;
                }

                // TODO REPLACE
                UpdateStatus("ID: " + sale.Id +
                             "\nStore ID: " + sale.StoreId +
                             "\nProduct ID: " + sale.ProductId +
                             "\nTimestamp: " + sale.Timestamp);

                return; // Prevent further searching

            }

            IList<Sale> sales = new List<Sale>();
            try
            {
                // Determine which search to perform
                if (_view.StoreId != 0)
                {
                    // Search by store ID
                    sales = _saleRepository.GetWithStoreId(_view.StoreId);
                } else if (_view.ProductId != 0)
                {
                    // Search by product ID
                    sales = _saleRepository.GetWithProductId(_view.ProductId);
                }
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Sale by Store ID\n");
                return;
            }

            // None found inform the user
            if (sales.Count == 0)
            {
                UpdateStatus("No Sales found with the Store ID of " + _view.StoreId);
                return;
            }

            // Return the list of sales back to the view
            OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Sale>>(sales, false));

        }

        /// <summary>
        /// Called when the user wants to add a new sale
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Add(object sender, EventArgs e)
        {

            if (!OpenSession())
                return;

            // Create a new sale object and assign values
            // values should be validated before getting here
            var sale = new Sale();
            sale.StoreId = _view.StoreId;
            sale.ProductId = _view.ProductId;
            sale.Timestamp = DateTime.Now;

            try
            {
                // Save new sale
                _saleRepository.Save(sale);
                _saleRepository.Commit();

                UpdateStatus("New sale created with ID of " + sale.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not add Sale\n");
            }
        }
    }
}
