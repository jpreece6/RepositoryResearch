using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Helpers
{
    /// <summary>
    /// Config reader allows us to read a
    /// XML settings file and query any elements from the root node.
    /// </summary>
    public class ConfigReader
    {
        private readonly XDocument _xmlDocument;

        /// <summary>
        /// Loads the XML file into memory
        /// </summary>
        /// <param name="filePath">Path to the XML file</param>
        public ConfigReader(string filePath)
        {
            _xmlDocument = XDocument.Load(filePath);
        }

        /// <summary>
        /// Returns all instances of a given element
        /// </summary>
        /// <param name="elementName">Name of the element to return</param>
        /// <returns>IList od XElements to further process</returns>
        public IList<XElement> GetAllInstancesOf(string elementName)
        {
            return _xmlDocument.Root?.Elements(elementName).ToList();
        }
    }
}
