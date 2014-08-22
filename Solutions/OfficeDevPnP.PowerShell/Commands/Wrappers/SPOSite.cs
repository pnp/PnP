using OfficeDevPnP.PowerShell.Core.Utils;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands
{
    public class SPOSite : SPOContextObject<Site>
    {

        private string _serverRelativeUrl;
        private bool _allowSelfServiceUpgrade;
        private bool _allowSelfServiceUpgradeEvaluation;
        private int _compatibilityLevel;
        private Lazy<bool> _canUpgrade;

        private Lazy<EventReceiverDefinitionCollection> _eventReceivers;
        private Lazy<FeatureCollection> _features;
        private Lazy<SPOnlineWeb> _rootWeb;

        public bool CanUpgrade { get { return _canUpgrade.Value; } }

        public int CompatibilityLevel { get; set; }

        public EventReceiverDefinitionCollection EventReceivers { get { return _eventReceivers.Value; } }

        public FeatureCollection Features { get { return _features.Value; } }

        public string ServerRelativeUrl { get { return _serverRelativeUrl; } }

        public SPOnlineWeb RootWeb { get { return _rootWeb.Value; } }

        public SPOSite(Site site)
        {
            this._contextObject = site;

            this._serverRelativeUrl = site.ServerRelativeUrl;

            this._allowSelfServiceUpgrade = site.AllowSelfServiceUpgrade;
            this._allowSelfServiceUpgradeEvaluation = site.AllowSelfServiceUpgradeEvaluation;

            this._compatibilityLevel = site.CompatibilityLevel;

            this._canUpgrade = new Lazy<Boolean>(() =>
            {
                this.ContextObject.Context.Load(this.ContextObject, s => s.CanUpgrade);
                this.ContextObject.Context.ExecuteQuery();
                return this.ContextObject.CanUpgrade;
            });

            this._eventReceivers = new Lazy<EventReceiverDefinitionCollection>(() =>
            {
                return this.ContextObject.EventReceivers.Load();
            });

            this._features = new Lazy<FeatureCollection>(() =>
            {
                return this.ContextObject.Features.Load();
            });

            this._rootWeb = new Lazy<SPOnlineWeb>(() =>
            {
                Web web = this.ContextObject.RootWeb;
                this.ContextObject.Context.Load(web);
                this.ContextObject.Context.ExecuteQuery();
                return new SPOnlineWeb(web);
            });
        }
    }
}
