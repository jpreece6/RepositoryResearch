using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Views
{
    interface IViewProduct : IView
    {
        int Id { get; set; }
        string Name { get; set; }
        float Price { get; set; }
    }
}
