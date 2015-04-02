using Contoso.Provisioning.OneDriveWeb;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.OneDriveWeb.Pages
{
    public partial class OneDriveCustomizer : Page
    {

        private const string OneDriveMarkerBagID = "Contoso_OneDriveVersion";
        private const string OneDriveCookieName = "OneDriveCustomizerCheck";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if we should skip this check. We do this only once per hour to avoid 
            // perf issues and there's really no point even hitting the user profile 
            // in every request.
            if (CookieCheckSkip())
                return;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (ClientContext clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Get user profile
                ProfileLoader loader = Microsoft.SharePoint.Client.UserProfiles.ProfileLoader.GetProfileLoader(clientContext);
                UserProfile profile = loader.GetUserProfile();
                Microsoft.SharePoint.Client.Site personalSite = profile.PersonalSite;

                clientContext.Load(personalSite);
                clientContext.ExecuteQuery();

                // Let's check if the site already exists
                if (personalSite.ServerObjectIsNull.Value)
                {
                    // Let's queue the personal site creation using oob timer job based approach
                    // Using async mode, since end user could go away from browser, you could do this using oob web part as well
                    profile.CreatePersonalSiteEnque(true);
                    clientContext.ExecuteQuery();
                    WriteDebugInformationIfNeeded("OneDrive for Business site was not present, queued for provisioning now.");
                }
                else
                {
                    // Site already exists, let's modify the branding by applying a theme... just as well you could upload
                    // master page and set that to be shown. Notice that you can also modify this code to change the branding
                    // later and updates would be reflected whenever user visits OneDrive host... or any other location where this
                    // app part is located. You could place this also to front page of the intranet for ensuring that it's applied.

                    string personalSiteUrl = personalSite.Url;
                    Uri siteUri = new Uri(personalSiteUrl);
                    string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
                    string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;
                    using (var appOnlyCtx = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken))
                    {
                        Web rootWeb = appOnlyCtx.Web;
                        appOnlyCtx.Load(rootWeb);
                        appOnlyCtx.ExecuteQuery();

                        //Let's set the theme only if needed, note that you can easily check for example specific version here as well
                        if (rootWeb.GetPropertyBagValueInt(OneDriveCustomizer.OneDriveMarkerBagID, 0) < 2)
                        {
                            // Let's first upload the contoso theme to host web, if it does not exist there
                            var colorFile = rootWeb.UploadThemeFile(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/Themes/SPC/SPCTheme.spcolor")));
                            var backgroundFile = rootWeb.UploadThemeFile(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/Themes/SPC/SPCbg.jpg")));
                            rootWeb.CreateComposedLookByUrl("SPC", colorFile.ServerRelativeUrl, null, backgroundFile.ServerRelativeUrl, string.Empty);
                        
                            // Setting the Contoos theme to host web
                            rootWeb.SetComposedLookByUrl("SPC");

                            // Add additional JS injection with the policy statement to the site
                            rootWeb.AddJsLink("OneDriveCustomJS", BuildJavaScriptUrl());

                            // Let's set the site processed, so that we don't update that all the time. Currently set as "version" 1 of branding
                            rootWeb.SetPropertyBagValue(OneDriveCustomizer.OneDriveMarkerBagID, 2);

                            // Write output if enabled
                            WriteDebugInformationIfNeeded(string.Format("OneDrive for Business site existed at {0}. Custom branding applied.", personalSiteUrl));
                        }
                        else
                        {
                            // Just to output status if enabled in the app part
                            WriteDebugInformationIfNeeded(string.Format("OneDrive for Business site existed at {0} and had right branding.", personalSiteUrl));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Just to build the JS path which can be then pointed to the OneDrive site.
        /// </summary>
        /// <returns></returns>
        public string BuildJavaScriptUrl()
        {
            string scenarioUrl = String.Format("{0}://{1}:{2}/Resources", this.Request.Url.Scheme, this.Request.Url.DnsSafeHost, this.Request.Url.Port);
            string revision = Guid.NewGuid().ToString().Replace("-", "");
            return string.Format("{0}/{1}?rev={2}", scenarioUrl, "FileConfidentialityMessage.js", revision);
        }

        private bool CookieCheckSkip()
        {
            // Get cookie from the current request.
            HttpCookie cookie = Request.Cookies.Get(OneDriveCustomizer.OneDriveCookieName);

            // Check if cookie exists in the current request.
            if (cookie == null)
            {
                // Create cookie.
                cookie = new HttpCookie(OneDriveCustomizer.OneDriveCookieName);
                // Set value of cookie to current date time.
                cookie.Value = DateTime.Now.ToString();
                // Set cookie to expire in 60 minutes.
                cookie.Expires = DateTime.Now.AddMinutes(60);
                // Insert the cookie in the current HttpResponse.
                Response.Cookies.Add(cookie);
                // Output debugging information
                WriteDebugInformationIfNeeded(
                    string.Format(@"Cookie did not exist, adding new cookie with {0} 
                                    as expiration. Execute code.",
                                    cookie.Expires));
                // Since cookie did not existed, let's execute the code, 
                // so skip is false.
                return false;
            }
            else
            {
                // Output debugging information
                WriteDebugInformationIfNeeded("Cookie did existed, skipping the code for now.");
                //  Since cookie did existed, let's skip the code
                return true;
            }
        }

        private void WriteDebugInformationIfNeeded(string message)
        {
            if (this.Page.Request.Params.AllKeys.Contains("OneDriveDebug") &&
                this.Page.Request.Params["OneDriveDebug"].ToString() == "true")
            {
                status.Text = message;
            }
            else
            {
                Response.Write(string.Format("<!--{0}-->", message));
            }
        }
    }
}