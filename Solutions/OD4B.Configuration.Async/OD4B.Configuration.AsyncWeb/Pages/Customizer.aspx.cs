using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using Microsoft.WindowsAzure;
using OD4B.Configuration.Async.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OD4B.Configuration.AsyncWeb.Pages
{
    public partial class Customizer : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if we should skip this check. We do this only once per hour to avoid 
            // perf issues and there's really no point even hitting the user profile 
            // in every request.
            if (CookieCheckSkip())
                return;

            var spContext = 
                SharePointContextProvider.Current.GetSharePointContext(Context);
            using (ClientContext clientContext = 
                spContext.CreateUserClientContextForSPHost())
            {
                // Get user profile
                ProfileLoader loader = ProfileLoader.GetProfileLoader(clientContext);
                UserProfile profile = loader.GetUserProfile();
                Microsoft.SharePoint.Client.Site personalSite = profile.PersonalSite;

                clientContext.Load(profile, prof => prof.AccountName);
                clientContext.Load(personalSite);
                clientContext.ExecuteQuery();

                // Let's check if the site already exists
                if (personalSite.ServerObjectIsNull.Value)
                {
                    // Let's queue the personal site creation using oob timer job based
                    // approach using async mode, since end user could go away from 
                    // browser, you could do this using oob web part as well
                    profile.CreatePersonalSiteEnque(true);
                    clientContext.ExecuteQuery();
                    WriteDebugInformationIfNeeded("OneDrive for Business site was not present, queued for provisioning now.");
                }
                else
                {
                    // Site already exists, let's create a task to the Azure queue for web job to start processing these requests...
                    // Notice that we bypass the site collection URL as the parameter to the queue, so that web job knows which site collection to process
                    AddConfigurationRequestToQueue(profile.AccountName, profile.PersonalSite.Url);

                    // Let's add taks to the queueu for configuration check up.
                    WriteDebugInformationIfNeeded(string.Format("OneDrive for Business site existed at {0}. Configuration check up task created.", personalSite.Url));

                }
            }
        }

        /// <summary>
        /// Actual code to add the configuration request to Azure storage queue, so that web job will start executing request in async way
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="siteUrl"></param>
        private void AddConfigurationRequestToQueue(string accountName, string siteUrl)
        {
            // Add configuration task to queue
            new SiteModificationManager().AddConfigRequestToQueue(accountName, siteUrl,
                                            CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }


        /// <summary>
        /// Checks if we need to execute the code customization code again. 
        /// Timer set to 60 minutes to avoid constant execution of the code for nothing.
        /// </summary>
        /// <returns></returns>
        private bool CookieCheckSkip()
        {
            // Get cookie from the current request.
            HttpCookie cookie = Request.Cookies.Get("OneDriveCustomizerCheck");

            // Check if cookie exists in the current request.
            if (cookie == null)
            {
                // Create cookie.
                cookie = new HttpCookie("OneDriveCustomizerCheck");
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
                WriteDebugInformationIfNeeded(string.Format(@"Cookie did existed, 
                                            with {0} as expiration. Skipping code.", 
                                            cookie.Expires));
                //  Since cookie did existed, let's skip the code
                return true;
            }
        }

        private void WriteDebugInformationIfNeeded(string message)
        {
            if (this.Page.Request.Params.AllKeys.Contains("OneDriveDebug") &&
                this.Page.Request.Params["OneDriveDebug"].ToString() == "true")
            {
                // Output additional message
                status.Text = status.Text + "<br/>" + message;
            }
            else
            {
                Response.Write(string.Format("<!--{0}-->", message));
            }
        }
    }
}