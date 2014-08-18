using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands.Base.PipeBinds
{
    public sealed class SPOFieldIdPipeBind
    {
        private string _name;
        private Guid _id;

        public SPOFieldIdPipeBind(Guid guid)
        {
            this._id = guid;
        }

        public SPOFieldIdPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                this._name = id;
            }
        }

        public SPOFieldIdPipeBind()
        {
            this._id = Guid.Empty;
            this._name = String.Empty;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
