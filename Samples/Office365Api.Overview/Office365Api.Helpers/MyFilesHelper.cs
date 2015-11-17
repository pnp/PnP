using Microsoft.Office365.SharePoint;
using SPOFileServices = Microsoft.Office365.SharePoint.FileServices;
using Microsoft.Office365.SharePoint.CoreServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Helpers
{
    public class MyFilesHelper : BaseOffice365Helper
    {
        public MyFilesHelper(AuthenticationHelper authenticationHelper) : 
            base(authenticationHelper)
        {
        }

        public async Task<IEnumerable<SPOFileServices.IItem>> GetMyFiles()
        {
            var client = await this.AuthenticationHelper
                .EnsureSharePointClientCreatedAsync(
                Office365Capabilities.MyFiles.ToString());

            client.Context.IgnoreMissingProperties = true;

            List<SPOFileServices.IItem> files = new List<SPOFileServices.IItem>();

            var filesQuery = (from f in client.Files
                             select f).Take(50);

            var filesResults = await filesQuery.ExecuteAsync();

            if (filesResults != null)
            {
                do
                {
                    files.AddRange(filesResults.CurrentPage.Where(i => i.Type == "File"));
                    filesResults = await filesResults.GetNextPageAsync();
                }
                while (null != filesResults);
            }

            return files.OrderBy(f => f.DateTimeLastModified);
        }

        public async Task<IEnumerable<SPOFileServices.IItem>> GetMyFiles(string folderId)
        {
            var client = await this.AuthenticationHelper
                .EnsureSharePointClientCreatedAsync(
                Office365Capabilities.MyFiles.ToString());

            client.Context.IgnoreMissingProperties = true;

            List<SPOFileServices.IItem> files = new List<SPOFileServices.IItem>();

            // Obtain files in passed folder
            var filesQuery = (from f in client.Files.GetById(folderId).ToFolder().Children
                             select f).Take(50);

            var filesResults = await filesQuery.ExecuteAsync();

            if (filesResults != null)
            {
                do
                {
                    files.AddRange(filesResults.CurrentPage.Where(i => i.Type == "File"));
                    filesResults = await filesResults.GetNextPageAsync();
                }
                while (null != filesResults);
            }

            return files.OrderBy(f => f.Name);
        }

        public async Task<IEnumerable<SPOFileServices.IItem>> GetMyFolders()
        {
            var client = await this.AuthenticationHelper
                .EnsureSharePointClientCreatedAsync(
                Office365Capabilities.MyFiles.ToString());

            client.Context.IgnoreMissingProperties = true;

            List<SPOFileServices.IItem> folders = new List<SPOFileServices.IItem>();

            var foldersQuery = (from f in client.Files
                               select f).Take(50);

            var foldersResults = await foldersQuery.ExecuteAsync();

            if (foldersResults != null)
            {
                do
                {
                    folders.AddRange(foldersResults.CurrentPage.Where(i => i.Type == "Folder"));
                    foldersResults = await foldersResults.GetNextPageAsync();
                }
                while (null != foldersResults);
            }

            return folders;
        }

        public async Task UploadFile(string filePath)
        {
            await UploadFile(filePath, null);
        }

        public async Task UploadFile(string filePath, string folderId)
        {
            var client = await this.AuthenticationHelper
                .EnsureSharePointClientCreatedAsync(
                Office365Capabilities.MyFiles.ToString());

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
    }
}
