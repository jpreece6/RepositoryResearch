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
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    class ProductPresenter : IPresenter
    {
        private readonly IViewProduct _view;
        private readonly ProductRepository<Product> _productRepository;
        private readonly SessionContext _sessionContext;

        public ProductPresenter(IViewProduct view)
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

            _productRepository = new ProductRepository<Product>(_sessionContext);
        }

        private void View_Edit(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            if (_view.Id == 0)
            {
                Console.WriteLine("No ID sepecifed");
                return;
            }

            Product prod;

            try
            {
                prod = _productRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve Product on get");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            if (prod == null)
            {
                Console.WriteLine("No product found with an ID of " + _view.Id);
                return;
            }

            Console.Clear();
            Console.WriteLine("Edit: (ID: " + prod.Id + ") " + prod.Prod_Name);
            Console.Write("\nName: ");

            var input = Console.ReadLine();
            if (input == null)
            {
                Console.WriteLine("\nPlease enter a name!");
                return;
            }

            prod.Prod_Name = input;
            Console.Write("Price: ");
            input = Console.ReadLine();
            float result;

            if (float.TryParse(input, out result) == false)
            {
                Console.WriteLine("\nPlease enter a valid type!");
                return;
            }

            prod.Price = result;

            try
            {
                _productRepository.Save(prod);
                _productRepository.Commit();
                Console.WriteLine("\nRecord updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not update product");
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        private void View_GetAll(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            IList<Product> prods;

            try
            {
                prods = _productRepository.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve Products\n");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

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
            if (!OpenSession())
                return;

            Product prod;

            if (_productRepository.AllowLocalEdits == false && _sessionContext.IsLocal())
            {
                Console.WriteLine("Unable to remove Product local remove is not allowed!");
                return;
            }

            try
            {
                prod = _productRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve Product on Remove");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            if (prod == null)
            {
                Console.WriteLine("No product found with an ID of " + _view.Id);
                return;
            }

            try
            {
                _productRepository.Remove(prod);
                _productRepository.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not remove Product!");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            Console.WriteLine("Removed");
        }

        private void View_Get(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            if (_view.Id != 0)
            {
                Product prod;

                try
                {
                    prod = _productRepository.Get(_view.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not retrieve Product on get");
                    Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                    return;
                }

                if (prod == null)
                {
                    Console.WriteLine("No product found with an ID of " + _view.Id);
                    return;
                }

                Console.WriteLine("ID: " + prod.Id);
                Console.WriteLine("Name: " + prod.Prod_Name);
                Console.WriteLine("Price: " + prod.Price);
                return;
            }

            IList<Product> prods;

            try
            {
                if (_view.Name != null)
                {
                    prods = _productRepository.GetWithName(_view.Name);
                }
                else
                {
                    prods = _productRepository.GetWithPrice(_view.Price);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not retrieve Product with Name");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            if (prods.Count == 0)
            {
                Console.WriteLine("No Products Found");
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

        private void View_Add(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            var prod = new Product();

            if (_productRepository.AllowLocalEdits == false && _sessionContext.IsLocal())
            {
                Console.WriteLine("Unable to create Product local creation not allowed!");
                return;
            }

            prod.Prod_Name = _view.Name;
            prod.Price = _view.Price;

            try
            {

                _productRepository.Save(prod);
                _productRepository.Commit();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not save Product");
                Console.WriteLine("Error: " + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            Console.WriteLine("Added new product with ID of " + prod.Id);
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
