using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands
{
    public class SPOCustomAction
    {
        public Guid Id { get; set; }
        public string Group { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public BasePermissions Rights { get; set; }
    }
}
