using System;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class FieldPipeBind
    {
        private readonly string _name;
        private readonly Guid _id = Guid.Empty;
        private readonly Field _field;

        public FieldPipeBind(Guid guid)
        {
            _id = guid;
        }

        public FieldPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                _name = id;
            }
        }

        public FieldPipeBind(Field field)
        {
            _field = field;
        }

        public FieldPipeBind()
        {
            _id = Guid.Empty;
            _name = String.Empty;
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