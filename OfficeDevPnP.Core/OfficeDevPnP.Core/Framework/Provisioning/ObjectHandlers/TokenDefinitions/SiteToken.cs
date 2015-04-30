using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class SiteToken : TokenDefinition
    {
        public SiteToken(Web web)
            : base(web, "~site", "{site}")
        {
        }

        public override string GetReplaceValue()
        {
            if (CacheValue == null)
            {
                Web.Context.Load(Web, w => w.ServerRelativeUrl);
                Web.Context.ExecuteQueryRetry();
                CacheValue = Web.ServerRelativeUrl;
                CacheValue = CacheValue.TrimEnd('/');
            }
            return CacheValue;
        }
    }
}