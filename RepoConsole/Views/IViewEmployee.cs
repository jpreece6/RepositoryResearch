using System;
using System.Collections.Generic;
using RepoConsole.Events;

namespace RepoConsole.Views
{
    /// <summary>
    /// Defines the attributes of the employee entity
    /// would have liked this to inherit from IEntity but
    /// due to the ID property being protected this is not possible
    /// </summary>
    internal interface IViewEmployee : IView
    {
        int Id { get; set; }
        string FirstName { get; set; }
        int StoreId { get; set; }

        event EventHandler<UpdateInputArgs<DataEngine.Entities.Employee>> Update;
    }
}