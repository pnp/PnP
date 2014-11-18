using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Branding.AlternateCSSAndSiteLogoWeb
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

        protected void btnScenario_Click(object sender, EventArgs e)
        {

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                // Upload the assets to host web
                UploadAssetsToHostWeb(web);

                // Set the properties accordingly
                // Notice that these are new properties in 2014 April CU of 15 hive CSOM and July release of MSO CSOM
                web.AlternateCssUrl = web.ServerRelativeUrl + "/SiteAssets/contoso.css";
                web.SiteLogoUrl = web.ServerRelativeUrl + "/SiteAssets/pnp.png";
                web.Update();
                web.Context.ExecuteQuery();

                lblStatus.Text = string.Format("Custom CSS and logo has been applied to the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            }
        }

        /// <summary>
        /// Uploads used CSS and site logo to host web
        /// </summary>
        /// <param name="web"></param>
        private void UploadAssetsToHostWeb(Web web)
        {
            // Instance to site assets
            List assetLibrary = web.Lists.GetByTitle("Site Assets");
            web.Context.Load(assetLibrary, l => l.RootFolder);

            // Get the path to the file which we are about to deploy
            string cssFile = System.Web.Hosting.HostingEnvironment.MapPath(
                                string.Format("~/{0}", "resources/contoso.css"));

            // Use CSOM to upload the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(cssFile);
            newFile.Url = "contoso.css";
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();

            // Get the path to the file which we are about to deploy
            string logoFile = System.Web.Hosting.HostingEnvironment.MapPath(
                                string.Format("~/{0}", "resources/pnp.png"));

            // Use CSOM to upload the file in
            newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(logoFile);
            newFile.Url = "pnp.png";
            newFile.Overwrite = true;
            uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();
        }


        /// <summary>
        /// Clean up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScenarioRemove_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                Web web = clientContext.Web;
                // Clear the properties accordingly
                // Notice that these are new properties in 2014 April CU of 15 hive CSOM and July release of MSO CSOM
                web.AlternateCssUrl = "";
                web.SiteLogoUrl = "";
                web.Update();
                web.Context.ExecuteQuery();

                lblStatus.Text = string.Format("Custom CSS and logo has been removed from the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
           
            }
        }
    }
}