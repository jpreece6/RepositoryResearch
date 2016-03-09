using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpdateInputArgs<T> : EventArgs
    {
        public T Entity { get; set; }
        public bool IsLocal { get; set; }

        public UpdateInputArgs(T record)
        {
            Entity = record;
        }

        public UpdateInputArgs(T record, bool isLocal)
        {
            Entity = record;
            IsLocal = isLocal;
        }
    }
}
