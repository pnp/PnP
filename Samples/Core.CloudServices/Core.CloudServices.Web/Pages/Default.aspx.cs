using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.CloudServices.Web.Pages
{

    public partial class Default : System.Web.UI.Page
    {
        public const string SPHostUrlKey = "SPHostUrl";
        private const string queueName = "workerusernamepassword";
        private const string queueNameOAuth = "oauth";
        private const string azureConnectionSettingKey = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        private static CloudQueueClient queueStorage;
        private static bool storageInitialized = false;
        private static object gate = new Object();

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

            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            RefreshSiteTitle();

            // Initialize the Azure queues
            InitializeStorage();
        }

        private void RefreshSiteTitle()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();
                lblCurrentTitle1.Text = clientContext.Web.Title;
                lblCurrentTitle2.Text = clientContext.Web.Title;
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshSiteTitle();
        }

        protected void btnRefresh2_Click(object sender, EventArgs e)
        {
            RefreshSiteTitle();
        }

        protected void btnChangeTitle1_Click(object sender, EventArgs e)
        {
            // submit provisioning data to queue
            CloudQueue queue = queueStorage.GetQueueReference(queueName);
            
            string hostWebUrl = Page.Request.Params[SPHostUrlKey];
            string messageToQueue = string.Format("{0}|{1}", txtNewTitle1.Text, hostWebUrl);
            
            // store the message in the queue
            CloudQueueMessage message = new CloudQueueMessage(messageToQueue);
            queue.AddMessage(message);
        }

        protected void btnChangeTitle2_Click(object sender, EventArgs e)
        {
            // submit provisioning data to queue
            CloudQueue queueOAuth = queueStorage.GetQueueReference(queueNameOAuth);

            string hostWebUrl = Page.Request.Params[SPHostUrlKey];
            string messageToQueue = string.Format("{0}|{1}", txtNewTitle2.Text, hostWebUrl);

            // store the message in the queue
            CloudQueueMessage message = new CloudQueueMessage(messageToQueue);
            queueOAuth.AddMessage(message);
        }


        /// <summary>
        /// Initializes Azure storage and sets up the queue needed to communicate with the worker process
        /// </summary>
        private void InitializeStorage()
        {
            if (storageInitialized)
            {
                return;
            }

            lock (gate)
            {
                if (storageInitialized)
                {
                    return;
                }

                try
                {
                    // read account configuration settings
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(azureConnectionSettingKey));
                    // Setup the queue to communicate with the worker role
                    queueStorage = cloudStorageAccount.CreateCloudQueueClient();
                    CloudQueue queue = queueStorage.GetQueueReference(queueName);
                    CloudQueue queueOAuth = queueStorage.GetQueueReference(queueNameOAuth);
                    queue.CreateIfNotExists();
                }
                catch (WebException)
                {
                    throw new WebException("Storage services initialization failure. "
                        + "Check your storage account configuration settings. If running locally, "
                        + "ensure that the Development Storage service is running.");
                }

                storageInitialized = true;
            }
        }

    }
}