using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    /// <summary>
    /// Contains a list of records returned from a GETALL search
    /// or a search that returns more than one result
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    public class ObjectGetAllReturnedArgs<T> : EventArgs
    {
        /// <summary>
        /// IList of Entities
        /// Holds a list of entites that have been returned
        /// </summary>
        public T RecordList { get; set; }

        /// <summary>
        /// IsLocal flag allows us to determine if connection state has changed
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// Creates a new ObjectReturnedArgs instance with list only
        /// </summary>
        /// <param name="record">IList of entities</param>
        public ObjectGetAllReturnedArgs(T record)
        {
            RecordList = record;
        }

        /// <summary>
        /// Creates a new ObjectReturnedArgs instance with a list of entities
        /// and returns the connection state
        /// </summary>
        /// <param name="record">IList of entities</param>
        /// <param name="isLocal">Bool connection state</param>
        public ObjectGetAllReturnedArgs(T record, bool isLocal)
        {
            RecordList = record;
            IsLocal = isLocal;
        }
    }
}
