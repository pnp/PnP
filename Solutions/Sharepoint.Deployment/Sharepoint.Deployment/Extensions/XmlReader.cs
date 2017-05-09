using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xml = System.Xml;

namespace SharePoint.Deployment {
    public static class XmlReader {
        public static int GetAttributeInt(this xml.XmlReader reader, string name, int defaultValue) {
            int returnValue;
            if (!int.TryParse(reader.GetAttribute(name), out returnValue)) {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        public static bool GetAttributeBool(this xml.XmlReader reader, string name, bool defaultValue) {
            bool returnValue;
            if (!bool.TryParse(reader.GetAttribute(name), out returnValue)) {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        public static T GetAttributeEnum<T>(this xml.XmlReader reader, string name, T defaultValue) where T : struct {
            T returnValue;
            if (!Enum.TryParse(reader.GetAttribute(name), true, out returnValue)) {
                returnValue = defaultValue;
            }
            return returnValue;
        }

    }
}
