using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    public class UpdateInputArgs<T> : EventArgs
    {
        public T Record { get; set; }

        public UpdateInputArgs(T record)
        {
            Record = record;
        } 
    }
}
