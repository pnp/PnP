using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.ExternalSharingWeb.Pages
{
    public partial class ExternalSettingsAtSiteCollectionLevel : System.Web.UI.Page
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
            if (!Page.IsPostBack)
            {
                using (var ctx = GetAdminContext())
                {
                    // get site collections.
                    Tenant tenant = new Tenant(ctx);
                    SPOSitePropertiesEnumerable sites = tenant.GetSiteProperties(0, true);
                    ctx.Load(tenant);
                    ctx.Load(sites);
                    ctx.ExecuteQuery();

                    SharingCapabilities tenantSharing = tenant.SharingCapability;
                    switch (tenantSharing)
                    {
                        case SharingCapabilities.Disabled:
                            lblStatus.Text = "External sharing is disabled at tenant level.";
                            break;
                        case SharingCapabilities.ExternalUserSharingOnly:
                            lblStatus.Text = "External sharing at tenant level is set only for authenticated users.";
                            break;
                        case SharingCapabilities.ExternalUserAndGuestSharing:
                            lblStatus.Text = "External sharing at tenant level is for authenticated and guest users.";
                            break;
                        default:
                            break;
                    }

                    if (tenantSharing != SharingCapabilities.Disabled)
                    {
                        // List site collections
                        foreach (var item in sites)
                        {
                            sitecollections.Items.Add(new System.Web.UI.WebControls.ListItem(item.Url, item.Url));
                        }
                    }
                }

            }
        }

        protected void btnUpdateSiteCollectionStatus_Click(object sender, EventArgs e)
        {
            string siteUrl = sitecollections.SelectedValue;
            using (var ctx = GetAdminContext())
            {
                // get site collections.
                Tenant tenant = new Tenant(ctx);
                SiteProperties siteProp = tenant.GetSitePropertiesByUrl(siteUrl, true);
                ctx.Load(siteProp);
                ctx.ExecuteQuery();

                switch (rblSharingOptions.SelectedValue)
                {
                    case "Disabled":
                        siteProp.SharingCapability = SharingCapabilities.Disabled;
                        lblStatus.Text = "External sharing is for authenticated and guest users.";
                        break;
                    case "ExternalUserAndGuestSharing":
                        siteProp.SharingCapability = SharingCapabilities.ExternalUserAndGuestSharing;
                        lblStatus.Text = "External sharing is for authenticated and guest users.";
                        break;
                    case "ExternalUserSharingOnly":
                        siteProp.SharingCapability = SharingCapabilities.ExternalUserSharingOnly;
                        lblStatus.Text = "External sharing is for authenticated and guest users.";
                        break;
                }
                // Update based on applied setting
                siteProp.Update();
                ctx.ExecuteQuery();
                lblStatus.Text = string.Format("Sharing status updated for site collection at URL: {0}", siteUrl);
            }
        }

        protected void sitecollections_SelectedIndexChanged(object sender, EventArgs e)
        {
            string siteUrl = sitecollections.SelectedValue;

            using (var ctx = GetAdminContext())
            {
                // get site collections.
                Tenant tenant = new Tenant(ctx);
                SiteProperties siteProp = tenant.GetSitePropertiesByUrl(siteUrl, true);
                ctx.Load(siteProp);
                ctx.ExecuteQuery();

                switch (siteProp.SharingCapability)
                {
                    case SharingCapabilities.Disabled:
                        lblStatus.Text = "External sharing is disabled.";
                        rblSharingOptions.SelectedValue = "Disabled";
                        break;
                    case SharingCapabilities.ExternalUserSharingOnly:
                        lblStatus.Text = "External sharing is for authenticated users.";
                        rblSharingOptions.SelectedValue = "ExternalUserSharingOnly";
                        break;
                    case SharingCapabilities.ExternalUserAndGuestSharing:
                        lblStatus.Text = "External sharing is for authenticated and guest users.";
                        rblSharingOptions.SelectedValue = "ExternalUserAndGuestSharing";
                        break;
                    default:
                        break;
                }
                lblStatus.Text = string.Format("Sharing status resolved for site collection at URL: {0}", siteUrl);
            }
        }

        /// <summary>
        /// Returns admin context for manipulating things at tenant level
        /// </summary>
        /// <returns></returns>
        private ClientContext GetAdminContext()
        {
            string hostUrl = Page.Request["SPHostUrl"];
            var tenantAdminUri = new Uri(GetAdminURL(hostUrl));
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            return TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token);
        }

        string GetAdminURL(string hostWebUrl)
        {
            //get the base tenant admin urls
            var tenantStr = hostWebUrl.ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));
            return String.Format("https://{0}-admin.sharepoint.com", tenantStr);
        }
    }
}