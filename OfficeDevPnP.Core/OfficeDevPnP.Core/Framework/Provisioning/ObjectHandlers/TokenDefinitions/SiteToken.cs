using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class SiteToken : TokenDefinition
    {
        public SiteToken(Web web)
            : base(web, "~site")
        {
        }

        public override string GetReplaceValue()
        {
            Web.Context.Load(Web, w => w.ServerRelativeUrl);
            Web.Context.ExecuteQueryRetry();
            return Web.ServerRelativeUrl;
        }
    }
}