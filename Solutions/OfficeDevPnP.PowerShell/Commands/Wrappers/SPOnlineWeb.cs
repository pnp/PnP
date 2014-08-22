using OfficeDevPnP.PowerShell.Core.Utils;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands
{
    public class SPOnlineWeb : SPOContextObject<Web>
    {
        private Guid _id;
        private string _title;
        private string _serverRelativeUrl;
        private string _url;
        private Lazy<FeatureCollection> _features;
        public FeatureCollection Features { get { return _features.Value; } }

        public string Title { get { return _title; } }

        public string Url { get { return _url; } }

        public string ServerRelativeUrl { get { return _serverRelativeUrl; } }

        public Guid Id { get { return _id; } }

        public SPOnlineWeb(Web web)
        {
            this._contextObject = web;
            this._id = web.Id;
            this._title = web.Title;
            this._serverRelativeUrl = web.ServerRelativeUrl;
            this._url = web.Url;

            this._features = new Lazy<FeatureCollection>(() =>
            {
                return this.ContextObject.Features.Load();
            });
        }
    }
}
