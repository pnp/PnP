using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class FieldPipeBind
    {
        private string _name;
        private Guid _id = Guid.Empty;
        private Field _field;

        public FieldPipeBind(Guid guid)
        {
            this._id = guid;
        }

        public FieldPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                this._name = id;
            }
        }

        public FieldPipeBind(Field field)
        {
            _field = field;
        }

        public FieldPipeBind()
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

        public Field Field
        {
            get { return _field; }
        }
    }
}