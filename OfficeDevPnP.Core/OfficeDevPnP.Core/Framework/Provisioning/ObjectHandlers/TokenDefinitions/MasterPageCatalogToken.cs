using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class MasterPageCatalogToken : TokenDefinition
    {
        public MasterPageCatalogToken(Web web)
            : base(web, "~masterpagecatalog","{masterpagecatalog}")
        {
        }

        public override string GetReplaceValue()
        {
            if (this.CacheValue == null)
            {
                var catalog = Web.GetCatalog((int) ListTemplateType.MasterPageCatalog);
                Web.Context.Load(catalog, c => c.RootFolder.ServerRelativeUrl);
                Web.Context.ExecuteQueryRetry();
                CacheValue = catalog.RootFolder.ServerRelativeUrl;
            }
            return CacheValue;
        }
    }
}