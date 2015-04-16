using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Provisioning.Services.SiteManager.AppWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set URL presentation
            lblHostUrl.Text = ConfigurationManager.AppSettings["WebApplicationUrl"];

            // Add options to the template listing
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Contoso Team", "STS#0"));
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Contoso Blog", "BLOG#0"));
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Contoso Community", "COMMUNITY#0"));
        }

        protected void Create_Click(object sender, EventArgs e)
        {
            // Let's first create the site collection using impersonation
            SiteManager.SiteManagerClient managerClient = GetSiteManagerClient();
            SiteManager.SiteData newSite = new SiteManager.SiteData()
            {
                Description = txtDescription.Text,
                LcId = "1033",
                OwnerLogin = txtAdminPrimary.Text,
                SecondaryContactLogin = txtAdminSecondary.Text,
                Title = txtTitle.Text,
                Url = string.Format("sites/" + txtUrl.Text),
                WebTemplate = listSites.SelectedValue
            };
            // Create the site collection by calling the WCF end point in SP farm. Starting from April CU (2014), this is supported also with CSOM
            string newSiteUrl = managerClient.CreateSiteCollection(newSite);

            // Let's also set the site regiional settings to en-UK using the WCF end point, since this is not exposed usign CSOM
            managerClient.SetSiteLocale(newSiteUrl, "fi-fi");

            // Let's also brand the just created site collection properly using app identity
            // Using app identity, since we don't know if the requestor account has permissions to just created site collection
            Uri targetSite = new Uri(newSiteUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(targetSite);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, targetSite.Authority, realm).AccessToken;
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(targetSite.ToString(), token))
            {
                // Deploy theme to web, so that we can set that for the site
                Web web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQuery();
                DeployThemeToWeb(ctx, web);

                //Set the properties for applying custom theme which was jus uplaoded
                string spColorURL = URLCombine(web.ServerRelativeUrl, "/_catalogs/theme/15/contoso.spcolor");
                string spFontURL = URLCombine(web.ServerRelativeUrl, "/_catalogs/theme/15/contoso.spfont");
                string backGroundImage = URLCombine(web.ServerRelativeUrl, "/_catalogs/theme/15/contosobg.jpg");

                // Use the Red theme for demonstration
                web.ApplyTheme(spColorURL,
                                    spFontURL,
                                    backGroundImage,
                                    false);
                ctx.ExecuteQuery();

                // Redirect to just created site
                Response.Redirect(newSiteUrl);
            }
        }

        /// <summary>
        /// Used to create context to location provided as URL
        /// </summary>
        /// <param name="spContext"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private ClientContext CreateAppOnlyClientContextForUrl(SharePointContext spContext, string url)
        {
            return TokenHelper.GetClientContextWithAccessToken(url, spContext.AppOnlyAccessTokenForSPHost);
        }


        /// <summary>
        /// Does the dynamic configuration fro the WCF end point using code
        /// </summary>
        /// <returns>Needed proxy client with impersonation information.</returns>
        private SiteManager.SiteManagerClient GetSiteManagerClient()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            if (ConfigurationManager.AppSettings["WebApplicationUrl"].Contains("https://"))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
            }
            else
            {
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            }
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

            EndpointAddress endPoint = new EndpointAddress(ConfigurationManager.AppSettings["WebApplicationUrl"] + "/_vti_bin/provisioning.services.sitemanager/sitemanager.svc");
            //Set time outs, since site collection creation could take a while. Also set on server side.
            binding.ReceiveTimeout = TimeSpan.FromMinutes(15);
            binding.CloseTimeout = TimeSpan.FromMinutes(15);
            binding.OpenTimeout = TimeSpan.FromMinutes(15);
            binding.SendTimeout = TimeSpan.FromMinutes(15);

            //Create proxy instance
            SiteManager.SiteManagerClient managerClient = new SiteManager.SiteManagerClient(binding, endPoint);
            managerClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            // Set impersonation information. This account is needed to have the web application permissions in SP side
            var impersonator = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Service_UserId"],
                                                                ConfigurationManager.AppSettings["Service_Pwd"],
                                                                ConfigurationManager.AppSettings["Service_Domain"]);
            managerClient.ClientCredentials.Windows.ClientCredential = impersonator;

            return managerClient;
        }

        #region THEME DEPLOYMENT

        /// <summary>
        /// Deploy theme files to the web and create theme option
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        private void DeployThemeToWeb(ClientContext clientContext, Web web)
        {
            // Deploy files one by one to proper location
            DeployFileToThemeFolderSite(clientContext, web, "DeploymentFiles/Theme/Contoso.spcolor");
            DeployFileToThemeFolderSite(clientContext, web, "DeploymentFiles/Theme/Contoso.spfont");
            DeployFileToThemeFolderSite(clientContext, web, "DeploymentFiles/Theme/contosobg.jpg");

            // Let's also add entry to the Theme catalog. This is not actually required, but provides visibility for the theme option, if manually changed
            AddNewThemeOptionToSite(clientContext, web);
        }

        /// <summary>
        /// Deployes given file to theme folder
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        /// <param name="sourceAddress"></param>
        private void DeployFileToThemeFolderSite(ClientContext clientContext, Web web, string sourceAddress)
        {
            // Get the path to the file which we are about to deploy
            string file = HostingEnvironment.MapPath(string.Format("~/{0}", sourceAddress));

            List themesList = web.GetCatalog(123);
            // get the theme list
            clientContext.Load(themesList);
            clientContext.ExecuteQuery();
            Folder rootfolder = themesList.RootFolder;
            clientContext.Load(rootfolder);
            clientContext.Load(rootfolder.Folders);
            clientContext.ExecuteQuery();
            Folder folder15 = rootfolder;
            foreach (Folder folder in rootfolder.Folders)
            {
                if (folder.Name == "15")
                {
                    folder15 = folder;
                    break;
                }
            }

            // Use CSOM to uplaod the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(file);
            newFile.Url = folder15.ServerRelativeUrl + "/" + Path.GetFileName(sourceAddress);
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = folder15.Files.Add(newFile);
            clientContext.Load(uploadFile);
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Creates new options to the look and feel section
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        private void AddNewThemeOptionToSite(ClientContext clientContext, Web web)
        {
            // Let's get instance to the composite look gallery
            List themesOverviewList = web.GetCatalog(124);
            clientContext.Load(themesOverviewList);
            clientContext.ExecuteQuery();
            // Is the item already in the list?
            if (!ContosoThemeEntryExists(clientContext, web, themesOverviewList))
            {
                // Let's create new theme entry. Notice that theme selection is not available from UI in personal sites, so this is just for consistency sake
                ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                Microsoft.SharePoint.Client.ListItem item = themesOverviewList.AddItem(itemInfo);
                item["Name"] = "Contoso";
                item["Title"] = "Contoso";
                item["ThemeUrl"] = URLCombine(web.ServerRelativeUrl, "/_catalogs/theme/15/contoso.spcolor"); ;
                item["FontSchemeUrl"] = URLCombine(web.ServerRelativeUrl, "/_catalogs/theme/15/contoso.spfont"); ;
                item["ImageUrl"] = URLCombine(web.ServerRelativeUrl, "/_catalogs/theme/15/contosobg.jpg");
                // Notice that we use oob master, but just as well you vould upload and use custom one
                item["MasterPageUrl"] = URLCombine(web.ServerRelativeUrl, "/_catalogs/masterpage/seattle.master");
                item["DisplayOrder"] = 0;
                item.Update();
                clientContext.ExecuteQuery();
            }

        }

        /// <summary>
        /// Used to check if the theme option already exists in the site
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        /// <param name="themeList"></param>
        /// <returns></returns>
        private bool ContosoThemeEntryExists(ClientContext clientContext, Web web, List themeList)
        {

            CamlQuery query = new CamlQuery();
            query.ViewXml = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Name' />
                                <Value Type='Text'>Contoso</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";
            var found = themeList.GetItems(query);
            clientContext.Load(found);
            clientContext.ExecuteQuery();
            if (found.Count > 0)
            {
                return true;
            }
            return false;
        }

        private string URLCombine(string baseUrl, string relativeUrl)
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