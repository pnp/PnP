using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Framework.Provisioning.Core.Configuration.Template
{
    /// <summary>
    /// Domain Object for Extensiblity Call out
    /// </summary>
    public class Provider
    {
        [XmlAttribute]
        public bool Enabled
        {
            get;
            set;
        }

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
