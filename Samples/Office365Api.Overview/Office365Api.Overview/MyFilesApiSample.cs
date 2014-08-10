using Microsoft.Office365.OAuth;
using Microsoft.Office365.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Overview
{
    static class  MyFilesApiSample
    {
        const string MyFilesCapability = "MyFiles";

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
    
        private static async Task<SharePointClient> EnsureClientCreated()
        {
            Authenticator authenticator = new Authenticator();
            var authInfo = await authenticator.AuthenticateAsync(MyFilesCapability, ServiceIdentifierKind.Capability);

            // Create the MyFiles client proxy:
            return new SharePointClient(authInfo.ServiceUri, authInfo.GetAccessToken);
        }
        public static async Task SignOut()
        {
            await new Authenticator().LogoutAsync();
        }
    }
}
