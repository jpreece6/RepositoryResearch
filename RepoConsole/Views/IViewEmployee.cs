using System;
using System.Collections.Generic;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    internal interface IViewEmployee : IView
    {
        int Id { get; set; }
        string FirstName { get; set; }
        int StoreId { get; set; }

        event EventHandler<UpdateInputArgs<IList<DataEngine.Entities.Employee>>> Update;
    }
}