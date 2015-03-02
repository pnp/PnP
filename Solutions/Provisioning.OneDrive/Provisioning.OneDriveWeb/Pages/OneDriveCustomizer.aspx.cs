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

        protected void Page_Load(object sender, EventArgs e)
        {
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

                    Web rootWeb = personalSite.RootWeb;
                    clientContext.Load(rootWeb);
                    clientContext.ExecuteQuery();

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
                        WriteDebugInformationIfNeeded(string.Format("OneDrive for Business site existed at {0}. Custom branding applied.", personalSite.Url));
                    }
                    else
                    {
                        // Just to output status if enabled in the app part
                        WriteDebugInformationIfNeeded(string.Format("OneDrive for Business site existed at {0} and had right branding.", personalSite.Url));
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