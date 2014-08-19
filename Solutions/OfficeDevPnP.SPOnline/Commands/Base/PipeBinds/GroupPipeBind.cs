using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands.Base.PipeBinds
{
    public sealed class GroupPipeBind
    {
        private int _id = -1;
        private Group _group;
        private string _name;
        public int Id
        {
            get
            {
                return _id;
            }
        }
        public Group Group
        {
            get
            {
                return _group;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public GroupPipeBind(int id)
        {
            this._id = id;
        }

        public GroupPipeBind(Group group)
        {
            this._group = group;
        }

        public GroupPipeBind(string name)
        {
            this._name = name;
        }

    }
}
