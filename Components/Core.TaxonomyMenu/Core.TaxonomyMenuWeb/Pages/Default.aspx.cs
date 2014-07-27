using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.TaxonomyMenuWeb
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
            
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                var user = clientContext.Web.CurrentUser;
                clientContext.Load(user);
                clientContext.ExecuteQuery();

                var peopleManager = new PeopleManager(clientContext);
                var userProperties = peopleManager.GetUserProfilePropertyFor(user.LoginName, "SPS-MUILanguages");               
                clientContext.ExecuteQuery();

                currentLanguages.Text = userProperties.Value;
            }
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

        protected void AddTaxonomy_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                TaxonomyHelper.SetupTermStore(clientContext);
            }
        }        

        private void AddScriptsToHostWeb(ClientContext clientContext)
        {
            var web = clientContext.Web;
            var library = web.Lists.GetByTitle("Site Assets");
            clientContext.Load(library, l => l.RootFolder);

            UploadScript(clientContext, library, "jquery-1.9.1.min.js");
            UploadScript(clientContext, library, "taxnav.js");
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
            customActionJQuery.Description = "taxonomyNavigationJQuery";
            customActionJQuery.Location = "ScriptLink";
            customActionJQuery.ScriptSrc = "~site/SiteAssets/jquery-1.9.1.min.js";
            customActionJQuery.Sequence = 1000;
            customActionJQuery.Update();

            var customActionTaxonomy = existingActions.Add();
            customActionTaxonomy.Description = "taxonomyNavigationScript";
            customActionTaxonomy.Location = "ScriptLink";
            customActionTaxonomy.ScriptSrc = "~site/SiteAssets/taxnav.js";
            customActionTaxonomy.Sequence = 1010;
            customActionTaxonomy.Update();
            
            clientContext.ExecuteQuery();
        }

        private static void RemoveScriptLinksFromHostWeb(ClientContext clientContext, UserCustomActionCollection existingActions)
        {
            var actions = existingActions.ToArray();
            foreach (var action in actions)
            {
                if (action.Location.Equals("ScriptLink") &&
                    (action.Description.Equals("taxonomyNavigationJQuery") || action.Description.Equals("taxonomyNavigationScript")))
                {
                    action.DeleteObject();
                }
            }

            clientContext.ExecuteQuery();
        }
    }
}