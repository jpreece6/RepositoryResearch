using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Views
{
    public interface IViewSale : IView
    {
        int Id { get; set; }
        int StoreId { get; set; }
        int ProductId { get; set; }
    }
}
