using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class GuidPipeBind
    {
        private Guid _id;

        public GuidPipeBind(Guid guid)
        {
            _id = guid;
        }

        public GuidPipeBind(string id)
        {
            _id = new Guid(id);
        }

        public Guid Id
        {
            get { return _id; }
        }

        public GuidPipeBind()
        {
            _id = Guid.Empty;
        }

    }
}
