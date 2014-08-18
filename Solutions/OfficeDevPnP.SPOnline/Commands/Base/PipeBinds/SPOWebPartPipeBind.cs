using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands.Base.PipeBinds
{
    public class SPOWebPartPipeBind
    {
        private Guid _id;
        private string _title;



        public SPOWebPartPipeBind(Guid guid)
        {
            this._id = guid;
        }

        public SPOWebPartPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                this._title = id;
            }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Title { get { return _title; } }

        public SPOWebPartPipeBind()
        {
            this._id = Guid.Empty;
            this._title = string.Empty;
        }
    }
}
