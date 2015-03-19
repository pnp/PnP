using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using OfficeDevPnP.Core;
using System.Text;
using Microsoft.Online.SharePoint.TenantAdministration;

namespace Core.SiteInformationWeb
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
            // define initial script, needed to render the chrome control
            string script = @"
            //function callback to render page after SP.UI.Controls.js loads
            function renderSPChrome() {
                $('body').show();
            }";

            //register script in page
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);

            //extract data during page load
            GetSiteInformation();
        }

        /// <summary>
        /// This method gathers the information from the current client context towards the host web
        /// </summary>
        private void GetSiteInformation()
        {
            string usageData = string.Empty;
            string siteCollectionUrl = string.Empty;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            //some information is gathered from an app-only token to the site collection
            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
            {
                //load data
                clientContext.Load(clientContext.Web, w => w.Title);
                clientContext.Load(clientContext.Site,
                    s => s.Usage,
                    s => s.ReadOnly,
                    s => s.Owner,
                    s => s.ShareByEmailEnabled,
                    s => s.ShareByLinkEnabled,
                    s => s.Url
                    );
                clientContext.ExecuteQuery();

                Site site = clientContext.Site;
                Web web  = clientContext.Web;
                siteCollectionUrl = site.Url;

                //Basic Info & binding to form
                UsageInfo usageInfo = site.Usage;
                lblStorageQuota.Text = FormatBytes(usageInfo.Storage);
                lblUsedStorage.Text = usageInfo.StoragePercentageUsed.ToString("P");
                lblTitle.Text = web.Title;
                lblReadOnly.Text = site.ReadOnly == true ? "Yes" : "No";
                lblExternalSharingByEmail.Text = site.ShareByEmailEnabled == true ? "Yes" : "No";
                lblExternalSharingByLink.Text = site.ShareByLinkEnabled == true ? "Yes" : "No";
                lblLastModified.Text = 

                //Site Collection Admins:
                lblOwner.Text = site.Owner.Title + " (" + site.Owner.Email + ")";

                //get site collection administrators
                var siteCollectionAdministrators = web.GetAdministrators();
                StringBuilder listOfAdmins = new StringBuilder();
                foreach (var admin in siteCollectionAdministrators)
                {
                    listOfAdmins.Append(admin.Title);
                    listOfAdmins.Append(" (");
                    listOfAdmins.Append(string.IsNullOrWhiteSpace(admin.Email) ? admin.LoginName : admin.Email);
                    listOfAdmins.Append("); ");
                }

                lblSiteCollectionAdmins.Text = listOfAdmins.ToString();
            }

            //proceed with data from the tenant object
            GetTenantInformation(siteCollectionUrl);
        }

        /// <summary>
        /// the following code uses tenant admin rights to get information from the Tenant API.
        /// this requires a tenatn administrator to trust the app
        /// </summary>
        /// <param name="siteCollectionUrl">The Url of the site collection for information</param>
        private void GetTenantInformation(string siteCollectionUrl)
        {
            string tenantName = ConfigurationManager.AppSettings["TenantName"];
            Uri tenantAdminUri = new Uri(string.Format("https://{0}-admin.sharepoint.com", tenantName));
            string adminRealm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            string adminToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, adminRealm).AccessToken;

            //tenant app-only context to admin site collection
            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), adminToken))
            {
                var tenant = new Tenant(clientContext);
                clientContext.Load(tenant);
                clientContext.ExecuteQuery();

                SiteProperties properties = tenant.GetSitePropertiesByUrl(siteCollectionUrl, true);
                clientContext.Load(properties);
                clientContext.ExecuteQuery();

                lblLastModified.Text = properties.LastContentModifiedDate.ToString();
                lblWebsCount.Text = properties.WebsCount.ToString();
            }
        }

        /// <summary>
        /// A simple method to display friendly bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
    }
}