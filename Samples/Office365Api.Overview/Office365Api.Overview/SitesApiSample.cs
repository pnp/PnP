using Microsoft.Office365.OAuth;
using Microsoft.Office365.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Overview
{
    static class  SitesApiSample
    {        
        //const string SharePointResourceId = "https://bertonline.sharepoint.com";

        public static string SharePointResourceId
        {
            get;
            set;
        }


        public static async Task<IEnumerable<IFileSystemItem>> GetDefaultDocumentFiles(string siteUrl)
        {
            var client = await EnsureClientCreated(siteUrl);
            client.Context.IgnoreMissingProperties = true;

            // Obtain files in default SharePoint folder
            var filesResults = await client.Files.ExecuteAsync();
            var files = filesResults.CurrentPage.OrderBy(e => e.Name);
            return files;
        }

        private static async Task<SharePointClient> EnsureClientCreated(string siteUrl)
        {
            Authenticator authenticator = new Authenticator();
            var authInfo = await authenticator.AuthenticateAsync(SharePointResourceId, ServiceIdentifierKind.Resource);

            // Create the SharePoint client proxy:
            return new SharePointClient(new Uri(string.Format("{0}/_api", siteUrl)), authInfo.GetAccessToken);
        }
        public static async Task SignOut()
        {
            await new Authenticator().LogoutAsync();
        }
    }
}
