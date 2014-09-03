using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class WebPipeBind
    {
        private Guid _id;
        private string _url;
        private Web _web;

        public WebPipeBind()
        {
            _id = Guid.Empty;
            _url = string.Empty;
            _web = null;
        }

        public WebPipeBind(Guid guid)
        {
            this._id = guid;
        }

        public WebPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                this._url = id;
            }
        }

        public WebPipeBind(Web web)
        {
            this._web = web;
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
