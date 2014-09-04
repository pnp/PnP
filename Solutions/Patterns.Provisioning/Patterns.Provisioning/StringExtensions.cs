using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Contoso.Patterns.Provisioning
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a filename to a stream object
        /// </summary>
        /// <param name="xmlContent">content to convert to stream</param>
        /// <returns>Created stream</returns>
        public static Stream ToStream(this string xmlContent)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xmlContent);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Parses XML file and returns a class instance of type T
        /// </summary>
        /// <typeparam name="T">Type of the parsed XML class instance</typeparam>
        /// <param name="xmlContent">XML content as string</param>
        /// <returns>Parsed XML class instance of type T</returns>
        public static T ParseXML<T>(this string xmlContent) where T : class
        {
            var reader = XmlReader.Create(xmlContent.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }


    }
}
