using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    public interface IViewSale : IView
    {
        int Id { get; set; }
        int StoreId { get; set; }
        int ProductId { get; set; }
        DateTime? Timestamp { get; set; }

        event EventHandler<UpdateInputArgs<DataEngine.Entities.Sale>> Update;
    }
}
