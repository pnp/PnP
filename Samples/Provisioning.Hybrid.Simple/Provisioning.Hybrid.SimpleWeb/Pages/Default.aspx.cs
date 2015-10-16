using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Provisioning.Hybrid.Simple.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.Hybrid.SimpleWeb
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

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (ClientContext ctx = spContext.CreateUserClientContextForSPHost())
            {
                var currUser = ctx.Web.CurrentUser;
                ctx.Load(currUser);
                ctx.ExecuteQuery();
                
                ProcessSiteRequest(currUser.Email);

                // Change active view
                processViews.ActiveViewIndex = 1;

                // Show that the information has been recorded.
                lblTitle.Text = txtTitle.Text;
                lblEnvironment.Text = drlEnvironment.SelectedItem.Text;
                lblSiteColAdmin.Text = currUser.Email;
            }
        }

        // Site creation request to queue
        private void ProcessSiteRequest(string adminEmail)
        {

            CloudStorageAccount storageAccount =
                                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Get queue... create if does not exist.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue =
                queueClient.GetQueueReference(Provisioning.Hybrid.Simple.Common.Consts.StorageQueueName);
            queue.CreateIfNotExists();

            // Pass in data for modification
            var newSiteRequest = new SiteCollectionRequest()
            {
                Title = txtTitle.Text,
                OwnerIdentifier = adminEmail,
                TargetEnvironment = drlEnvironment.SelectedValue,
                Template = drlTemplate.SelectedValue
            };

            // Add entry to queue
            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(newSiteRequest)));
        }

    }
}