using Microsoft.VisualStudio.Shell;
using Perficient.Provisioning.VSTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Perficient.Provisioning.VSTools.Helpers
{
    public static class XmlHelpers
    {
        internal static T GetConfigFile<T>(string filepath, bool projectItem = true)
        {
            if (System.IO.File.Exists(filepath))
            {
                var dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
                var solutionItem = dte.Solution.FindProjectItem(filepath);

                if (solutionItem != null || projectItem == false)
                {
                    return XmlHelpers.DeserializeObject<T>(filepath);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Deserializes a file to the specified type
        /// </summary>
        internal static T DeserializeObject<T>(string filename)
        {
            T result;

            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                result = (T)ser.Deserialize(sr);
            }

            return result;
        }

        /// <summary>
        /// Serializes an item to the specied file
        /// </summary>
        internal static void SerializeObject(object source, string filename)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            XmlSerializer serializer = new XmlSerializer(source.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                XmlWriter writer = XmlWriter.Create(ms, settings);
                serializer.Serialize(writer, source);

                System.IO.File.WriteAllBytes(filename, ms.ToArray());
            }
        } 
    }
}
