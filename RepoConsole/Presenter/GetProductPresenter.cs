using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using DataEngine.Helpers;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    class GetProductPresenter : IPresenter
    {
        private Product _product;
        private IViewProduct _viewProduct;
        private EmployeeRepository<Product> _employeeRepository; 

        public GetProductPresenter(IViewProduct viewProduct)
        {
            _viewProduct = viewProduct;
            _viewProduct.Get += _viewProduct_Get;
            Initialise();
        }

        private void _viewProduct_Get(object sender, EventArgs e)
        {
            Product prod = _employeeRepository.Get(_viewProduct.Id);
            if (prod == null)
            {
                Console.WriteLine("No product found with an ID of " + _viewProduct.Id);
                return;
            }
            Console.WriteLine("Name: " + prod.Prod_Name);
            Console.WriteLine("Price: " + prod.Price);
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

            _employeeRepository = new EmployeeRepository<Product>(sessionContext);
        }
    }
}
