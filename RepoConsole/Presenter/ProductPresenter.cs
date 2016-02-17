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
using DataEngine.Helpers;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    class ProductPresenter : IPresenter
    {
        private readonly IViewProduct _view;
        private ProductRepository<Product> _productRepository; 

        public ProductPresenter(IViewProduct view)
        {
            _view = view;
            _view.Add += View_Add;
            _view.Get += View_Get;
            _view.GetAll += View_GetAll;
            _view.Remove += View_Remove;

            Initialise();
        }

        private void View_GetAll(object sender, EventArgs e)
        {
            Console.WriteLine("");
            var prods = _productRepository.GetAll();

            if (prods.Count <= 0)
            {
                Console.WriteLine("No products found.");
                return;
            }

            foreach (var product in prods)
            {
                Console.WriteLine("ID: " + product.Id);
                Console.WriteLine("Name: " + product.Prod_Name);
                Console.WriteLine("Price: " + product.Price);
                Console.WriteLine("");
            }
        }

        private void View_Remove(object sender, EventArgs e)
        {
            Console.WriteLine("");
            var prod = _productRepository.Get(_view.Id);

            if (prod == null)
            {
                Console.WriteLine("No product found with an ID of " + _view.Id);
                return;
            }

            _productRepository.Remove(prod);
            _productRepository.Commit();

            Console.WriteLine("Removed");
        }

        private void View_Get(object sender, EventArgs e)
        {
            Console.WriteLine("");
            var prod = _productRepository.Get(_view.Id);

            if (prod == null)
            {
                Console.WriteLine("No product found with an ID of " + _view.Id);
                return;
            }

            Console.WriteLine("Name: " + prod.Prod_Name);
            Console.WriteLine("Price: " + prod.Price);
        }

        private void View_Add(object sender, EventArgs e)
        {
            Console.WriteLine("");
            var prod = new Product();
            prod.Prod_Name = _view.Name;
            prod.Price = _view.Price;

            _productRepository.Save(prod);
            _productRepository.Commit();

            Console.WriteLine("Added new product with ID of " + prod.Id);
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

            _productRepository = new ProductRepository<Product>(sessionContext);
        }
    }
}
