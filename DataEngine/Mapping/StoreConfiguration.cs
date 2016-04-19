using System;
using FluentNHibernate.Automapping;

namespace DataEngine.Mapping
{
    public class StoreConfiguration : DefaultAutomappingConfiguration
    {
        // Ensures the ORM only maps classes from the entities folder
        public override bool ShouldMap(Type type)
        {
            return type.Namespace == "DataEngine.Entities";
        }
    }
}
