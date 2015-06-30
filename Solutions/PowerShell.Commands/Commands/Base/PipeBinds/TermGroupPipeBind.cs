using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class TermGroupPipeBind
    {
        private readonly Guid _id = Guid.Empty;
        private readonly string _name = string.Empty;
        public TermGroupPipeBind(Guid guid)
        {
            _id = guid;
        }

        public TermGroupPipeBind()
        {
        }

        public TermGroupPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                _name = id;
            }
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
