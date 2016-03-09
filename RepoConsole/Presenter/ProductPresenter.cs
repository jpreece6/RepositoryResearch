using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using Helpers;
using RepoConsole.Events;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    /// <summary>
    /// Handles the DB logic for the Product menu
    /// </summary>
    class ProductPresenter : Presenter
    {
        private readonly IViewProduct _view;
        private readonly ProductRepository<Product> _productRepository;

        public event EventHandler<ObjectGetReturnedArgs<Product>> OnObjectGetReturned;
        public event EventHandler<ObjectGetAllReturnedArgs<IList<Product>>> OnObjectGetAllReturned;

        /// <summary>
        /// Creates a new instance of the product presenter
        /// </summary>
        /// <param name="view">View to bind to</param>
        public ProductPresenter(IViewProduct view)
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

            _productRepository = new ProductRepository<Product>(SessionContext);
        }

        /// <summary>
        /// Called when a user wants to update a entity
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Update(object sender, UpdateInputArgs<Product> e)
        {
            if (!OpenSession())
                return;

            // Ensure network state hasn't changed!
            // If it has we cannot ensure a safe update so we cannot continue the process
            if ((e.IsLocal && SessionContext.IsLocal()) ||
                (e.IsLocal == false && SessionContext.IsLocal() == false))
            {

                var isDirty = false;

                // We should have at least 1 object
                // if not inform the user and prevent the update
                if (e.Entity == null)
                {
                    OperationFailed(new Exception("No object available!"),
                        "Object to update was missing from the collection!");
                    return;
                }

                if (_view.Name != "" || _view.Name != null)
                {
                    e.Entity.Prod_Name = _view.Name;
                    isDirty = true;
                }

                if (_view.Price.HasValue)
                {
                    e.Entity.Price = _view.Price.Value;
                    isDirty = true;
                }

                // No properties were changed inform the user
                // we don't need to update the entity
                if (isDirty == false)
                {
                    UpdateStatus("Record not updated, no values changed!");
                    return;
                }

                try
                {
                    // Update the entity in the DB
                    _productRepository.Save(e.Entity);
                    _productRepository.Commit();

                    UpdateStatus("\nRecord updated successfully!");
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not update product");
                }
            }
            else
            {
                OperationFailed(new Exception("Network state changed mid update!"), "Could not update record!");
            }
        }

        /// <summary>
        /// Called when the user wants to edit an entity
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Edit(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Product prod;

            try
            {
                // Search with ID
                prod = _productRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Product on get");
                return;
            }

            // No products found with the criteria inform the user
            if (prod == null)
            {
                UpdateStatus("No product found with an ID of " + _view.Id);
                return;
            }

            // Package our product object into a list so we can return it
            var output = new List<Product>();
            output.Add(prod);

            // Return the object to the view so we can process it
            // update = true, so we will update the entity when we get it back
            // Connection state is recorded so we can check is on update
            OnObjectGetReturned?.Invoke(this, new ObjectGetReturnedArgs<Product>(prod, true, SessionContext.IsLocal()));

        }

        /// <summary>
        /// Called when the user wants to return all Products
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_GetAll(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            IList<Product> prods;

            try
            {
                // Get all
                prods = _productRepository.GetAll();
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Products\n");
                return;
            }

            // We found nothing so inform the user
            if (prods.Count <= 0)
            {
                UpdateStatus("No products found.");
                return;
            }

            // Return the list of products to the view to display them
            OnObjectGetAllReturned?.Invoke(this, new ObjectGetAllReturnedArgs<IList<Product>>(prods));
        }

        /// <summary>
        /// Called when the user wants to remove a product
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Remove(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Product prod;

            try
            {
                // Search by ID
                prod = _productRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Product on Remove");
                return;
            }

            // No product found inform the user
            if (prod == null)
            {
                UpdateStatus("No product found with an ID of " + _view.Id);
                return;
            }

            try
            {
                // Remove the product from the DB
                _productRepository.Remove(prod);
                _productRepository.Commit();

                UpdateStatus("Product Removed!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not remove Product!");
            }
        }

        /// <summary>
        /// Called when the user wants to search for one or more
        /// products in the DB with a ceriteria
        /// </summary>
        /// <param name="sender">event owner</param>
        /// <param name="e">Event args</param>
        private void View_Get(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            // Search by ID if we have one
            if (_view.Id != 0)
            {
                Product prod;

                try
                {
                    // Search by ID
                    prod = _productRepository.Get(_view.Id);
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not retrieve Product on get");
                    return;
                }

                // Nothing found inform the user and return
                if (prod == null)
                {
                    UpdateStatus("No product found with an ID of " + _view.Id);
                    return;
                }

                // Display what we found to the user
                UpdateStatus("ID: " + prod.Id +
                             "\nName: " + prod.Prod_Name +
                             "\nPrice: " + prod.Price);
                // Stop further searching
                return;
            }

            IList<Product> prods;

            try
            {
                // Dtermine the search to perform
                if (_view.Name != null)
                {
                    // Search by name
                    prods = _productRepository.GetWithName(_view.Name);
                }
                else
                {
                    // Search by price
                    prods = _productRepository.GetWithPrice(_view.Price.Value);
                }
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Product with Name or Price");
                return;
            }

            // Nothing found inform the user
            if (prods.Count == 0)
            {
                UpdateStatus("No Products Found");
                return;
            }

            // Return a list of products that we found to display them
            OnObjectGetAllReturned?.Invoke(this, new ObjectGetAllReturnedArgs<IList<Product>>(prods));

        }

        /// <summary>
        /// Called when the user wants to add a new product
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Add(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            // Create a new object and add the new values
            // the values should be checked before getting here
            var prod = new Product();
            prod.Prod_Name = _view.Name;
            prod.Price = _view.Price.Value;

            try
            {
                // Save the new product to the DB
                _productRepository.Save(prod);
                _productRepository.Commit();

                UpdateStatus("Added new product with ID of " + prod.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not save Product");
            }
        }
    }
}
