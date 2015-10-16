using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.EmbedJavaScript.WeekNumbersWeb
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
            RegisterChromeControlScript();
        }

        private void RegisterChromeControlScript()
        {
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

            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);
        }


        protected void AddScripts_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                AddScriptsToHostWeb(clientContext);
                AddScriptLinksToHostWeb(clientContext);
            }
        }
        protected void RemoveScripts_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                var existingActions = clientContext.Web.UserCustomActions;
                clientContext.Load(existingActions);
                clientContext.ExecuteQuery();

                RemoveScriptLinksFromHostWeb(clientContext, existingActions);
            }
        }

        private void AddScriptsToHostWeb(ClientContext clientContext)
        {
            var web = clientContext.Web;
            var library = web.Lists.GetByTitle("Site Assets");
            clientContext.Load(library, l => l.RootFolder);

            UploadScript(clientContext, library, "jquery-1.9.1.min.js");
            UploadScript(clientContext, library, "weeknumbers.js");
        }

        private static void UploadScript(ClientContext clientContext, List library, string fileName)
        {
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/Scripts/{0}", fileName));
            var newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(filePath);
            newFile.Url = fileName;
            newFile.Overwrite = true;
            var uploadFile = library.RootFolder.Files.Add(newFile);
            clientContext.Load(uploadFile);
            clientContext.ExecuteQuery();
        }

        private static void AddScriptLinksToHostWeb(ClientContext clientContext)
        {
            var existingActions = clientContext.Web.UserCustomActions;
            clientContext.Load(existingActions);
            clientContext.ExecuteQuery();

            RemoveScriptLinksFromHostWeb(clientContext, existingActions);

            var customActionJQuery = existingActions.Add();
            customActionJQuery.Description = "weeknumberJQuery";
            customActionJQuery.Location = "ScriptLink";
            customActionJQuery.ScriptSrc = "~site/SiteAssets/jquery-1.9.1.min.js";
            customActionJQuery.Sequence = 1000;
            customActionJQuery.Update();

            var customActionWeekNumber = existingActions.Add();
            customActionWeekNumber.Description = "weeknumberScript";
            customActionWeekNumber.Location = "ScriptLink";
            customActionWeekNumber.ScriptSrc = "~site/SiteAssets/weeknumbers.js";
            customActionWeekNumber.Sequence = 1010;
            customActionWeekNumber.Update();

            clientContext.ExecuteQuery();
        }

        private static void RemoveScriptLinksFromHostWeb(ClientContext clientContext, UserCustomActionCollection existingActions)
        {
            var actions = existingActions.ToArray();
            foreach (var action in actions)
            {
                if (action.Location.Equals("ScriptLink") &&
                    (action.Description.Equals("weeknumberJQuery") || action.Description.Equals("weeknumberScript")))
                {
                    action.DeleteObject();
                }
            }

            clientContext.ExecuteQuery();
        }
    }
}