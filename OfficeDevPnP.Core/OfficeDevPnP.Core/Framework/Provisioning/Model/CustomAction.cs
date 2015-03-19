using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning
{
    /// <summary>
    /// Domain Object for custom action.
    /// </summary>
    public partial class CustomAction
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlAttribute]
        public string Group { get; set; }
        [XmlAttribute]
        public string Location { get; set; }
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public int Sequence { get; set; }
        [XmlAttribute]
        public int Rights { get; set; }   
        [XmlAttribute]
        public string Url { get; set; }
        [XmlAttribute]
        public bool Enabled { get; set; }
        [XmlAttribute]
        public string ScriptBlock { get; set; }
        [XmlAttribute]
        public string ImageUrl { get; set; }
        [XmlAttribute]
        public string ScriptSrc { get; set; }
    }
}
