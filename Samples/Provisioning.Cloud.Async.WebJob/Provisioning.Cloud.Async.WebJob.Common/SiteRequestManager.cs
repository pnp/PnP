using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Provisioning.Cloud.Async.WebJob.Common
{
    public class SiteRequestManager
    {
        #region CONSTANTS

        public const string StorageQueueName = "asyncsiterequests";

        #endregion

        /// <summary>
        /// Used to add new storage queue entry.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="siteUrl"></param>
        /// <param name="storageConnectionString"></param>
        public void AddConfigRequestToQueue(ProvisioningData provisioningData, string storageConnectionString)
        {
            CloudStorageAccount storageAccount =
                                CloudStorageAccount.Parse(storageConnectionString);

            // Get queue... create if does not exist.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue =
                queueClient.GetQueueReference(SiteRequestManager.StorageQueueName);
            queue.CreateIfNotExists();

            // Add entry to queue
            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(provisioningData)));

        }

        public bool SiteURLAlreadyInUse(ClientContext adminCtx, string fullSiteUrl)
        {
            var tenant = new Tenant(adminCtx);
            SPOSitePropertiesEnumerable spp = tenant.GetSiteProperties(0, true);
            adminCtx.Load(spp);
            adminCtx.ExecuteQuery();
            foreach (SiteProperties sp in spp)
            {
                if (sp.Url.ToLowerInvariant() == fullSiteUrl.ToLowerInvariant())
                {
                    return true;
                }
            }
            return false;
        }

        public string ProcessSiteCreationRequest(ClientContext adminCtx, ProvisioningData provisionData)
        {
            // Create the site collection
            //get the base tenant administration urls
            var webFullUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", provisionData.TenantName, "sites", provisionData.RequestData.Url);

            var tenant = new Tenant(adminCtx);
            var properties = new SiteCreationProperties()
            {
                Url = webFullUrl,
                Owner = provisionData.RequestData.Owner,
                Title = provisionData.RequestData.Title,
                Template = provisionData.RequestData.Template,
                TimeZoneId = provisionData.RequestData.TimeZoneId,
                Lcid = provisionData.RequestData.Lcid,
                StorageMaximumLevel = provisionData.RequestData.StorageMaximumLevel
            };

            //start the SPO operation to create the site
            SpoOperation op = tenant.CreateSite(properties);
            adminCtx.Load(tenant);
            adminCtx.Load(op, i => i.IsComplete);
            adminCtx.ExecuteQuery();

            //check if site creation operation is complete
            while (!op.IsComplete)
            {
                //wait 15 seconds and try again
                System.Threading.Thread.Sleep(15000);
                op.RefreshLoad();
                adminCtx.ExecuteQuery();
            }

            // Apply branding if theme information is provided
            if (!string.IsNullOrEmpty(provisionData.BrandingData.ThemeName))
            {
                ApplyTemplateForCreatedSiteCollection(webFullUrl, provisionData);
            }

            return webFullUrl;
        }

        /// <summary>
        /// Used to upload and apply branding to the newly created site. You could add new libraries and whatever needed.
        /// </summary>
        /// <param name="webUrl"></param>
        /// <param name="token"></param>
        /// <param name="realm"></param>
        private static void ApplyTemplateForCreatedSiteCollection(string webUrl, ProvisioningData provisionData)
        {
            //get the new site collection
            var siteUri = new Uri(webUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

            using (var ctx = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), token))
            {
                // Set the time out as high as possible
                ctx.RequestTimeout = Timeout.Infinite;

                // Let's first upload the custom theme to host web
                DeployThemeToWeb(ctx.Web,
                                provisionData.BrandingData.ThemeName,
                                provisionData.BrandingData.ThemeColorFilePath,
                                string.Empty,
                                provisionData.BrandingData.ThemeBackgrounImagePath,
                                provisionData.BrandingData.ThemeMasterPageName);

                // Apply theme. We could upload a custom one as well or apply any other changes to newly created site
                SetThemeBasedOnName(ctx.Web, "Garage");

                // Upload the assets to host web
                SetLogoToWeb(ctx.Web, provisionData.BrandingData.LogoImagePath);
            }
        }
        /// <summary>
        /// Sets the theme for the just created site 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="web"></param>
        /// <param name="rootWeb"></param>
        /// <param name="themeName"></param>
        private static void SetThemeBasedOnName(Web web, string themeName)
        {
            // Let's get instance to the composite look gallery
            List themeList = web.GetCatalog(124);
            web.Context.Load(themeList);
            web.Context.ExecuteQuery();

            CamlQuery query = new CamlQuery();
            string camlString = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Name' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";
            // Let's update the theme name accordingly
            camlString = string.Format(camlString, themeName);
            query.ViewXml = camlString;
            var found = themeList.GetItems(query);
            web.Context.Load(found);
            web.Context.ExecuteQuery();
            if (found.Count > 0)
            {
                Microsoft.SharePoint.Client.ListItem themeEntry = found[0];
                //Set the properties for applying custom theme which was just uploaded
                string spColorURL = null;
                if (themeEntry["ThemeUrl"] != null && themeEntry["ThemeUrl"].ToString().Length > 0)
                {
                    spColorURL = MakeAsRelativeUrl((themeEntry["ThemeUrl"] as FieldUrlValue).Url);
                }
                string spFontURL = null;
                if (themeEntry["FontSchemeUrl"] != null && themeEntry["FontSchemeUrl"].ToString().Length > 0)
                {
                    spFontURL = MakeAsRelativeUrl((themeEntry["FontSchemeUrl"] as FieldUrlValue).Url);
                }
                string backGroundImage = null;
                if (themeEntry["ImageUrl"] != null && themeEntry["ImageUrl"].ToString().Length > 0)
                {
                    backGroundImage = MakeAsRelativeUrl((themeEntry["ImageUrl"] as FieldUrlValue).Url);
                }

                // Set theme for demonstration
                web.ApplyTheme(spColorURL,
                                    spFontURL,
                                    backGroundImage,
                                    false);

                // Let's also update master page, if needed
                if (themeEntry["MasterPageUrl"] != null && themeEntry["MasterPageUrl"].ToString().Length > 0)
                {
                    web.MasterUrl = MakeAsRelativeUrl((themeEntry["MasterPageUrl"] as FieldUrlValue).Url); ;
                }

                web.Context.ExecuteQuery();
            }
        }


        public static void DeployThemeToWeb(Web web, string themeName, string colorFilePath, string fontFilePath, string backgroundImagePath, string masterPageName)
        {
            // Deploy files one by one to proper location
            if (!string.IsNullOrEmpty(colorFilePath))
            {
                DeployFileToThemeFolderSite(web, colorFilePath);
            }
            if (!string.IsNullOrEmpty(fontFilePath))
            {
                DeployFileToThemeFolderSite(web, fontFilePath);
            }
            if (!string.IsNullOrEmpty(backgroundImagePath))
            {
                DeployFileToThemeFolderSite(web, backgroundImagePath);
            }
            // Let's also add entry to the Theme catalog. This is not actually required, but provides visibility for the theme option, if manually changed
            AddNewThemeOptionToSite(web, themeName, colorFilePath, fontFilePath, backgroundImagePath, masterPageName);
        }


        public static void DeployFileToThemeFolderSite(Web web, string sourceAddress)
        {
            // Get the path to the file which we are about to deploy
            string file = sourceAddress;

            List themesList = web.GetCatalog(123);
            // get the theme list
            web.Context.Load(themesList);
            web.Context.ExecuteQuery();
            Folder rootfolder = themesList.RootFolder;
            web.Context.Load(rootfolder);
            web.Context.Load(rootfolder.Folders);
            web.Context.ExecuteQuery();
            Folder folder15 = rootfolder;
            foreach (Folder folder in rootfolder.Folders)
            {
                if (folder.Name == "15" || folder.Name == "16")
                {
                    folder15 = folder;
                    break;
                }
            }

            // Use CSOM to upload the file to the web
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(file);
            newFile.Url = folder15.ServerRelativeUrl + "/" + Path.GetFileName(sourceAddress);
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = folder15.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();
        }


        public static bool ThemeEntryExists(Web web, List themeList, string themeName)
        {

            CamlQuery query = new CamlQuery();
            string camlString = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Name' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";
            // Let's update the theme name accordingly
            camlString = string.Format(camlString, themeName);
            query.ViewXml = camlString;
            var found = themeList.GetItems(query);
            web.Context.Load(found);
            web.Context.ExecuteQuery();
            if (found.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static void AddNewThemeOptionToSite(Web web, string themeName, string colorFilePath, string fontFilePath, string backGroundPath, string masterPageName)
        {
            // Let's get instance to the composite look gallery
            List themesOverviewList = web.GetCatalog(124);
            web.Context.Load(themesOverviewList);
            web.Context.ExecuteQuery();
            // Do not add duplicate, if the theme is already there
            if (!ThemeEntryExists(web, themesOverviewList, themeName))
            {
                // if web information is not available, load it
                if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
                {
                    web.Context.Load(web);
                    web.Context.ExecuteQuery();
                }
                // Let's create new theme entry. Notice that theme selection is not available from UI in personal sites, so this is just for consistency sake
                ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                Microsoft.SharePoint.Client.ListItem item = themesOverviewList.AddItem(itemInfo);
                item["Name"] = themeName;
                item["Title"] = themeName;
                if (!string.IsNullOrEmpty(colorFilePath))
                {
                    item["ThemeUrl"] = URLCombine(web.ServerRelativeUrl, string.Format("/_catalogs/theme/15/{0}", System.IO.Path.GetFileName(colorFilePath)));
                }
                if (!string.IsNullOrEmpty(fontFilePath))
                {
                    item["FontSchemeUrl"] = URLCombine(web.ServerRelativeUrl, string.Format("/_catalogs/theme/15/{0}", System.IO.Path.GetFileName(fontFilePath)));
                }
                if (!string.IsNullOrEmpty(backGroundPath))
                {
                    item["ImageUrl"] = URLCombine(web.ServerRelativeUrl, string.Format("/_catalogs/theme/15/{0}", System.IO.Path.GetFileName(backGroundPath)));
                }
                // we use seattle master if anythign else is not set
                if (string.IsNullOrEmpty(masterPageName))
                {
                    item["MasterPageUrl"] = URLCombine(web.ServerRelativeUrl, "/_catalogs/masterpage/seattle.master");
                }
                else
                {
                    item["MasterPageUrl"] = URLCombine(web.ServerRelativeUrl, string.Format("/_catalogs/masterpage/{0}", Path.GetFileName(masterPageName)));
                }

                item["DisplayOrder"] = 11;
                item.Update();
                web.Context.ExecuteQuery();
            }

        }


        private static string MakeAsRelativeUrl(string urlToProcess)
        {
            Uri uri = new Uri(urlToProcess);
            return uri.AbsolutePath;
        }

        /// <summary>
        /// Uploads site logo to host web
        /// </summary>
        /// <param name="web"></param>
        public static void SetLogoToWeb(Web web, string logoFile)
        {
            // Instance to site assets
            List assetLibrary = web.Lists.GetByTitle("Site Assets");
            web.Context.Load(assetLibrary, l => l.RootFolder);

            // Use CSOM to upload the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(logoFile);
            newFile.Url = Path.GetFileName(logoFile);
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();

            // Load relative URL
            web.Context.Load(web, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            // Set the properties accordingly
            web.SiteLogoUrl = web.ServerRelativeUrl + "/SiteAssets/" + Path.GetFileName(logoFile);
            web.Update();
            web.Context.ExecuteQuery();
        }

        private static string URLCombine(string baseUrl, string relativeUrl)
        {
            if (baseUrl.Length == 0)
                return relativeUrl;
            if (relativeUrl.Length == 0)
                return baseUrl;
            return string.Format("{0}/{1}", baseUrl.TrimEnd(new char[] { '/', '\\' }), relativeUrl.TrimStart(new char[] { '/', '\\' }));
        }
    }


}
