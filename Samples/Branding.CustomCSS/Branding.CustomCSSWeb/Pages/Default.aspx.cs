using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Branding.CustomCSSWeb
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

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;

                    List assetLibrary = web.Lists.GetByTitle("Site Assets");
                    clientContext.Load(assetLibrary, l => l.RootFolder);

                    // Get the path to the file which we are about to deploy
                    string file = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/{0}", "CSS/contoso.css"));

                      // Use CSOM to uplaod the file in
                    FileCreationInformation newFile = new FileCreationInformation();
                    newFile.Content = System.IO.File.ReadAllBytes(file);
                    newFile.Url = "contoso.css";
                    newFile.Overwrite = true;
                    Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile); 
                    clientContext.Load(uploadFile);
                    clientContext.ExecuteQuery();

                    // Now, apply a reference to the CSS URL via a custom action
                    string actionName = "ContosoCSSLink";

                    // Clean up existing actions that we may have deployed
                    var existingActions = web.UserCustomActions;
                    clientContext.Load(existingActions);

                    // Execute our uploads and initialzie the existingActions collection
                    clientContext.ExecuteQuery();

                    var actions = existingActions.ToArray();
                    // Clean up existing custom action with same name, if it exists
                    foreach (var existingAction in actions)
                    {
                        if (existingAction.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase))
                            existingAction.DeleteObject();
                    }
                    clientContext.ExecuteQuery();

                    // Build a custom action to write a link to our new CSS file
                    UserCustomAction cssAction = web.UserCustomActions.Add();
                    cssAction.Location = "ScriptLink";
                    cssAction.Sequence = 100;
                    cssAction.ScriptBlock = @"document.write('<link rel=""stylesheet"" href=""" + assetLibrary.RootFolder.ServerRelativeUrl + @"/contoso.css"" />');";
                    cssAction.Name = actionName;

                    // Apply
                    cssAction.Update();
                    clientContext.ExecuteQuery();

                    lblStatus.Text = string.Format("Custom CSS 'contoso.css' has been applied to the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
                }
            }

        }

        protected void btnScenario1Remove_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                Web web = clientContext.Web;
                string actionName = "ContosoCSSLink";
                // Clean up existing actions that we may have deployed
                var existingActions = web.UserCustomActions;
                clientContext.Load(existingActions);
                clientContext.ExecuteQuery();

                var actions = existingActions.ToArray();

                // Clean up
                foreach (var existingAction in actions)
                {
                    if (existingAction.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase))
                        existingAction.DeleteObject();
                }
                clientContext.ExecuteQuery();
                lblStatus.Text = string.Format("Custom CSS has been removed from the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            }
        }
    }
}