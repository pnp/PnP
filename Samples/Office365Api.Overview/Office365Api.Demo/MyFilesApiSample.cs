using Microsoft.Office365.OAuth;
using Microsoft.Office365.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    static class  MyFilesApiSample
    {
        const string MyFilesCapability = "MyFiles";

        // Do not make static in Web apps; store it in session or in a cookie instead
        static string _lastLoggedInUser;
//        static DiscoveryContext _discoveryContext;

        public static DiscoveryContext _discoveryContext
        {
            get;
            set;
        }


        public static async Task<IEnumerable<IFileSystemItem>> GetMyFiles()
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            var filesResults = await client.Files.ExecuteAsync();
            var files = filesResults.CurrentPage.OrderBy(e => e.TimeLastModified);

            return files;
        }

        public static async Task<IEnumerable<IFileSystemItem>> GetMyFiles(string folder)
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            // Obtain files in passed folder
            var filesResults = await client.Files[folder].ToFolder().Children.ExecuteAsync();
            var files = filesResults.CurrentPage.OrderBy(e => e.Name);

            return files;
        }

        public static async Task UploadFile(string filePath, string folder)
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            string filename = System.IO.Path.GetFileName(filePath);
            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                if (!String.IsNullOrEmpty(folder))
                {
                    filename = string.Format("{0}/{1}", folder, filename);
                }

                var uploadedFile = await client.Files.AddAsync(filename, true, fileStream);                
            }
        }

        public static async Task<SharePointClient> EnsureClientCreated()
        {
            if (_discoveryContext == null)
            {
                _discoveryContext = await DiscoveryContext.CreateAsync();
            }

            var dcr = await _discoveryContext.DiscoverCapabilityAsync(MyFilesCapability);

            var ServiceResourceId = dcr.ServiceResourceId;
            var ServiceEndpointUri = dcr.ServiceEndpointUri;

            _lastLoggedInUser = dcr.UserId;

            // Create the MyFiles client proxy:
            return new SharePointClient(ServiceEndpointUri, async () =>
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
