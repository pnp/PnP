using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PNP.Deployer.Common
{
    // =======================================================
    /// <author>
    /// Simon-Pierre Plante (sp.plante@gmail.com)
    /// </author>
    // =======================================================
    public static class XmlUtility
    {
        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Validates the specified XML file based on the specified XSD file
        /// </summary>
        /// <param name="xmlPath">The path of the XML file to validate</param>
        /// <param name="xsdPath">The path of the XSD file to be used for validation</param>
        // ===========================================================================================================
        public static void ValidateSchema(string xmlPath, string xsdPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            doc.Schemas.Add(null, xsdPath);
            doc.Validate(null);
        }


        // ===========================================================================================================
        /// <summary>
        /// Deserializes the specified XML file into the desired type of object based on the specified XML file
        /// </summary>
        /// <typeparam name="T">The type of object in which the XML needs to be deserialized</typeparam>
        /// <param name="xmlPath">The path of the XML file that needs to be deserialized</param>
        /// <returns>The deserialized XML in the form of the requested type (T)</returns>
        // ===========================================================================================================
        public static T DeserializeXmlFile<T>(string xmlPath)
        {
            T deserializedObject = default(T);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlTextReader reader = new XmlTextReader(xmlPath);
            deserializedObject = (T)serializer.Deserialize(reader);

            return deserializedObject;
        }


        // ===========================================================================================================
        /// <summary>
        /// Deserializes the specified XML file into the desired type of object based on the specified string reader
        /// </summary>
        /// <typeparam name="T">The type of object in which the XML needs to be deserialized</typeparam>
        /// <param name="xml">A <b>string</b> that contains the XML that needs to be deserialized</param>
        /// <returns>The deserialized XML in the form of the requested type (T)</returns>
        // ===========================================================================================================
        public static T DeserializeXml<T>(string xml)
        {
            T deserializedObject = default(T);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                deserializedObject = (T)serializer.Deserialize(stream);
            }
            
            return deserializedObject;
        }

        #endregion
    }
}
