using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Provisioning.SiteCol.OnPremWeb
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
            // Just routing of the app token cross post backs if http is used with ACS in on-prem.
            // Quick fix for http handling.
            if (!string.IsNullOrEmpty(this.Request.Form["SPAppToken"]))
            {
                SPAppToken.Value = this.Request.Form["SPAppToken"];
            }

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

            // Let's add some template options. These are all oob team sites, but we could differentiate them by branding or whatever is needed
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Team", "STS#0"));
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Super Team", "STS#0"));
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Über Team", "STS#0"));
            listSites.SelectedIndex = 0;

            lblBasePath.Text = Request["SPHostUrl"].Substring(0, 8 + Request["SPHostUrl"].Substring(8).IndexOf("/")) + "/sites/";
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string newWebUrl = newWebUrl = CreateSiteCollection(Page.Request["SPHostUrl"], txtUrl.Text,
                                                listSites.SelectedValue, txtTitle.Text, txtAdminAccount.Text);
            Response.Redirect(newWebUrl);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }

        private string CreateSiteCollection(string hostWebUrl, string url, string template, string title, string adminAccount)
        {
            // Resolve root site collection URL from host web. We assume that this has been set as the "TenantAdminSite"
            string rootSiteUrl = hostWebUrl.Substring(0, 8 + hostWebUrl.Substring(8).IndexOf("/"));

            //Resolve URL for the new site collection
            var webUrl = string.Format("{0}/sites/{1}", rootSiteUrl, url);

            // Notice that this assumes that AdministrationSiteType as been set as TenantAdministration for root site collection
            // If this tenant admin URI is pointing to site collection which is host named site collection, code does create host named site collection as well
            var tenantAdminUri = new Uri(rootSiteUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                var tenant = new Tenant(adminContext);
                var properties = new SiteCreationProperties()
                {
                    Url = webUrl,
                    Owner = adminAccount,
                    Title = title,
                    Template = template
                };

                //start the SPO operation to create the site
                SpoOperation op = tenant.CreateSite(properties);
                adminContext.Load(op, i => i.IsComplete);
                adminContext.ExecuteQuery();
            }

            // Set theme for the new site
            SetThemeToNewSite(webUrl);

            // Return URL for redirection
            return webUrl;
        }

        /// <summary>
        /// Used to connect to the newly created site and to apply custom branding to it.
        /// </summary>
        /// <param name="webUrl">URL to connect to</param>
        private void SetThemeToNewSite(string webUrl)
        {
            Uri siteUrl = new Uri(webUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUrl);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUrl.Authority, realm).AccessToken;
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(siteUrl.ToString(), token))
            {
                // Apply theme. We could upload a custom one as well or apply any other changes to newly created site
                new ThemeManager().SetThemeBasedOnName(ctx.Web, "Sketch");
            }
        }
    }
}