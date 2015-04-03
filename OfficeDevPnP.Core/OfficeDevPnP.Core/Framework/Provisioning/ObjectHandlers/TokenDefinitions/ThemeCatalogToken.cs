using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class ThemeCatalogToken : TokenDefinition
    {
        public ThemeCatalogToken(Web web)
            : base(web, "~themecatalog","{themecatalog}")
        {
        }

        public override string GetReplaceValue()
        {
            if (CacheValue == null)
            {
                using (ClientContext cc = Web.Context.GetSiteCollectionContext())
                {
                    var catalog = cc.Web.GetCatalog((int)ListTemplateType.ThemeCatalog);
                    cc.Load(catalog, c => c.RootFolder.ServerRelativeUrl);
                    cc.ExecuteQueryRetry();
                    CacheValue = catalog.RootFolder.ServerRelativeUrl;
                }
            }
            return CacheValue;
        }
    }
}