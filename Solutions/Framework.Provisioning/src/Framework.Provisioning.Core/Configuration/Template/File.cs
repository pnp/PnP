using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Framework.Provisioning.Core.Configuration.Template
{
    public class File
    {
        [XmlAttribute]
        public string Src { get; set; }
        
        [XmlAttribute]
        public string TargetFolder { get; set; }
        
        [XmlAttribute]
        public bool UploadToDocumentLibary { get; set; }

    }
}
