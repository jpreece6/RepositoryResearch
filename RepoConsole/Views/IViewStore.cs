using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    /// <summary>
    /// Defines the attributes of the store entity
    /// would have liked this to inherit from IEntity but
    /// due to the ID property being protected this is not possible
    /// </summary>
    public interface IViewStore : IView
    {
        int Id { get; set; }
        string StoreName { get; set; }

        event EventHandler<UpdateInputArgs<DataEngine.Entities.Store>>  Update;
    }
}
