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
    class AddProductPresenter : IPresenter
    {
        private Product _product;
        private IViewProduct _view;

        private EmployeeRepository<Product> _employeeRepository; 

        public AddProductPresenter(IViewProduct view)
        {
            _view = view;
            view.Add += View_Add;
            
            Initialise();
        }

        private void View_Add(object sender, EventArgs e)
        {
            var product = new Product();
            product.Prod_Name = _view.Name;
            product.Price = _view.Price;
            
            _employeeRepository.Save(product);
            _employeeRepository.Commit();
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
