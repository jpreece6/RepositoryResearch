﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoConsole.Events
{
    /// <summary>
    /// Contains arguments needed when an object has been returned from
    /// the database an needs further processing. After a GET operation
    /// update = false and will print the contents of RecordList to the screen.
    /// After an EDIT operation update = true and the contents will be modified and returned to
    /// the current session database
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    public class ObjectReturnedArgs<T> : EventArgs
    {
        /// <summary>
        /// IList of Entities
        /// Holds a list of entites that have been returned
        /// </summary>
        public T RecordList { get; set; }

        /// <summary>
        /// Update flag tells the program we want to update the record in RecordList
        /// </summary>
        public bool Update { get; set; }

        /// <summary>
        /// IsLocal flag allows us to determine if connection state has changed
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// Creates a new ObjectReturnedArgs instance with list only
        /// </summary>
        /// <param name="record">IList of entities</param>
        public ObjectReturnedArgs(T record)
        {
            RecordList = record;
        }

        /// <summary>
        /// Creates a new ObjectReturnedArgs instance with a list of entities
        /// and a flag if we need to update
        /// </summary>
        /// <param name="record">IList of entities</param>
        /// <param name="bUpdate">Bool update the stored entity</param>
        public ObjectReturnedArgs(T record, bool bUpdate)
        {
            RecordList = record;
            Update = bUpdate;
        }

        /// <summary>
        /// Creates a new ObjectReturnedArgs instance with a list of entities
        /// and a flag if we need to update and returns the connection state
        /// </summary>
        /// <param name="record">IList of entities</param>
        /// <param name="bUpdate">Bool update the stored entity</param>
        /// <param name="isLocal">Bool connection state</param>
        public ObjectReturnedArgs(T record, bool bUpdate, bool isLocal)
        {
            RecordList = record;
            Update = bUpdate;
            IsLocal = isLocal;
        }
    }
}
