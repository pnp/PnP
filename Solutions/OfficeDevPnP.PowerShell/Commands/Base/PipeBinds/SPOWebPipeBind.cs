using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class SPOWebPipeBind
    {
        private Guid _id;
        private string _url;
        private SPOnlineWeb _spOnlineWeb;
        private Web _web;

        public SPOWebPipeBind()
        {
            _id = Guid.Empty;
            _url = string.Empty;
            _spOnlineWeb = null;
            _web = null;
        }

        public SPOWebPipeBind(Guid guid)
        {
            this._id = guid;
        }

        public SPOWebPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                this._url = id;
            }
        }

        public SPOWebPipeBind(SPOnlineWeb onlineWeb)
        {
            this._spOnlineWeb = onlineWeb;
        }

        public SPOWebPipeBind(Web web)
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
                if (_spOnlineWeb != null)
                {
                    return _spOnlineWeb.ContextObject;
                }
                else
                {
                    return _web;
                }
            }
        }


    }
}
