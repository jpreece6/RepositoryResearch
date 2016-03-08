using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    public class ObjectReturnedArgs<T> : EventArgs

    {
        public T RecordList { get; set; }
        public bool Update { get; set; }
        public bool IsLocal { get; set; }

        public ObjectReturnedArgs(T record)
        {
            RecordList = record;
        } 

        public ObjectReturnedArgs(T record, bool bUpdate)
        {
            RecordList = record;
            Update = bUpdate;
        }

        public ObjectReturnedArgs(T record, bool bUpdate, bool isLocal)
        {
            RecordList = record;
            Update = bUpdate;
            IsLocal = isLocal;
        }
    }
}
