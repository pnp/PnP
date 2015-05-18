using Microsoft.SharePoint.Client;
using Provisioning.Framework.Cloud.Async.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.Framework.Cloud.AsyncWeb
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

            // Set some default values
            lblBasePath.Text = Request["SPHostUrl"].Substring(0, 8 + Request["SPHostUrl"].Substring(8).IndexOf("/")) + "/sites/";
            templateSiteLink.Text = ConfigurationManager.AppSettings["TemplateSiteUrl"];
            templateSiteLink.NavigateUrl = ConfigurationManager.AppSettings["TemplateSiteUrl"];

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

                // Add wanted languages for creation list
                language.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(1033).DisplayName, "1033"));
                language.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(1035).DisplayName, "1035"));
                language.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(1036).DisplayName, "1036"));
                language.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(1053).DisplayName, "1053"));
            }

            // Set default values for the controls
            if (!Page.IsPostBack)
            {
                // Set template options - could also come from Azure or from some other solution, now hard coded. 
                listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Team", "ContosoTemplate-01.xml"));
                listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Simplistic", "ContosoTemplate-02.xml"));
                listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Oslo Team", "ContosoTemplate-03.xml"));
                listTemplates.SelectedIndex = 0;

                txtUrl.Text = DateTime.Now.Ticks.ToString();
                txtStorage.Text = "100";
                timeZone.SelectedValue = "10";
                listTemplates.Enabled = false;
                templateSiteLink.Enabled = true;
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
                lblSiteColAdmin.Text = currUser.Email;
            }
        }

        protected void templateSelectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (templateSelectionType.SelectedValue == "Site")
            {
                listTemplates.Enabled = false;
                templateSiteLink.Enabled = true;
            }
            else
            {
                listTemplates.Enabled = true;
                templateSiteLink.Enabled = false;
            }
        }

        private void AddRequestToQueue(string ownerEmail)
        {
            //get the base tenant url
            var tenantName = Page.Request["SPHostUrl"].ToLower().Replace("-my", "").Substring(8);
            tenantName = tenantName.Substring(0, tenantName.IndexOf("."));

            // Create provisioning message objects for the storage queue
            SiteCollectionRequest data = new SiteCollectionRequest()
            {
                TenantName = tenantName,
                Url = txtUrl.Text,
                Owner = ownerEmail,
                ManagedPath = "sites",
                TimeZoneId = int.Parse(timeZone.SelectedValue),
                StorageMaximumLevel = int.Parse(txtStorage.Text),
                Lcid = uint.Parse(language.SelectedValue),
                Title = txtTitle.Text
            };

            if (templateSelectionType.SelectedValue == "Site")
            {
                data.ProvisioningType = SiteProvisioningType.TemplateSite;
                data.TemplateId = templateSiteLink.Text;
            }
            else
            {
                data.ProvisioningType = SiteProvisioningType.Identity;
                data.TemplateId = listTemplates.SelectedValue;
            }

            new SiteManager().AddConfigRequestToQueue(data,
                                            ConfigurationManager.AppSettings["StorageConnectionString"]);
        }

        private string ResolveFutureUrl()
        {
            var tenantName = Page.Request["SPHostUrl"].ToLower().Replace("-my", "").Substring(8);
            tenantName = tenantName.Substring(0, tenantName.IndexOf("."));
            return string.Format("https://{0}/{1}/{2}", tenantName, "sites", txtUrl.Text);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }
    }
}