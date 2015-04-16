using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class WebPipeBind
    {
        private readonly Guid _id;
        private readonly string _url;
        private readonly Web _web;

        public WebPipeBind()
        {
            _id = Guid.Empty;
            _url = string.Empty;
            _web = null;
        }

        public WebPipeBind(Guid guid)
        {
            _id = guid;
        }

        public WebPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                _url = id;
            }
        }

        public WebPipeBind(Web web)
        {
            _web = web;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Url
        {
            get { return _url; }
        }

        public Web Web
        {
            get
            {
                return _web;
            }
        }


    }
}
