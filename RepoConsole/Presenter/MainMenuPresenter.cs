using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    class MainMenuPresenter : IPresenter
    {
        private IView _view;

        public MainMenuPresenter(IView view)
        {
            _view = view;
            Console.Clear();
        }
    }
}
