using System;

namespace Helpers
{
    /// <summary>
    /// Copies all data from one object to another. nHibernate does not allow us
    /// to modify the ID property of entities as this property is set by the database's
    /// auto increment feature. However for us to be able to sync local records to the remote database
    /// we need to 'reset' the property ID so that the remote database can use its auto increment to update this value.
    /// To do this we copy all data from one entity to another exluding the ID property so remote DB can re assign this value.
    /// </summary>
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
