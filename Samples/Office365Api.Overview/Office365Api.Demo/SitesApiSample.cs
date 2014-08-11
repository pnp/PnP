using Microsoft.Office365.OAuth;
using Microsoft.Office365.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    static class  SitesApiSample
    {        
        //const string SharePointResourceId = "https://bertonline.sharepoint.com";
        
        // Do not make static in Web apps; store it in session or in a cookie instead
        static string _lastLoggedInUser;
        //static DiscoveryContext _discoveryContext;
        public static DiscoveryContext _discoveryContext
        {
            get;
            set;
        }

        public static string ServiceResourceId
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

        //private static async Task<SharePointClient> EnsureClientCreated(string siteUrl)
        //{
        //    Authenticator authenticator = new Authenticator();
        //    var authInfo = await authenticator.AuthenticateAsync(SharePointResourceId, ServiceIdentifierKind.Resource);

        //    // Create the SharePoint client proxy:
        //    return new SharePointClient(new Uri(string.Format("{0}/_api", siteUrl)), authInfo.GetAccessToken);
        //}

        public static async Task<SharePointClient> EnsureClientCreated(string siteUrl)
        {
            if (_discoveryContext == null)
            {
                _discoveryContext = await DiscoveryContext.CreateAsync();
            }

            var dcr = await _discoveryContext.DiscoverResourceAsync(ServiceResourceId);

            _lastLoggedInUser = dcr.UserId;

            return new SharePointClient(new Uri(string.Format("{0}/_api", siteUrl)), async () =>
            {
                return (await _discoveryContext.AuthenticationContext.AcquireTokenSilentAsync(ServiceResourceId, _discoveryContext.AppIdentity.ClientId, new Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifier(dcr.UserId, Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifierType.UniqueId))).AccessToken;
            });
        }

        public static async Task SignOut()
        {
            if (string.IsNullOrEmpty(_lastLoggedInUser))
            {
                return;
            }

            if (_discoveryContext == null)
            {
                _discoveryContext = await DiscoveryContext.CreateAsync();
            }

            await _discoveryContext.LogoutAsync(_lastLoggedInUser);
        }
    }
}
