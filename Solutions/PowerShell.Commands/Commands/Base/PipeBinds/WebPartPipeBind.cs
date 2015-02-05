using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public class WebPartPipeBind
    {
        private Guid _id;
        private string _title;

        public WebPartPipeBind(Guid guid)
        {
            _id = guid;
        }

        public WebPartPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                _title = id;
            }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Title { get { return _title; } }

        public WebPartPipeBind()
        {
            _id = Guid.Empty;
            _title = string.Empty;
        }
    }
}
