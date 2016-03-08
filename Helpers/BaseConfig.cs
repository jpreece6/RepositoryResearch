using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Helpers
{
    /// <summary>
    /// Type safe enum for connection method
    /// http://stackoverflow.com/questions/424366/c-sharp-string-enums?page=1&tab=votes#tab-top
    /// </summary>
    public sealed class ConnectionMethod
    {

        private readonly string _name;
        private readonly int _value;

        private static readonly Dictionary<string, ConnectionMethod> instance = new Dictionary<string, ConnectionMethod>();
        public static readonly ConnectionMethod MsSql2012 = new ConnectionMethod(1, "MsSQL2012");
        public static readonly ConnectionMethod JetProvider = new ConnectionMethod(2, "JetProvider");

        private ConnectionMethod(int value, string name)
        {
            instance[name] = this;
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return _name;
        }

        public static explicit operator ConnectionMethod(string str)
        {
            ConnectionMethod result;
            if (instance.TryGetValue(str, out result))
                return result;

            throw new InvalidCastException();
        }

    }

    public static class BaseConfig
    {
        public static IList<XElement> Sources;
        public static bool IsLocal = false;
    }
}
