using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PNP.Deployer.Common
{
    public static class DeployerUtility
    {
        #region Private Methods

        // ==================================================================================================================
        /// <summary>
        /// Returns whether the specified type is a simple type or not
        /// </summary>
        /// <param name="type">The <b>Type</b> that needs to be checked.</param>
        /// <returns><b>True</b> if it's a simple type, otherwise <b>False</b>.</returns>
        // ==================================================================================================================
        private static bool IsSimpleType(Type type)
        {
            return type.IsValueType ||
                   type.IsPrimitive ||
                   new Type[]
                   {
                       typeof(String),
                       typeof(Decimal),
                       typeof(DateTime),
                       typeof(DateTimeOffset),
                       typeof(TimeSpan),
                       typeof(Guid)
                   }.Contains(type) ||
                   Convert.GetTypeCode(type) != TypeCode.Object;
        }


        // ==================================================================================================================
        /// <summary>
        /// Checks whether the specified property has an invalid value or not
        /// </summary>
        /// <param name="property">A <b>PropertyInfo</b> object that needs to be verified</param>
        /// <param name="propertyValue">The property value</param>
        /// <returns><b>True</b> if the property is invaid, otherwise <b>False</b></returns>
        // ==================================================================================================================
        private static bool IsPropertyInvalid(PropertyInfo property, object propertyValue)
        {
            bool isRequired = Attribute.IsDefined(property, typeof(RequiredAttribute));
            return isRequired && (propertyValue == null || property.PropertyType == typeof(String) && String.IsNullOrEmpty(propertyValue.ToString()));
        }


        // ==================================================================================================================
        /// <summary>
        /// Validates the required attributes of a given serializable object and throws an exception if invalid
        /// </summary>
        /// <typeparam name="T">The type of object in which the XML has been deserialized.</typeparam>
        /// <param name="deserializedObject">The deserialized XML of type <b>T</b></param>
        // ==================================================================================================================
        private static void ValidateRequiredAttributes<T>(T deserializedObject)
        {
            IEnumerable listToValidate;

            // ------------------------------------------------
            // Makes sure the object is an Enumerable
            // ------------------------------------------------
            if(typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                listToValidate = deserializedObject as IEnumerable;
            }
            else
            {
                listToValidate = new List<T>()
                {
                    deserializedObject
                };
            }

            // ------------------------------------------------
            // Loops through the objects to validate
            // ------------------------------------------------
            foreach(var obj in listToValidate)
            {
                // ------------------------------------------------
                // Gets the objects properties
                // ------------------------------------------------
                var properties = obj.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0);

                foreach(PropertyInfo property in properties)
                {
                    // ------------------------------------------------
                    // Validates the required properties
                    // ------------------------------------------------
                    object propertyValue = property.GetValue(obj);

                    if(IsPropertyInvalid(property, propertyValue))
                    {
                        throw new Exception(string.Format("Property '{0}' from object type '{1}' is marked as required and is missing from the extensibility provider's schema.", property.Name, deserializedObject.GetType().Name));
                    }

                    // ------------------------------------------------
                    // Recursively validates the property if needed
                    // ------------------------------------------------
                    if (!IsSimpleType(property.PropertyType))
                    {
                        DeployerUtility.ValidateRequiredAttributes(propertyValue);
                    }
                }
            }
        }

        #endregion


        #region Public Methods

        // ==================================================================================================================
        /// <summary>
        /// Deserializes the given <b>XML</b> into an object of the specified type
        /// </summary>
        /// <typeparam name="T">The object type in which the XML should be deserialized into</typeparam>
        /// <param name="xml">The XML that should be deserialized into a <b>T</b> object</param>
        /// <returns>The deserialized XML as a <b>T</b> object</returns>
        // ==================================================================================================================
        public static T DeserializeProviderConfig<T>(string xml)
        {
            var config = XmlUtility.DeserializeXml<T>(xml);
            ValidateRequiredAttributes(config);
            return config;
        }

        #endregion
    }
}
