using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands.Entities
{
    public class SiteEntity : EntityContextObject<Site>
    {

        public string ServerRelativeUrl { get; set; }
        public bool AllowSelfServiceUpgrade { get; set; }
        public bool AllowSelfServiceUpgradeEvaluation { get; set; }
        public int CompatibilityLevel { get; set; }
        public bool CanUpgrade { get; set; }
        public EventReceiverDefinitionCollection EventReceivers { get; set; }
        public FeatureCollection Features { get; set; }
        public WebEntity RootWeb { get; set; }

        public SiteEntity(Site site)
        {
            this._contextObject = site;

            var clientContext = _contextObject.Context;

            clientContext.Load(site,
                s => s.ServerRelativeUrl,
                s => s.AllowSelfServiceUpgrade,
                s => s.AllowSelfServiceUpgradeEvaluation,
                s => s.Features.IncludeWithDefaultProperties(f => f.DisplayName, f => f.DefinitionId),
                s => s.CanUpgrade,
                s => s.CompatibilityLevel,
                s => s.EventReceivers.IncludeWithDefaultProperties(e => e.ReceiverName, e => e.ReceiverUrl, e => e.ReceiverId, e => e.SequenceNumber),
                s => s.RootWeb);

            clientContext.ExecuteQuery();

            this.ServerRelativeUrl = site.ServerRelativeUrl;
            this.AllowSelfServiceUpgrade = site.AllowSelfServiceUpgrade;
            this.AllowSelfServiceUpgradeEvaluation = site.AllowSelfServiceUpgradeEvaluation;
            this.CompatibilityLevel = site.CompatibilityLevel;
            this.CanUpgrade = site.CanUpgrade;
            this.EventReceivers = site.EventReceivers;
            this.Features = site.Features;
            this.RootWeb = new WebEntity(site.RootWeb);
        }
    }
}
