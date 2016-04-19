using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    /// <summary>
    /// Defines the attributes of the sale entity
    /// would have liked this to inherit from IEntity but
    /// due to the ID property being protected this is not possible
    /// </summary>
    public interface IViewSale : IView
    {
        int Id { get; set; }
        int StoreId { get; set; }
        int ProductId { get; set; }
        DateTime? Timestamp { get; set; }

        event EventHandler<UpdateInputArgs<DataEngine.Entities.Sale>> Update;
    }
}
