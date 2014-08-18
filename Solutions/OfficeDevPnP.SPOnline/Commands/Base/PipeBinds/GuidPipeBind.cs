using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands.Base.PipeBinds
{
    public sealed class GuidPipeBind
    {
        private Guid _id;

        public GuidPipeBind(Guid guid)
        {
            this._id = guid;
        }

        public GuidPipeBind(string id)
        {
            this._id = new Guid(id);
        }

        public Guid Id
        {
            get { return _id; }
        }

        public GuidPipeBind()
        {
            this._id = Guid.Empty;
        }

    }
}
