using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.Helpers;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    public class StorePresenter : IPresenter
    {
        private readonly IViewStore _view;
        private StoreRepository<Store> _storeRepository;

        public StorePresenter(IViewStore view)
        {
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;
            Initialise();
        }

        private void View_Remove(object sender, EventArgs e)
        {
            Console.WriteLine("");
            var store = _storeRepository.Get(_view.Id);

            if (store == null)
            {
                Console.WriteLine("No store found with ID of " + _view.Id);
                return;
            }

            _storeRepository.Remove(store);
            _storeRepository.Commit();

            Console.WriteLine("Store Removed!");
        }

        private void View_GetAll(object sender, EventArgs e)
        {
            Console.Clear();
            var stores = _storeRepository.GetAll();

            if (stores.Count <= 0)
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
            Console.Clear();
            if (_view.Id != 0)
            {
                var store = _storeRepository.Get(_view.Id);
                if (store == null)
                {
                    Console.WriteLine("No store found with an ID of " + _view.Id);
                    return;
                }

                Console.WriteLine("ID: " + store.Id);
                Console.WriteLine("Name: " + store.StoreName);
                return;
            }

            if (_view.StoreName != null)
            {
                var stores = _storeRepository.GetWithName(_view.StoreName);

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
            Console.WriteLine("");
            var store = new Store();
            store.StoreName = _view.StoreName;

            _storeRepository.Save(store);
            _storeRepository.Commit();

            Console.WriteLine("New store created with ID of " + store.Id);
        }

        public void Initialise()
        {
            // Load settings
            var configReader = new ConfigReader(@"C:\repoSettings.xml");
            BaseConfig.Sources = configReader.GetAllInstancesOf("ConnectionString");

            // Init page.. 
            var sessionFactManager = new SessionFactoryManager();
            var sessionContext = new SessionContext(sessionFactManager);
            sessionContext.OpenContextSession();

            _storeRepository = new StoreRepository<Store>(sessionContext);
        }
    }
}
