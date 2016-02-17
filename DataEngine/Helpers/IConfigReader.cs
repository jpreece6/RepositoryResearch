using System.Collections.Generic;
using System.Xml.Linq;

namespace DataEngine.Helpers
{
    public interface IConfigReader
    {
        IList<XElement> GetAllInstancesOf(string elementName);
    }
}
