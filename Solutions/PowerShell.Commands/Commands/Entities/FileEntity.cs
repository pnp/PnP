using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Entities
{
    public class FileEntity
    {
        public string ServerRelativeUrl { get; set; }
        public string Title { get; set; }
        public override string ToString()
        {
            return this.ServerRelativeUrl;
        }

        public DateTime TimeCreated { get; set; }

        public DateTime TimeLastModified { get; set; }
    }

}
