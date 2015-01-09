using Microsoft.Office365.OAuth;
using Microsoft.Office365.SharePoint;
using SPOFileServices = Microsoft.Office365.SharePoint.FileServices;
using Microsoft.Office365.SharePoint.CoreServices;
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
        public static async Task<IEnumerable<SPOFileServices.IItem>> GetMyFiles()
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            var filesResults = await client.Files.ExecuteAsync();
            var files = filesResults.CurrentPage.OrderBy(e => e.DateTimeLastModified);

            return files;
        }

        public static async Task<IEnumerable<SPOFileServices.IItem>> GetMyFiles(string folderId)
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            // Obtain files in passed folder
            var filesResults = await client.Files.GetById(folderId).ToFolder().Children.ExecuteAsync();
            var files = filesResults.CurrentPage.OrderBy(e => e.Name);

            return files;
        }

        public static async Task<IEnumerable<SPOFileServices.IItem>> GetMyFolders()
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            var filesResults = await client.Files.ExecuteAsync();
            var folders = filesResults.CurrentPage.Where(e => e.Type == "Folder");

            return folders;
        }

        public static async Task UploadFile(string filePath)
        {
            await UploadFile(filePath, null);
        }

        public static async Task UploadFile(string filePath, string folderId)
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            string filename = System.IO.Path.GetFileName(filePath);
            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                SPOFileServices.File file = new SPOFileServices.File() { Name = filename };
                if (!String.IsNullOrEmpty(folderId))
                {
                    await client.Files.GetById(folderId).ToFolder().Children.AddItemAsync(file);
                }
                else
                {
                    await client.Files.AddItemAsync(file);
                }
                await client.Files.GetById(file.Id).ToFile().UploadAsync(fileStream);
            }
        }

        public static async Task<SharePointClient> EnsureClientCreated()
        {
            var discoveryResult = await DiscoveryAPISample.DiscoveryClient.DiscoverCapabilityAsync(Office365Capabilities.MyFiles.ToString());

            var ServiceResourceId = discoveryResult.ServiceResourceId;
            var ServiceEndpointUri = discoveryResult.ServiceEndpointUri;

            // Create the MyFiles client proxy:
            return new SharePointClient(
                ServiceEndpointUri,
                async () =>
                {
                    return await AuthenticationHelper.GetAccessTokenForServiceAsync(discoveryResult);
                });
        }
    }
}
