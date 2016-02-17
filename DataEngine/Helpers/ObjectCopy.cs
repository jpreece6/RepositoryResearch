using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using Remotion.Linq.Clauses.ResultOperators;

namespace DataEngine.Helpers
{
    public static class ObjectCopy
    {
        /// <summary>
        /// Copies all properties from one object to another.
        /// This allows us to effectivley remove the id from an entity
        /// </summary>
        /// <typeparam name="T">Entity type to copy</typeparam>
        /// <param name="inputObject">Source object</param>
        /// <param name="outputObject">Destination object</param>
        public static T Copy<T>(T inputObject)
        {
            Type inputObjType = inputObject.GetType();
            T outputObject = (T)Activator.CreateInstance(inputObjType); // Create a new object to copy to

            var inputProperties = inputObjType.GetProperties();

            foreach (var property in inputProperties)
            {
                if (property.Name == "Id") continue; // Skip over id as we want to remove it

                property.SetValue(outputObject, property.GetValue(inputObject));
            }

            return outputObject;
        }
    }
}
