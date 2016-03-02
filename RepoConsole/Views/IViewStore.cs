using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Views
{
    public interface IViewStore : IView
    {
        int Id { get; set; }
        string StoreName { get; set; }
    }
}
