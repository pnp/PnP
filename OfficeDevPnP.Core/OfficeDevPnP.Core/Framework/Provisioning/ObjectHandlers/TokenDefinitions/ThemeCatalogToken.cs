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
                var catalog = Web.GetCatalog((int) ListTemplateType.ThemeCatalog);
                Web.Context.Load(catalog, c => c.RootFolder.ServerRelativeUrl);
                Web.Context.ExecuteQueryRetry();
                CacheValue = catalog.RootFolder.ServerRelativeUrl;
            }
            return CacheValue;
        }
    }
}