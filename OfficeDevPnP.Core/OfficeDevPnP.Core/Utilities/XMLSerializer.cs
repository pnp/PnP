using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Utilities
{
    public static class XMLSerializer
    {
        #region Private Instance Members
        private static readonly Dictionary<Type, XmlSerializer> _XmlFormatter = new Dictionary<Type, XmlSerializer>();
        #endregion

        #region Constructors
        static XMLSerializer()
        {
           
        }
   
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the formatter for the specified type. If the formatter is not provided one will be created.
        /// </summary>
        private static XmlSerializer GetFormatter(Type objectType)
        {
            if (!_XmlFormatter.ContainsKey(objectType))
            {
                _XmlFormatter.Add(objectType, new XmlSerializer(objectType));
            }
            return _XmlFormatter[objectType];
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Deserialize an XDocuemnt to instance of an object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        public static T Deserialize<T>(XDocument xdoc)
        {
            XmlSerializer _xmlSerializer = new XmlSerializer(typeof(T));
            using (var _reader = xdoc.Root.CreateReader())
            {
                return (T)_xmlSerializer.Deserialize(_reader);
            }
        }

        /// <summary>
        /// Serializes an object instance to an XML represented string. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns>An string that represents the serialized object.</returns>
        public static string Serialize<T>(T objectToSerialize) where T : new()
        {
            return (Serialize(objectToSerialize, null));
        }

        /// <summary>
        /// Serializes an object instance to an XML represented string, providing custom namespace prefixes. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="ns"></param>
        /// <returns>An string that represents the serialized object.</returns>
        public static string Serialize<T>(T objectToSerialize, XmlSerializerNamespaces ns) where T : new()
        {
            using (StringWriter _sw = new StringWriter())
            {
                XmlSerializer xs = GetFormatter(objectToSerialize.GetType());
                if (ns != null)
                {
                    xs.Serialize(_sw, objectToSerialize, ns);
                }
                else
                {
                    xs.Serialize(_sw, objectToSerialize);
                }
                return _sw.ToString();
            }
        }

        /// <summary>
        /// Serializes an object instance to a stream. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns>An string that represents the serialized object.</returns>
        public static Stream SerializeToStream<T>(T objectToSerialize) where T : new()
        {
            return (SerializeToStream(objectToSerialize, null));
        }

        /// <summary>
        /// Serializes an object instance to a stream, providing custom namespace prefixes. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="ns"></param>
        /// <returns>An string that represents the serialized object.</returns>
        public static Stream SerializeToStream<T>(T objectToSerialize, XmlSerializerNamespaces ns) where T : new()
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xs = GetFormatter(objectToSerialize.GetType());
            if (ns != null)
            {
                xs.Serialize(stream, objectToSerialize, ns);
            }
            else
            {
                xs.Serialize(stream, objectToSerialize);
            }
            
            return stream;
        }

        /// <summary>
        /// Deserializes an XML string to an object instance
        /// </summary>
        /// <typeparam name="T">The Object Type to Desserialize</typeparam>
        /// <param name="xmlString">A string in XML format that representing the serialized object</param>
        /// <returns>An object instance of T</returns>
        public static T Deserialize<T>(string xmlString) where T : new()
        {
            if (!String.IsNullOrEmpty(xmlString))
            {
                using (StringReader _sr = new StringReader(xmlString))
                {
                    return (T)GetFormatter(typeof(T)).Deserialize(_sr);
                }
            }
            return default(T);
        }
        #endregion
    }
}
