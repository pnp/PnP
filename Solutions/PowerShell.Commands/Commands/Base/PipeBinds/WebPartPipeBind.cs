using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public class WebPartPipeBind
    {
        private readonly Guid _id;
        private readonly string _title;

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
