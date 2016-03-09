using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    public interface IViewStore : IView
    {
        int Id { get; set; }
        string StoreName { get; set; }

        event EventHandler<UpdateInputArgs<DataEngine.Entities.Store>>  Update;
    }
}
