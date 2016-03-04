using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Helpers
{
    public class ConfigReader
    {
        private readonly XDocument _xmlDocument;

        public ConfigReader(string filePath)
        {
            _xmlDocument = XDocument.Load(filePath);
        }

        public IList<XElement> GetAllInstancesOf(string elementName)
        {
            return _xmlDocument.Root?.Elements(elementName).ToList();
        }
    }
}
