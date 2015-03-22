using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class File
    {
        public string Src { get; set; }
        
        public string Folder { get; set; }

        public bool Overwrite { get; set; }
    }
}
