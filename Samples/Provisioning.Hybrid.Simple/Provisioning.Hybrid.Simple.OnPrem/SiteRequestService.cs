using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Provisioning.Hybrid.Simple.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Provisioning.Hybrid.Simple.OnPrem
{
    class SiteRequestService : ISiteRequest
    {

        #region Service Interface

        /// <summary>
        /// Actual main method for the site collection creation.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ProvisionSiteCollection(Common.SiteCollectionRequest request)
        {
            // Output site collection creation request to console
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(string.Format("Got request to process site with Title of '{0}' and template as '{1}' at {2}.", request.Title, request.Template, DateTime.Now.ToLongTimeString()));

            try
            {
                // Process the actual request
                string returnValue =  ProcessSiteCreationRequest(request);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format("New site collection created to address '{0}' at {1}.", returnValue, DateTime.Now.ToLongTimeString()));

                return returnValue;
            }
            catch (Exception ex)
            {
                // Output exception to console and return to caller
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Site collection creation failed: '{0}'.", ex.ToString()));

                return ex.ToString();
            }
        }

        /// <summary>
        ///  Tester method for remote connectivity verification
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string SendMessage(string message)
        {
            // Testing the process
            Console.ForegroundColor = ConsoleColor.Gray;
            string newMessage = string.Format("Got message: '{0}' at {1}.", message, DateTime.Now.ToLongTimeString());
            Console.WriteLine(newMessage);
            return newMessage;
        }

        #endregion

        #region business logic

        /// <summary>
        /// Actual business logic to create the site collections.
        /// See more details on the requirements for on-premises from following blog post:
        /// http://blogs.msdn.com/b/vesku/archive/2014/06/09/provisioning-site-collections-using-sp-app-model-in-on-premises-with-just-csom.aspx
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string ProcessSiteCreationRequest(SiteCollectionRequest request)
        {
            // Get the base tenant admin url needed for site collection creation
            string tenantStr = ConfigurationManager.AppSettings[Consts.AdminSiteCollectionUrl];

            // Resolve root site collection URL from host web.
            string rootSiteUrl = ConfigurationManager.AppSettings[Consts.LeadingURLForSiteCollections];

            // Create unique URL based on GUID. In real production implementation you might do this otherways, but this is for simplicity purposes
            var webUrl = string.Format("{0}/sites/{1}", rootSiteUrl, Guid.NewGuid().ToString().Replace("-", ""));
            var tenantAdminUri = ConfigurationManager.AppSettings[Consts.AdminSiteCollectionUrl];

            // Notice that we do NOT use app model where for this sample. We use just specific service account. Could be easily
            // changed for example based on following sample: https://github.com/OfficeDev/PnP/tree/master/Samples/Provisioning.OnPrem.Async 
            using (var ctx = new ClientContext(tenantAdminUri))
            {
                ctx.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings[Consts.ProvisioningAccount], 
                                                                    ConfigurationManager.AppSettings[Consts.ProvisioningPassword], 
                                                                    ConfigurationManager.AppSettings[Consts.ProvisioningDomain]);
                // Set the time out as high as possible
                ctx.RequestTimeout = Timeout.Infinite;

                var tenant = new Tenant(ctx);
                var properties = new SiteCreationProperties()
                {
                    Url = webUrl,
                    Owner = string.Format("{0}\\{1}", 
                                ConfigurationManager.AppSettings[Consts.ProvisioningDomain], 
                                ConfigurationManager.AppSettings[Consts.ProvisioningAccount]),
                    Title = request.Title,
                    Template = "STS#0" // Create always team site, but specialize the site based on the template value
                };

                //start the SPO operation to create the site
                SpoOperation op = tenant.CreateSite(properties);
                ctx.Load(op, i => i.IsComplete);
                ctx.ExecuteQuery();
            }

            // Do some branding for the new site
            SetThemeToNewSite(webUrl);

            // Do addditional customziations based on the selected template request.Template

            return webUrl;
        }

        /// <summary>
        /// Used to connect to the newly created site and to apply custom branding to it.
        /// </summary>
        /// <param name="webUrl">URL to connect to</param>
        private static void SetThemeToNewSite(string webUrl)
        {
            // Notice that we do NOT use app model where for this sample. We use just specific service account. Could be easily
            // changed for example based on following sample: https://github.com/OfficeDev/PnP/tree/master/Samples/Provisioning.OnPrem.Async 
            using (var ctx = new ClientContext(webUrl))
            {
                ctx.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings[Consts.ProvisioningAccount],
                                                                    ConfigurationManager.AppSettings[Consts.ProvisioningPassword],
                                                                    ConfigurationManager.AppSettings[Consts.ProvisioningDomain]);
                // Set the time out as high as possible
                ctx.RequestTimeout = Timeout.Infinite;

                // Let's first upload the custom theme to host web
                DeployContosoThemeToWeb(ctx.Web, "Garage",
                                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/garagewhite.spcolor"),
                                string.Empty,
                                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/garagebg.jpg"),
                                "seattle.master");

                // Apply theme. We could upload a custom one as well or apply any other changes to newly created site
                SetThemeBasedOnName(ctx.Web, "Garage");

                // Upload the assets to host web
                UploadLogoToHostWeb(ctx.Web);

                // Set the properties accordingly
                // Notice that these are new properties in 2014 April CU of 15 hive CSOM 
                ctx.Web.SiteLogoUrl = ctx.Web.ServerRelativeUrl + "/SiteAssets/garagelogo.png";
                ctx.Web.Update();
                ctx.Web.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Uploads site logo to host web
        /// </summary>
        /// <param name="web"></param>
        private static void UploadLogoToHostWeb(Web web)
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

        public static void DeployContosoThemeToWeb(Web web, string themeName, string colorFilePath, string fontFilePath, string backgroundImagePath, string masterPageName)
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


        private static void DeployFileToThemeFolderSite(Web web, string sourceAddress)
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
                if (folder.Name == "15")
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


        private static bool ThemeEntryExists(Web web, List themeList, string themeName)
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

        private static void AddNewThemeOptionToSite(Web web, string themeName, string colorFilePath, string fontFilePath, string backGroundPath, string masterPageName)
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
