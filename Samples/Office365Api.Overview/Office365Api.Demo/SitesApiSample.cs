using Microsoft.Office365.OAuth;
using Microsoft.Office365.SharePoint;
using Microsoft.Office365.SharePoint.CoreServices;
using Microsoft.Office365.SharePoint.FileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    static class  SitesApiSample
    {
        public static async Task<IEnumerable<IItem>> GetDefaultDocumentFiles(string tenantUri, string siteUri)
        {
            var client = EnsureClientCreated(tenantUri, siteUri);
            client.Context.IgnoreMissingProperties = true;

            List<IItem> files = new List<IItem>();

            // ***********************************************************
            // Note from @PaoloPia: To not stress the server, limit the
            // the query to no more than 50 email items
            // ***********************************************************

            // ***********************************************************
            // IMPORTANT: This method always fails ... we need to 
            // figure out why! Is it a kind of bug? Using Fiddler the
            // JSON response comes out properly, then is the OData client
            // that fails while materializing the object, saying that
            // it cannot create an abstract class!
            // ***********************************************************
            var tmp = await client.Files.ExecuteAsync();

            var query = (from f in client.Files
                         orderby f.DateTimeLastModified descending
                         select f).Take(50);

            var filesResults = await query.ExecuteAsync(); 

            do
            {
                files.AddRange(filesResults.CurrentPage);
                filesResults = await filesResults.GetNextPageAsync();
            }
            while (filesResults.MorePagesAvailable);

            return files;
        }

        public static SharePointClient EnsureClientCreated(string tenantUri, string siteUri)
        {
            // Create the MyFiles client proxy:
            return new SharePointClient(
                new Uri(string.Format("{0}{1}_api", siteUri, siteUri.EndsWith("/") ? String.Empty : "/")),
                async () =>
                {
                    return await AuthenticationHelper.GetAccessTokenForServiceAsync(tenantUri);
                });
        }
    }
}
