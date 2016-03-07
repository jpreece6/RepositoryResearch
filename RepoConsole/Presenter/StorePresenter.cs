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
    public class StorePresenter : Presenter
    {
        private readonly IViewStore _view;
        private readonly StoreRepository<Store> _storeRepository;
       
        public event EventHandler<ObjectReturnedArgs<IList<Store>>> OnObjectReturned;

        public StorePresenter(IViewStore view)
        {
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;
            _view.Edit += View_Edit;
            _view.Update += View_Update;

            // Init page.. 
            var sessionFactManager = new SessionFactoryManager();
            SessionContext = new SessionContext(sessionFactManager);

            _storeRepository = new StoreRepository<Store>(SessionContext);
        }

        private void View_Update(object sender, UpdateInputArgs<IList<Store>> e)
        {
            if (!OpenSession())
                return;

            if (e.Record.Count == 0)
            {
                OperationFailed(new Exception("No object available!"), "Object to update was missing from the collection!");
                return;
            }

            e.Record[0].StoreName = _view.StoreName;

            try
            {
                _storeRepository.Save(e.Record[0]);
                _storeRepository.Commit();
                UpdateStatus("Record updated successfully!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not update record!");
            }
        }

        private void View_Edit(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Store store;
            try
            {
                store = _storeRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Store on Edit");
                return;
            }

            if (store == null)
            {
                UpdateStatus("No store found with an ID of " + _view.Id);
                return;
            }

            var output = new List<Store>();
            output.Add(store);
            OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Store>>(output, true));
        }

        private void View_Remove(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Store store;

            try
            {
                store = _storeRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Store on remove");
                return;
            }

            if (store == null)
            {
                UpdateStatus("No store found with ID of " + _view.Id);
                return;
            }

            try
            {
                _storeRepository.Remove(store);
                _storeRepository.Commit();
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not remove Store from DB");
                return;
            }

            UpdateStatus("Store Removed!");
        }

        private void View_GetAll(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            IList<Store> stores;

            try
            {
                stores = _storeRepository.GetAll();
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve any Stores from GetAll");
                return;
            }

            if (stores.Count == 0)
            {
                UpdateStatus("No stores found.");
                return;
            }

            OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Store>>(stores, false));
        }

        private void View_Get(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            // Get by ID
            if (_view.Id != 0)
            {
                Store store;
                try
                {
                    store = _storeRepository.Get(_view.Id);
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not retrieve Store by ID");
                    return;
                }

                if (store == null)
                {
                    UpdateStatus("No store found with an ID of " + _view.Id);
                    return;
                }

                var output = new List<Store>();
                output.Add(store);
                OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Store>>(output, false));

                return;
            }

            // Get by name
            if (_view.StoreName != null)
            {
                IList<Store> stores;

                try
                {
                    stores = _storeRepository.GetWithName(_view.StoreName);
                }
                catch (Exception ex)
                {
                    OperationFailed(ex, "Could not retrieve Store(s) by name");
                    return;
                }

                if (stores.Count == 0)
                {
                    UpdateStatus("No stores found with name " + _view.StoreName);
                    return;
                }

                OnObjectReturned?.Invoke(this, new ObjectReturnedArgs<IList<Store>>(stores, false));
            }
        }

        private void View_Add(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            var store = new Store();
            store.StoreName = _view.StoreName;

            try
            {
                _storeRepository.Save(store);
                _storeRepository.Commit();
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not add record to DB");
                return;
            }

            UpdateStatus("New Store created with ID of " + store.Id);
        }
    }
}
