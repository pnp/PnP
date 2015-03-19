using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning
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
