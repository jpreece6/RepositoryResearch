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
    class ProductPresenter : Presenter
    {
        private readonly IViewProduct _view;
        private readonly ProductRepository<Product> _productRepository;

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
            SessionContext = new SessionContext(sessionFactManager);

            _productRepository = new ProductRepository<Product>(SessionContext);
        }

        private void View_Edit(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Product prod;

            try
            {
                prod = _productRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Product on get");
                return;
            }

            if (prod == null)
            {
                UpdateStatus("No product found with an ID of " + _view.Id);
                return;
            }

            Console.Clear();
            Console.WriteLine("Edit: (ID: " + prod.Id + ") " + prod.Prod_Name);
            Console.Write("\nName: ");

            var input = Console.ReadLine();
            if (input == null)
            {
                UpdateStatus("\nPlease enter a name!");
                return;
            }

            prod.Prod_Name = input;
            Console.Write("Price: ");
            input = Console.ReadLine();
            float result;

            if (float.TryParse(input, out result) == false)
            {
                UpdateStatus("\nPlease enter a valid type!");
                return;
            }

            prod.Price = result;

            try
            {
                _productRepository.Save(prod);
                _productRepository.Commit();
                UpdateStatus("\nRecord updated successfully!");
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not update product");
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
                OperationFailed(ex, "Could not retrieve Products\n");
                return;
            }

            if (prods.Count <= 0)
            {
                UpdateStatus("No products found.");
                return;
            }

            foreach (var product in prods)
            {
                UpdateStatus("ID: " + product.Id +
                             "\nName: " + product.Prod_Name +
                             "\nPrice: " + product.Price +
                             "\n");
            }
        }

        private void View_Remove(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            Product prod;

            /* TODO
            if (_productRepository.AllowLocalEdits == false && SessionContext.IsLocal())
            {
                Console.WriteLine("Unable to remove Product local remove is not allowed!");
                return;
            }*/

            try
            {
                prod = _productRepository.Get(_view.Id);
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not retrieve Product on Remove");
                return;
            }

            if (prod == null)
            {
                UpdateStatus("No product found with an ID of " + _view.Id);
                return;
            }

            try
            {
                _productRepository.Remove(prod);
                _productRepository.Commit();
            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not remove Product!");
                return;
            }

            UpdateStatus("Product Removed!");
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
                    OperationFailed(ex, "Could not retrieve Product on get");
                    return;
                }

                if (prod == null)
                {
                    UpdateStatus("No product found with an ID of " + _view.Id);
                    return;
                }

                UpdateStatus("ID: " + prod.Id +
                             "\nName: " + prod.Prod_Name +
                             "\nPrice: " + prod.Price);
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
                OperationFailed(ex, "Could not retrieve Product with Name or Price");
                return;
            }

            if (prods.Count == 0)
            {
                UpdateStatus("No Products Found");
                return;
            }

            foreach (var product in prods)
            {
                UpdateStatus("ID: " + product.Id +
                             "Name: " + product.Prod_Name +
                             "Price: " + product.Price +
                             "\n");
            }

        }

        private void View_Add(object sender, EventArgs e)
        {
            if (!OpenSession())
                return;

            var prod = new Product();

            /* TODO
            if (_productRepository.AllowLocalEdits == false && SessionContext.IsLocal())
            {
                Console.WriteLine("Unable to create Product local creation not allowed!");
                return;
            }*/

            prod.Prod_Name = _view.Name;
            prod.Price = _view.Price;

            try
            {

                _productRepository.Save(prod);
                _productRepository.Commit();

            }
            catch (Exception ex)
            {
                OperationFailed(ex, "Could not save Product");
                return;
            }

            UpdateStatus("Added new product with ID of " + prod.Id);
        }
    }
}
