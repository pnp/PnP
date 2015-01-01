using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OD4B.Configuration.Async.Common
{
    public class SiteModificationManager
    {
        #region Const

        private const string JSLocationFolderName = "OneDriveCustomization";
        private const string JSFileName = "OneDriveConfiguration.js";
        private const int BrandingVersion = 1;
        private const string OneDriveMarkerBagID = "Contoso_OneDriveVersion";
        private const string OneDriveCustomActionID = "OneDriveCustomJS";
        public const string StorageQueueName = "od4bconfig";

        #endregion

        public void AddConfigRequestToQueue(
                    string account, string siteUrl, string storageConnectionString)
        {
            CloudStorageAccount storageAccount = 
                                CloudStorageAccount.Parse(storageConnectionString);

            // Get queue... create if does not exist.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = 
                queueClient.GetQueueReference(SiteModificationManager.StorageQueueName);
            queue.CreateIfNotExists();

            // Pass in data for modification
            var newSiteConfigRequest = new SiteModificationData()
            {
                AccountId = account,
                SiteUrl = siteUrl
            };

            // Add entry to queue
            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(newSiteConfigRequest)));

        }

public void ApplySiteConfiguration(ClientContext ctx, 
                                    SiteModificationConfig config)
{
    // Check current site configuration status - is it already in right version?
    if (ctx.Web.GetPropertyBagValueInt(
        SiteModificationManager.OneDriveMarkerBagID, 0) 
        < SiteModificationManager.BrandingVersion)
    {
        // Set the time out as high as possible for needed operations, just in case
        // In this case needed operations shold not take that long, so we shold be fine
        ctx.RequestTimeout = Timeout.Infinite;

        // Set JavaScript policy message, refresh if existed on site already
        UploadJSToRootSiteCollection(ctx, config.SiteUrl, config.JSFile);
        ctx.Web.DeleteJsLink("OneDriveCustomJS");
        ctx.Web.AddJsLink("OneDriveCustomJS", BuildJavaScriptUrl(config.SiteUrl));

        // Upload theme files to theme gallery
        ctx.Web.UploadThemeFile(config.ThemeColorFile);
        ctx.Web.UploadThemeFile(config.ThemeBGFile);
        // Set theme pointing to right files directly
        ctx.Load(ctx.Web, w => w.AllProperties, w => w.ServerRelativeUrl);
        ctx.ExecuteQuery();
        ctx.Web.ApplyTheme(ctx.Web.ServerRelativeUrl + "/_catalogs/theme/15/" + 
                            Path.GetFileName(config.ThemeColorFile),
                            null,
                            ctx.Web.ServerRelativeUrl + "/_catalogs/theme/15/" + 
                            Path.GetFileName(config.ThemeBGFile),
                            true);
        ctx.ExecuteQuery();

        // Save current branding applied indicator to site
        ctx.Web.SetPropertyBagValue(
                SiteModificationManager.OneDriveMarkerBagID, 
                SiteModificationManager.BrandingVersion);
    }
}

        /// <summary>
        /// Just to build the JS path which can be then pointed to the OneDrive site.
        /// </summary>
        /// <returns></returns>
        public string BuildJavaScriptUrl(string siteUrl)
        {
            // Solve root site collection URL
            Uri url = new Uri(siteUrl);
            string scenarioUrl = String.Format("{0}://{1}:{2}/{3}", 
                                 url.Scheme, url.DnsSafeHost, 
                                 url.Port, JSLocationFolderName);
            // Unique rev generated each time JS is added, so that we force browsers to refresh JS file wiht latest version
            string revision = Guid.NewGuid().ToString().Replace("-", "");
            return string.Format("{0}/{1}?rev={2}", scenarioUrl, JSFileName, revision);
        }

        private void UploadJSToRootSiteCollection(ClientContext ctx, string siteUrl, string jsLocation)
        {
            // Solve root site collection URL
            Uri url = new Uri(siteUrl);
            Uri rootUrl = new Uri(String.Format("{0}://{1}:{2}", url.Scheme, url.DnsSafeHost, url.Port));

            //Open connection to root site collection
            string realm = TokenHelper.GetRealmFromTargetUrl(rootUrl);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, rootUrl.Authority, realm).AccessToken;
            using (var ctxRoot = TokenHelper.GetClientContextWithAccessToken(rootUrl.ToString(), token))
            {
                // Upload JavaScript to Style    
                Folder jsFolder;
                string folder = JSLocationFolderName;

                if (!ctxRoot.Web.FolderExists(folder))
                {
                    jsFolder = ctxRoot.Web.Folders.Add(folder);

                }
                else
                {
                    jsFolder = ctxRoot.Web.Folders.GetByUrl(folder);
                }
                // Load Folder instance
                ctxRoot.Load(jsFolder);
                ctxRoot.ExecuteQuery();

                // Uplaod JS file to folder
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = System.IO.File.ReadAllBytes(jsLocation);
                newFile.Url = UrlUtility.EnsureTrailingSlash(jsFolder.ServerRelativeUrl) + JSFileName;
                // Right now we override this in each upload, could be optimized
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File uploadFile = jsFolder.Files.Add(newFile);
                ctxRoot.Load(uploadFile);
                ctxRoot.ExecuteQuery();
            }


        }

        public void ResetSiteConfiguration(ClientContext ctx)
        {
            // Set the time out as high as possible
            ctx.RequestTimeout = int.MaxValue;

            // Delete JS config
            ctx.Web.DeleteJsLink("OneDriveCustomJS");

            // Perform needed modifications to site. Set theme to site.
            ctx.Web.SetComposedLookByUrl("Office");

            // Set branding version as 0
            ctx.Web.SetPropertyBagValue(SiteModificationManager.OneDriveMarkerBagID, 0);
        }
    }
}
