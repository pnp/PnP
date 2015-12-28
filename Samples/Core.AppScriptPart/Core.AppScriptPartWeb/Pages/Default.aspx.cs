using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client.WebParts;
using ListItem = Microsoft.SharePoint.Client.ListItem;

namespace Core.AppScriptPartWeb
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
                Site site = clientContext.Site;
                clientContext.Load(site, s => s.Url);
                clientContext.ExecuteQuery();
                String webPartGalleryUrl = site.Url.TrimEnd('/') + "/_catalogs/wp";

                var folder = site.RootWeb.GetList(webPartGalleryUrl).RootFolder;
                //var folder = clientContext.Site.RootWeb.Lists.GetByTitle("Web Part Gallery").RootFolder;
                clientContext.Load(folder);
                clientContext.ExecuteQuery();

                //upload the "userprofileinformation.webpart" file
                using (var stream = System.IO.File.OpenRead(
                                Server.MapPath("~/userprofileinformation.webpart")))
                {
                    FileCreationInformation fileInfo = new FileCreationInformation();
                    fileInfo.ContentStream = stream;
                    fileInfo.Overwrite = true;
                    fileInfo.Url = "userprofileinformation.webpart";
                    File file = folder.Files.Add(fileInfo);
                    // Let's update the group for just uploaded web part
                    ListItem webpartItem = file.ListItemAllFields;
                    webpartItem["Group"] = "Add-in Script Part";
                    webpartItem.Update();
                    clientContext.ExecuteQuery();
                }

                lblStatus.Text = string.Format("Add-in script part has been added to web part gallery. You can find 'User Profile Information' script part under 'Add-in Script Part' group in the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            }
        }
    }
}