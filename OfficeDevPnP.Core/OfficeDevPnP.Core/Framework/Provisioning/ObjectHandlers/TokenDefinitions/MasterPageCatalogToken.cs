using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class MasterPageCatalogToken : TokenDefinition
    {
        public MasterPageCatalogToken(Web web)
            : base(web, "~masterpagecatalog")
        {
        }

        public override string GetReplaceValue()
        {
            var catalog = Web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
            Web.Context.Load(catalog, c => c.RootFolder.ServerRelativeUrl);
            Web.Context.ExecuteQueryRetry();
            return catalog.RootFolder.ServerRelativeUrl;
        }
    }
}