using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands
{
    public class SPOContentType : SPOContextObject<ContentType>
    {
        private string _name;
        private string _id;
        private string _group;
        private string _description;

        public string Name { get { return _name; } }
        public string Id { get { return _id; } }
        public string Group { get { return _group; } }
        public string Description { get { return _description; } }


        public SPOContentType(ContentType ct)
        {
            _contextObject = ct;
            _name = ct.Name;
            _id = ct.StringId;
            _group = ct.Group;
            _description = ct.Description;
        }
    }
}
