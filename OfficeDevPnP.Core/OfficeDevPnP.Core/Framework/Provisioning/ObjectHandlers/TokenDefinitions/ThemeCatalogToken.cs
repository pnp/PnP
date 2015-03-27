using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class ThemeCatalogToken : TokenDefinition
    {
        public ThemeCatalogToken(Web web)
            : base(web, "~themecatalog")
        {
        }

        public override string GetReplaceValue()
        {
            var catalog = Web.GetCatalog((int)ListTemplateType.ThemeCatalog);
            Web.Context.Load(catalog, c => c.RootFolder.ServerRelativeUrl);
            Web.Context.ExecuteQueryRetry();
            return catalog.RootFolder.ServerRelativeUrl;
        }
    }
}