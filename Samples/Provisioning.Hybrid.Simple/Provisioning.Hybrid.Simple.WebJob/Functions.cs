using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Configuration;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Provisioning.Hybrid.Simple.Common;

namespace Provisioning.Hybrid.Simple.WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger(Consts.StorageQueueName)] SiteCollectionRequest request, TextWriter log)
        {
            switch (request.TargetEnvironment.ToLowerInvariant())
            {
                case Consts.DeploymentTypeCloud:
                    ProcessCloudRequest(request, log);
                    break;
                case Consts.DeploymentTypeOnPremises:
                    ProcessOnPremRequest(request, log);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Perform call to on-prem
        /// </summary>
        /// <param name="request"></param>
        /// <param name="log"></param>
        private static void ProcessOnPremRequest(SiteCollectionRequest request, TextWriter log)
        {

            log.WriteLine(String.Format("Send request to create new site collection at {0}", DateTime.Now.ToLongTimeString()));

            string returnMessage = new ServiceBusMessageManager().SendSiteRequestMessage(request,
                                                            ConfigurationManager.AppSettings[Consts.ServiceBusNamespaceKey],
                                                            ConfigurationManager.AppSettings[Consts.ServiceBusSecretKey]);

            log.WriteLine(String.Format("Got followign message back '{0}' at {1}", returnMessage, DateTime.Now.ToLongTimeString()));

        }

        /// <summary>
        /// Ccloud request processing. Based on following project: 
        /// This should be located in cloud specific buisness logic component, but added here for simplicity reasons.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="log"></param>
        private static void ProcessCloudRequest(SiteCollectionRequest request, TextWriter log)
        {
            //get the base tenant admin urls
            string tenantStr = ConfigurationManager.AppSettings["Office365Tenant"];

            //create site collection using the Tenant object
            var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", Guid.NewGuid().ToString().Replace("-", ""));
            var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantStr));

            // Connecting to items using app only token
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                var tenant = new Tenant(adminContext);
                var properties = new SiteCreationProperties()
                {
                    Url = webUrl,
                    Owner = request.OwnerIdentifier,
                    Title = request.Title,
                    Template = "STS#0", // Create always team site and specialize after site collection is created as needed
                    StorageMaximumLevel = 100,
                    UserCodeMaximumLevel = 100
                };

                //start the SPO operation to create the site
                SpoOperation op = tenant.CreateSite(properties);
                adminContext.Load(tenant);
                adminContext.Load(op, i => i.IsComplete);
                adminContext.ExecuteQuery();

                //check if site creation operation is complete
                while (!op.IsComplete)
                {
                    //wait 15 seconds and try again
                    System.Threading.Thread.Sleep(15000);
                    op.RefreshLoad();
                    adminContext.ExecuteQuery();
                }
            }

            log.WriteLine(String.Format("Create new site collection with URL '{1}' at {0}", webUrl, DateTime.Now.ToLongTimeString()));
            ApplyTemplateForCreatedSiteCollection(webUrl, token, realm);
            log.WriteLine(String.Format("Applied custom branding to new site collection with URL '{1}' at {0}", webUrl, DateTime.Now.ToLongTimeString()));
        }

        /// <summary>
        /// Used to uplaod and apply branding to the newly created site. You could add new libraries and whatever needed.
        /// </summary>
        /// <param name="webUrl"></param>
        /// <param name="token"></param>
        /// <param name="realm"></param>
        private static void ApplyTemplateForCreatedSiteCollection(string webUrl, string token, string realm)
        {
            //get the new site collection
            var siteUri = new Uri(webUrl);
            token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;
            using (var newWebContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), token))
            {
                // Set the time out as high as possible
                newWebContext.RequestTimeout = int.MaxValue;

                // Let's first upload the custom theme to host web
                DeployThemeToWeb(newWebContext.Web, "Garage",
                                Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources/garagewhite.spcolor"),
                                string.Empty,
                                Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources/garagebg.jpg"),
                                "seattle.master");

                // Apply theme. We could upload a custom one as well or apply any other changes to newly created site
                SetThemeBasedOnName(newWebContext.Web, "Garage");

                // Upload the assets to host web
                UploadLogoToHostWeb(newWebContext.Web);

                // Set the properties accordingly
                // This is waiting for 16 version of the CSOM update. Should be there on Sep.
                newWebContext.Web.SiteLogoUrl = newWebContext.Web.ServerRelativeUrl + "/SiteAssets/garagelogo.png";
                newWebContext.Web.Update();
                newWebContext.Web.Context.ExecuteQuery();
            }
        }

        #region Business Logic

        /// <summary>
        /// Uploads site logo to host web
        /// </summary>
        /// <param name="web"></param>
        public static void UploadLogoToHostWeb(Web web)
        {
            // Instance to site assets
            List assetLibrary = web.Lists.GetByTitle("Site Assets");
            web.Context.Load(assetLibrary, l => l.RootFolder);

            // Get the path to the file which we are about to deploy
            string logoFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/garagelogo.png");

            // Use CSOM to uplaod the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(logoFile);
            newFile.Url = "garagelogo.png";
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Sets the theme for the just cretaed site 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="web"></param>
        /// <param name="rootWeb"></param>
        /// <param name="themeName"></param>
        public static void SetThemeBasedOnName(Web web, string themeName)
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
                //Set the properties for applying custom theme which was jus uplaoded
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
            newFile.Url = folder15.ServerRelativeUrl + "/" + System.IO.Path.GetFileName(sourceAddress);
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

        private static string URLCombine(string baseUrl, string relativeUrl)
        {
            if (baseUrl.Length == 0)
                return relativeUrl;
            if (relativeUrl.Length == 0)
                return baseUrl;
            return string.Format("{0}/{1}", baseUrl.TrimEnd(new char[] { '/', '\\' }), relativeUrl.TrimStart(new char[] { '/', '\\' }));
        }

        #endregion

    }
}
