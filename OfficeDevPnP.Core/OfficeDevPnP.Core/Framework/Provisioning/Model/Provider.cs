using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for Extensiblity Call out
    /// </summary>
    public class Provider
    {
        [XmlAttribute]
        public string Assembly
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Type
        {
            get;
            set;
        }
        
        [XmlElement("Configuration")]
        public string Configuration { get; set; }
    }
}
