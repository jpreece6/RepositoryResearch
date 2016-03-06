using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using RepoConsole.Views;
using SyncEngine;

namespace RepoConsole.Presenter
{
    public class StorePresenter : IPresenter
    {
        private readonly IViewStore _view;
        private readonly StoreRepository<Store> _storeRepository;
        private readonly SessionContext _sessionContext;

        public StorePresenter(IViewStore view)
        {
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;

            // Init page.. 
            var sessionFactManager = new SessionFactoryManager();
            _sessionContext = new SessionContext(sessionFactManager);

            _storeRepository = new StoreRepository<Store>(_sessionContext);
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
                Console.WriteLine("Could not retrieve Store on remove");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            if (store == null)
            {
                Console.WriteLine("No store found with ID of " + _view.Id);
                return;
            }

            try
            {
                _storeRepository.Remove(store);
                _storeRepository.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not remove Store from DB");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            Console.WriteLine("Store Removed!");
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
                Console.WriteLine("Could not retrieve any Stores from GetAll");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            if (stores.Count == 0)
            {
                Console.WriteLine("No stores found.");
                return;
            }

            foreach (var store in stores)
            {
                Console.WriteLine("ID: " + store.Id);
                Console.WriteLine("Name: " + store.StoreName);
                Console.WriteLine("");
            }
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
                    Console.WriteLine("Could not retrieve Store on Get");
                    Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                    return;
                }

                if (store == null)
                {
                    Console.WriteLine("No store found with an ID of " + _view.Id);
                    return;
                }

                Console.WriteLine("ID: " + store.Id);
                Console.WriteLine("Name: " + store.StoreName);
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
                    Console.WriteLine("Could not retrieve Store(s) by name");
                    Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                    return;
                }

                if (stores.Count == 0)
                {
                    Console.WriteLine("No stores found with name " + _view.StoreName);
                    return;
                }

                foreach (var store in stores)
                {
                    Console.WriteLine("ID: " + store.Id);
                    Console.WriteLine("Name: " + store.StoreName);
                    Console.WriteLine("");
                }
            }
        }

        private void View_Add(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            if (_storeRepository.AllowLocalEdits == false && _sessionContext.IsLocal())
            {
                //Console.WriteLine("Unable to create store");
            }

            var store = new Store();
            store.StoreName = _view.StoreName;

            try
            {
                _storeRepository.Save(store);
                _storeRepository.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not add new Store to DB");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            Console.WriteLine("New store created with ID of " + store.Id);
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

        private void SessionFinished()
        {
            if (_sessionContext.IsLocal())
            {
                Console.WriteLine("Local instance found attemping to sync");
                SyncManager syncManager = new SyncManager();
                syncManager.SyncTable_Store();
            }
        }
    }
}
