using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for Extensiblity Call out
    /// </summary>
    public class Provider
    {
        public bool Enabled
        {
            get;
            set;
        }

        public string Assembly
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }
        
        public string Configuration { get; set; }
    }
}
