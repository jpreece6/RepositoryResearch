using System;

namespace RepoConsole.Views
{
    internal interface IViewEmployee : IView
    {
        int Id { get; set; }
        string FirstName { get; set; }

        event EventHandler<EventArgs> GetAll;
    }
}