using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Provisioning.Cloud.Async.Console
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

                            // Mark it as finished & ready
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
            // Create the site collection
            //get the base tenant admin urls
            string tenantStr = ConfigurationManager.AppSettings["SiteCollectionRequests_SiteUrl"];
            tenantStr = tenantStr.ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));

            //create site collection using the Tenant object
            var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", listItem["SiteUrl"]);
            var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantStr));
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                var tenant = new Tenant(adminContext);
                var properties = new SiteCreationProperties()
                {
                    Url = webUrl,
                    Owner = listItem["RequestorEmail"].ToString(),
                    Title = listItem["Title"].ToString(),
                    Template = listItem["Template"].ToString(),
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

            ApplyTemplateForCreatedSiteCollection(webUrl, token, realm);

            return webUrl;
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
                newWebContext.RequestTimeout = Timeout.Infinite;

                // Let's first upload the custom theme to host web
                RemoteManager.DeployThemeToWeb(newWebContext.Web, "Garage",
                                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/garagewhite.spcolor"),
                                string.Empty,
                                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/garagebg.jpg"),
                                "seattle.master");

                // Apply theme. We could upload a custom one as well or apply any other changes to newly created site
                RemoteManager.SetThemeBasedOnName(newWebContext.Web, "Garage");

                // Upload the assets to host web
                RemoteManager.UploadLogoToHostWeb(newWebContext.Web);

                // Set the properties accordingly
                // This is waiting for 16 version of the CSOM update. Should be there on Sep.
                //ctx.Web.SiteLogoUrl = ctx.Web.ServerRelativeUrl + "/SiteAssets/garagelogo.png";
                //ctx.Web.Update();
                //ctx.Web.Context.ExecuteQuery();
            }
        }

        private static void SendEmailToRequestorAndNotifiedEmail(ClientContext ctx, ListItem listItem, string siteUrl)
        {
            string requestorEmail = listItem["RequestorEmail"].ToString();
            string notifyEmail = listItem["NotifyEmail"].ToString();

            // Following lines are commented for a purpose, but do show how to implement this.
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("sharepoint@contosoc.om");                       // Could come from web.config
            msg.To.Add(requestorEmail);
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
        private static void SetThemeBasedOnName(ClientContext ctx, Web web, string themeName)
        {
            // Let's get instance to the composite look gallery
            List themeList = web.GetCatalog(124);
            ctx.Load(themeList);
            ctx.ExecuteQuery();

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
            ctx.Load(found);
            ctx.ExecuteQuery();
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

                ctx.ExecuteQuery();
            }
        }

        private static string MakeAsRelativeUrl(string urlToProcess)
        {
            Uri uri = new Uri(urlToProcess);
            return uri.AbsolutePath;
        }

    }
}
