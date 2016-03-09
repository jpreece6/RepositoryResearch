using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Entities;
using RepoConsole.Views;

namespace RepoConsole.Presenter
{
    /// <summary>
    /// Controls the DB loigc for the main menu (NONE)
    /// </summary>
    class MainMenuPresenter : Presenter
    {
        private IView _view;

        /// <summary>
        /// Creates a new instance of the main menu presenter
        /// </summary>
        /// <param name="view"></param>
        public MainMenuPresenter(IView view)
        {
            _view = view;
        }
    }
}
