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
        public bool IsLocal { get; set; }

        public UpdateInputArgs(T record)
        {
            Record = record;
        }

        public UpdateInputArgs(T record, bool isLocal)
        {
            Record = record;
            IsLocal = isLocal;
        }
    }
}
