using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class SiteCollectionTermStoreIdToken : TokenDefinition
    {
        public SiteCollectionTermStoreIdToken(Web web)
            : base(web, "~sitecollectiontermstoreid", "{sitecollectiontermstoreid}")
        {
        }

        public override string GetReplaceValue()
        {
            if (CacheValue == null)
            {
                TaxonomySession session = TaxonomySession.GetTaxonomySession(Web.Context);
                var termStore = session.GetDefaultSiteCollectionTermStore();
                Web.Context.Load(termStore, t => t.Id);
                Web.Context.ExecuteQueryRetry();
                CacheValue = termStore.Id.ToString();
            }
            return CacheValue;
        }
    }
}