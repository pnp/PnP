using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class SiteCollectionToken : TokenDefinition
    {
        public SiteCollectionToken(Web web)
            : base(web, "~sitecollection")
        {
        }

        public override string GetReplaceValue()
        {
            var context = this.Web.Context as ClientContext;
            var site = context.Site;
            context.Load(site, s => s.RootWeb.ServerRelativeUrl);
            context.ExecuteQueryRetry();
            return site.RootWeb.ServerRelativeUrl;
        }
    }
}