using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    /// <summary>
    /// Defines the attributes of the product entity
    /// would have liked this to inherit from IEntity but
    /// due to the ID property being protected this is not possible
    /// </summary>
    interface IViewProduct : IView
    {
        int Id { get; set; }
        string Name { get; set; }
        float? Price { get; set; }

        event EventHandler<UpdateInputArgs<DataEngine.Entities.Product>> Update;
    }
}
