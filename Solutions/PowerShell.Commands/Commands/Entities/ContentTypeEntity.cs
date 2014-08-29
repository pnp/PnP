using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Entities
{
    public class ContentTypeEntity : EntityContextObject<ContentType>
    {
        private string _name;
        private string _id;
        private string _group;
        private string _description;

        public string Name { get { return _name; } }
        public string Id { get { return _id; } }
        public string Group { get { return _group; } }
        public string Description { get { return _description; } }


        public ContentTypeEntity(ContentType ct)
        {
            _contextObject = ct;
            _name = ct.Name;
            if(!ct.IsObjectPropertyInstantiated("StringId"))
            {
                ct.Context.Load(ct, c => c.StringId);
                ct.Context.ExecuteQuery();
            }
            _id = ct.StringId;
            _group = ct.Group;
            _description = ct.Description;
        }
    }
}
