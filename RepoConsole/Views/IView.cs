using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    /// <summary>
    /// Enfore all views to include the
    /// followinf methods
    /// </summary>
    public interface IView
    {
        event EventHandler<EventArgs> Add;
        event EventHandler<EventArgs> Edit;
        event EventHandler<EventArgs> Get;
        event EventHandler<EventArgs> GetAll;
        event EventHandler<EventArgs> Remove;

        void Show();
        void Show_Add();
        void Show_Edit();
        void Show_Get();
        void Show_GetAll();
        void Show_Remove();
        void WaitForInput();
        //void GetEditInput();
    }
}
