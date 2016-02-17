using System;
using FluentNHibernate.Automapping;

namespace DataEngine.Mapping
{
    public class StoreConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.Namespace == "DataEngine.Entities";
        }
    }
}
