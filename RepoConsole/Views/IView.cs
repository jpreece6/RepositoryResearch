﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Views
{
    public interface IView
    {
        event EventHandler<EventArgs> Add;
        event EventHandler<EventArgs> Get;
        event EventHandler<EventArgs> GetAll;
        event EventHandler<EventArgs> Remove;

        void Show();
        void Show_Add();
        void Show_Get();
        void Show_GetAll();
        void Show_Remove();
    }
}