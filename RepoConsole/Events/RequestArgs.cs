using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    public class RequestArgs<T> : EventArgs
    {
        public T Entity { get; set; }

        public RequestArgs(T entity)
        {
            Entity = entity;
        } 
    }
}
