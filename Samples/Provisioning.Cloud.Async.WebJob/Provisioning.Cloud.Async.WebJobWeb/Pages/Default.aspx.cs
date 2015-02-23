using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure;
using Provisioning.Cloud.Async.WebJob.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.Cloud.Async.WebJobWeb
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


            listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Team", "STS#0"));
            listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Organization", "STS#0"));
            listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Group", "STS#0"));
            listTemplates.SelectedIndex = 0;

            lblBasePath.Text = Request["SPHostUrl"].Substring(0, 8 + Request["SPHostUrl"].Substring(8).IndexOf("/")) + "/sites/";

            txtStorage.Text = "100";

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (ClientContext ctx = spContext.CreateUserClientContextForSPHost())
            {
                // Get time zones from server side
                RegionalSettings reg = ctx.Web.RegionalSettings;
                TimeZoneCollection zones = ctx.Web.RegionalSettings.TimeZones;
                ctx.Load(reg);
                ctx.Load(zones);
                ctx.ExecuteQuery();

                foreach (var item in zones)
                {
                    timeZone.Items.Add(new System.Web.UI.WebControls.ListItem(item.Description, item.Id.ToString()));
                }
                timeZone.SelectedValue = "10";

                // Add wanted languages for creation list
                language.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(1033).DisplayName, "1033"));
                language.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(1035).DisplayName, "1035"));
                language.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(1036).DisplayName, "1036"));
                language.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(1053).DisplayName, "1053"));

                txtUrl.Text = Guid.NewGuid().ToString().Replace("-", "");
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (ClientContext ctx = spContext.CreateUserClientContextForSPHost())
            {

                //get the current user to set as owner
                var currUser = ctx.Web.CurrentUser;
                ctx.Load(currUser);
                ctx.ExecuteQuery();

                // Add request to the process queue
                AddRequestToQueue(currUser.Email);

                // Change active view
                processViews.ActiveViewIndex = 1;

                // Show that the information has been recorded.
                lblTitle.Text = txtTitle.Text;
                lblUrl.Text = ResolveFutureUrl();
                lblEmailToNotify.Text = currUser.Email;
                lblSiteColAdmin.Text = currUser.Email;


            }
        }

        private void AddRequestToQueue(string ownerEmail)
        {
            ProvisioningData data = new ProvisioningData();
            // Add request data in
            data.RequestData = new SiteRequestData()
            {
                Title = txtTitle.Text,
                Template = listTemplates.SelectedValue,
                Lcid = uint.Parse(language.SelectedValue),
                Owner = ownerEmail,
                StorageMaximumLevel = int.Parse(txtStorage.Text),
                TimeZoneId = int.Parse(timeZone.SelectedValue),
                Url = txtUrl.Text
            };

            // Resolve tenant name
            var tenantStr = Page.Request["SPHostUrl"].ToLower().Replace("-my", "").Substring(8);
            data.TenantName = tenantStr.Substring(0, tenantStr.IndexOf("."));

            new SiteRequestManager().AddConfigRequestToQueue(data,
                                            CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }

        private string ResolveFutureUrl()
        {
            var tenantStr = Page.Request["SPHostUrl"].ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));
            var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", txtUrl.Text);
            return webUrl;
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }
    }
}