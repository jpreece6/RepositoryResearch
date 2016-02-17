using System;

namespace DataEngine.RepositoryCore
{
    public class RepositoryKeyValue : IRepositoryKeyValue
    {
        /*private readonly IKeyDataContext _context;

        public RepositoryKeyValue(IKeyDataContext context)
        {
            _context = context;
        }*/

        /// <summary>
        /// Add a new value to the repository
        /// </summary>
        /// <typeparam name="T">Data type to store</typeparam>
        /// <param name="key">Key value of the data</param>
        /// <param name="tValue">Value to store</param>
        public void Save<T>(string key, T tValue)
        {
            //_context.Add<T>(key, tValue);
        }

        /// <summary>
        /// Retrieves a value from the repository
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="key">Key to retrieve</param>
        /// <returns>Value</returns>
        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a value from the repository
        /// </summary>
        /// <param name="key">Key to remove</param>
        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}
