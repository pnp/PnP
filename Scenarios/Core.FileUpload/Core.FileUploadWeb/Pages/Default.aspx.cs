using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using System.Web.Hosting;

namespace Core.FileUploadWeb
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

        protected void btnScenario1_Click(object sender, EventArgs e)
        {
            // Deploy to library
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                ctx.Web.UploadDocumentToLibrary(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/SP2013_LargeFile.pptx")), "Docs", true);
                lblStatus1.Text = "Document has been uploaded to host web to new library called Docs, which was created unless it already existed.";
            }
        }

        protected void btnScenario2_Click(object sender, EventArgs e)
        {
            // Deploy to folder
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                ctx.Web.UploadDocumentToFolder(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/SP2013_LargeFile.pptx")), "hiddentest", true);
                lblStatus1.Text = "Document has been uploaded to host web to folder called hiddentest, which was created unless it already existed.<br/>Folder are not visible from the browser UI, but if you request the URL in browser, you can verify that the file is there.";
            }
        }
    }
}