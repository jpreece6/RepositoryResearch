using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using RepoConsole.Events;
using RepoConsole.Views;
using SyncEngine;

namespace RepoConsole.Presenter
{
    /// <summary>
    /// Handles the DB logic for the store menu
    /// </summary>
    public class StorePresenter : Presenter
    {
        private readonly IViewStore _view;
        private readonly StoreRepository<Store> _storeRepository;

        public event EventHandler<ObjectReturnedArgs<IList<Store>>> OnObjectReturned;

        /// <summary>
        /// Creates a new instance of the store presenter
        /// </summary>
        /// <param name="view">View to bind to</param>
        public StorePresenter(IViewStore view)
        {
            // Bind to all view events
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;
            _view.Edit += View_Edit;
            _view.Update += View_Update;

            // Create new repository
            var sessionFactManager = new SessionFactoryManager();
            SessionContext = new SessionContext(sessionFactManager);

            _storeRepository = new StoreRepository<Store>(SessionContext);
        }

        /// <summary>
        /// Called when the user wants to update a store
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Update(object sender, UpdateInputArgs<IList<Store>> e)
        {
            if (!OpenSession())
                return;

            // Ensure network state hasn't changed!
            // If it has we cannot ensure a safe update so we cannot continue the process
            if ((e.IsLocal && SessionContext.IsLocal()) ||
                (e.IsLocal == false && SessionContext.IsLocal() == false))
            {
                // We should have at least 1 record
                // inform the user and do not update
                if (e.Record.Count == 0)
                {
                    OperationFailed(new Exception("No object available!"),
                        "Object to update was missing from the collection!");
                    return;
                }

                if (_view.StoreName != "" || _view.StoreName != null)
                {
                    e.Record[0].StoreName = _view.StoreName;
                }
                else
                {
                    // Nothing changed inform the user
                    UpdateStatus("Record not updated, value was not changed!");
                    return;
                }

                try
                {
                    // Update record in DB
                    _storeRepository.Save(e.Record[0]);
                    _storeRepository.Commit();

                    UpdateStatus("Record updated successfully!");
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not update record!");
                }
            }
            else
            {
                OperationFailed(new Exception("Network state changed mid update!"), "Could not update record!");
            }
        }

        /// <summary>
        /// Called when the user wants to edit a store
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Edit(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Store store;
            try
            {
                // Search by ID
                store = _storeRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Store on Edit");
                return;
            }

            // None found inform the user
            if (store == null)
            {
                UpdateStatus("No store found with an ID of " + _view.Id);
                return;
            }

            // Package store into list so we can return it
            var output = new List<Store>();
            output.Add(store);

            // Return the list of stores to the user
            // update = true, when we get the entity back update it
            // Connection state recorded so we can check this on update
            OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Store>>(output, true, SessionContext.IsLocal()));
        }

        /// <summary>
        /// Called when the user wants to remove a store
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Remove(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Store store;

            try
            {
                // Search by ID
                store = _storeRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Store on remove");
                return;
            }

            // Nothing found inform the user
            if (store == null)
            {
                UpdateStatus("No store found with ID of " + _view.Id);
                return;
            }

            try
            {
                // Remove from the DB
                _storeRepository.Remove(store);
                _storeRepository.Commit();

                UpdateStatus("Store Removed!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not remove Store from DB");
            }
        }

        /// <summary>
        /// Called when the user wants to get all stores
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_GetAll(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            IList<Store> stores;

            try
            {
                // Find all
                stores = _storeRepository.GetAll();
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve any Stores from GetAll");
                return;
            }

            // Nothing found inform the user
            if (stores.Count == 0)
            {
                UpdateStatus("No stores found.");
                return;
            }

            // Return the list of stores to the view
            OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Store>>(stores, false));
        }

        /// <summary>
        /// Called when the user wants to return one or more stores
        /// with a specified critiera
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
                Store store;
                try
                {
                    // Search by ID
                    store = _storeRepository.Get(_view.Id);
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not retrieve Store by ID");
                    return;
                }

                // None found inform the user
                if (store == null)
                {
                    UpdateStatus("No store found with an ID of " + _view.Id);
                    return;
                }

                // Package store into a list so we can return it
                var output = new List<Store>();
                output.Add(store);

                // Return list to the view
                OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Store>>(output, false));

                return;
            }

            // Search by name if we have one
            if (_view.StoreName != null)
            {
                IList<Store> stores;

                try
                {
                    // Search by name
                    stores = _storeRepository.GetWithName(_view.StoreName);
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not retrieve Store(s) by name");
                    return;
                }

                // Nothing found inform the user
                if (stores.Count == 0)
                {
                    UpdateStatus("No stores found with name " + _view.StoreName);
                    return;
                }

                // Return the list of stores to the view
                OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Store>>(stores, false));
            }
        }

        /// <summary>
        /// Called when the user wants to add a new store
        /// </summary>
        /// <param name="sender">Event owner</param>
        /// <param name="e">Event args</param>
        private void View_Add(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            // Create a new store object and assign values
            // values should be checked before getting here
            var store = new Store();
            store.StoreName = _view.StoreName;

            try
            {
                // Add the store to the DB
                _storeRepository.Save(store);
                _storeRepository.Commit();

                UpdateStatus("New Store created with ID of " + store.Id);

            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not add record to DB");
            }
        }

    }
}
