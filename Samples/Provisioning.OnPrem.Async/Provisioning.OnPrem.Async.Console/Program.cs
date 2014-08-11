using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Provisioning.OnPrem.Async.Console
{
    class Program
    {
        private static string SharePointPrincipal = "00000003-0000-0ff1-ce00-000000000000";

        static void Main(string[] args)
        {

            Uri siteUri = new Uri(ConfigurationManager.AppSettings["SiteCollectionRequests_SiteUrl"]);

            //Get the realm for the URL
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);

            //Get the access token for the URL.  
            //   Requires this app to be registered with the tenant
            string accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                siteUri.Authority, realm).AccessToken;

            //Get client context with access token
            using (var ctx =
                TokenHelper.GetClientContextWithAccessToken(
                    siteUri.ToString(), accessToken))
            {
                // Set the time out as high as possible
                ctx.RequestTimeout = int.MaxValue;
                // Get items which are in requested status
                List list = ctx.Web.Lists.GetByTitle(ConfigurationManager.AppSettings["SiteCollectionRequests_List"]);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Status'/>" +
                                    "<Value Type='Text'>Requested</Value></Eq></Where></Query><RowLimit>10</RowLimit></View>";
                ListItemCollection listItems = list.GetItems(camlQuery);
                ctx.Load(listItems);
                ctx.ExecuteQuery();

                foreach (ListItem item in listItems)
                {
                    // get item one more time and check that it's still in requested status
                    ListItem listItem = list.GetItemById(item.Id);
                    ctx.Load(listItem);
                    ctx.ExecuteQuery();

                    if (listItem["Status"].ToString().ToLowerInvariant() == "Requested".ToLowerInvariant())
                    {
                        try
                        {
                            // Mark it as provisioning
                            UpdateStatusToList(ctx, listItem.Id, "Provisioning", "Started provisioning at " + DateTime.Now.ToString());

                            // Process request
                            string newUrl = ProcessSiteCreationRequest(ctx, listItem);

                            // Mark it as provisioning
                            UpdateStatusToList(ctx, listItem.Id, "Ready", "Created at " + DateTime.Now.ToString());

                            // Send email
                            SendEmailToRequestorAndNotifiedEmail(ctx, listItem, newUrl);

                        }
                        catch (Exception ex)
                        {
                            // Store the exception information to the list for viewing from browser
                            UpdateStatusToList(ctx, listItem.Id, "Failed", ex.Message);
                        }
                    }
                }
            }
        }

        private static string ProcessSiteCreationRequest(ClientContext ctx, ListItem listItem)
        {

            //get the base tenant admin urls
            string tenantStr = ConfigurationManager.AppSettings["SiteCollectionRequests_SiteUrl"];

            // Resolve root site collection URL from host web. We assume that this has been set as the "TenantAdminSite"
            string rootSiteUrl = tenantStr.Substring(0, 8 + tenantStr.Substring(8).IndexOf("/"));

            //Resolve URL for the new site collection
            var webUrl = string.Format("{0}/sites/{1}", rootSiteUrl, listItem["SiteUrl"].ToString());
            var tenantAdminUri = new Uri(rootSiteUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                // Set the time out as high as possible
                adminContext.RequestTimeout = int.MaxValue;

                var tenant = new Tenant(adminContext);
                var properties = new SiteCreationProperties()
                {
                    Url = webUrl,
                    Owner = listItem["AdminAccount"].ToString(),
                    Title = listItem["Title"].ToString(),
                    Template = listItem["Template"].ToString(),
                };

                //start the SPO operation to create the site
                SpoOperation op = tenant.CreateSite(properties);
                adminContext.Load(op, i => i.IsComplete);
                adminContext.RequestTimeout = int.MaxValue;
                adminContext.ExecuteQuery();
            }

            // Do some branding for the new site
            SetThemeToNewSite(webUrl);

            return webUrl;
        }

        /// <summary>
        /// Used to connect to the newly created site and to apply custom branding to it.
        /// </summary>
        /// <param name="webUrl">URL to connect to</param>
        private static void SetThemeToNewSite(string webUrl)
        {
            Uri siteUrl = new Uri(webUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUrl);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUrl.Authority, realm).AccessToken;
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(siteUrl.ToString(), token))
            {
                // Set the time out as high as possible
                ctx.RequestTimeout = int.MaxValue;

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
                // Notice that these are new properties in 2014 April CU of 15 hive CSOM and July release of MSO CSOM
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

        private static void SendEmailToRequestorAndNotifiedEmail(ClientContext ctx, ListItem listItem, string siteUrl)
        {
            string notifyEmail = listItem["NotifyEmail"].ToString();

            // Following lines are commented for a purpose, but do show how to implement this.
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("sharepoint@contoso.com");                       // Could come from web.config
            msg.To.Add(notifyEmail);
            msg.Subject = "Your site has been created.";
            msg.Body = string.Format("Your site has been now created to {0}.", siteUrl);
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "hostname";                                                     // Could come from web.config
            smtp.Port = 24;                                                             // Could come from web.config
            smtp.Credentials = new System.Net.NetworkCredential("account", "pwd");      // from web config and could be crypted
            smtp.EnableSsl = true;
            // Commented for a purpose for now. You can implement what ever kind of notification mechanism you want, 
            // like show the created stuff in the portal front page for creator or post a notification to Yammer.
            // smtp.Send(msg);
        }

        private static void UpdateStatusToList(ClientContext ctx, int id, string status, string statusMessage)
        {
            List list = ctx.Web.Lists.GetByTitle(ConfigurationManager.AppSettings["SiteCollectionRequests_List"]);
            ListItem listItem = list.GetItemById(id);
            listItem["Status"] = status;
            listItem["StatusMessage"] = statusMessage;
            listItem.Update();
            ctx.ExecuteQuery();
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

    }
}
