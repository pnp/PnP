using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.CreateSiteWeb.Pages
{
    public partial class Scenario2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // define initial script, needed to render the chrome control
            string script = @"
            function chromeLoaded() {
                $('body').show();
            }

            //function callback to render chrome after SP.UI.Controls.js loads
            function renderSPChrome() {
                //Set the chrome options for launching Help, Account, and Contact pages
                var options = {
                    'appTitle': document.title,
                    'onCssLoaded': 'chromeLoaded()'
                };

                //Load the Chrome Control in the divSPChrome element of the page
                var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                chromeNavigation.setVisible(true);
            }";

            //register script in page
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);

            if (!Page.IsPostBack)
            {
                // Provide options for the template
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    Web web = ctx.Web;
                    ctx.Load(web);
                    ctx.ExecuteQuery();
                    // Get templates to choose from
                    WebTemplateCollection templates = ctx.Web.GetAvailableWebTemplates(ctx.Web.Language, false);
                    ctx.Load(templates);
                    ctx.ExecuteQuery();

                    drpContentTypes.Items.Clear();
                    foreach (var item in templates)
                    {
                        // Provide options for the template
                        drpContentTypes.Items.Add(new System.Web.UI.WebControls.ListItem(item.Title, item.Name));
                        drpContentTypes.SelectedValue = "STS#0";
                    }
                }
            }
        }

        protected void btnCheckUrl_Click(object sender, EventArgs e)
        {
            User currUser = ResolveCurrentUser();

            //get the base tenant admin urls
            var tenantStr = Page.Request["SPHostUrl"].ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            // Let's resolve the admin URL and wanted new site URL
            var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", txtUrl.Text);
            var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantStr));

            // Creating new app only context for the operation
            string accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                tenantAdminUri.Authority,
                TokenHelper.GetRealmFromTargetUrl(tenantAdminUri)).AccessToken;

            using (var ctx = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), accessToken))
            {
                Tenant tenant = new Tenant(ctx);

                if (tenant.SiteExists(webUrl))
                {
                    lblStatus1.Text = string.Format("Site already existed. Used URL - {0}", webUrl);
                }
                else
                {
                    lblStatus1.Text = string.Format("Site with given URL does not exist. Used URL - {0}", webUrl);
                }
            }

        }

        protected void btnCreateSite_Click(object sender, EventArgs e)
        {
            User currUser = ResolveCurrentUser();

            //get the base tenant admin urls
            var tenantStr = Page.Request["SPHostUrl"].ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));

            // Let's resolve the admin URL and wanted new site URL
            var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", txtUrl.Text);
            var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantStr));

            // Creating new app only context for the operation
            string accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                tenantAdminUri.Authority,
                TokenHelper.GetRealmFromTargetUrl(tenantAdminUri)).AccessToken;

            using (var ctx = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), accessToken))
            {
                Tenant tenant = new Tenant(ctx);

                if (tenant.SiteExists(webUrl))
                {
                    lblStatus1.Text = string.Format("Site already existed. Used URL - {0}", webUrl);
                }
                else
                {
                    // Create new site collection with some storage limts and English locale
                    tenant.CreateSiteCollection(webUrl, txtName.Text, currUser.Email, drpContentTypes.SelectedValue, 500, 400, 7, 7, 1, 1033);

                    // Let's get instance to the newly added site collection using URLs
                    var siteUri = new Uri(webUrl);
                    string token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, TokenHelper.GetRealmFromTargetUrl(new Uri(webUrl))).AccessToken;
                    using (var newWebContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), token))
                    {
                        // Let's modify the web slightly
                        var newWeb = newWebContext.Web;
                        newWebContext.Load(newWeb);
                        newWebContext.ExecuteQuery();

                        // Let's add two document libraries to the site 
                        newWeb.CreateDocumentLibrary("Specifications"); 
                        newWeb.CreateDocumentLibrary("Presentations");

                        // Let's also apply theme to the site to demonstrate how easy this is
                        newWeb.SetComposedLookByUrl("Characters");
                    }

                    lblStatus1.Text = string.Format("Created a new site collection to address <a href='{0}'>{1}</a>", webUrl, webUrl);
                }
            }
        }

        private User ResolveCurrentUser()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            User currUser = null;

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                //get the current user to set as owner
                currUser = ctx.Web.CurrentUser;
                ctx.Load(currUser);
                ctx.ExecuteQuery();
            }

            return currUser;
        }
    }
}