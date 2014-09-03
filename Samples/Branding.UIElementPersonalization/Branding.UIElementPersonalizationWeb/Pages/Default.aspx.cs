using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Branding.UIElementPersonalizationWeb
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
            status.Items.Add("Ready...");

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
        /// <summary>
        /// Initiate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            status.Items.Clear();
            status.Items.Add("Inject Customization clicked...");

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (ClientContext clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Upload the assets to host web
                UploadAssetsToHostWeb(clientContext.Web);

                status.Items.Add("Image assets uploaded...");

                // Setup sample codes list for demo use only
                SetupCodesList(clientContext, clientContext.Web);

                status.Items.Add("Sample codes list setup...");

                // Inject the JsLink
                AddPersonalizeJsLink(clientContext, clientContext.Web);

                status.Items.Add("Javascript injected...");
                status.Items.Add("Click the 'Back to site' link to see the customizations applied...");
            }
        }

        /// <summary>
        /// Uploads sample images to use for personalization demo
        /// </summary>
        /// <param name="web"></param>
        private void UploadAssetsToHostWeb(Web web)
        {
            // Instance to site assets
            List assetLibrary = web.Lists.GetByTitle("Site Assets");
            web.Context.Load(assetLibrary, l => l.RootFolder);

            // Get the path to the first file which we are about to deploy
            string xxFile = System.Web.Hosting.HostingEnvironment.MapPath(
                                string.Format("~/{0}", "resources/contosoxx.png"));

            // Use CSOM to upload the image file
            FileCreationInformation newXXFile = new FileCreationInformation();
            newXXFile.Content = System.IO.File.ReadAllBytes(xxFile);
            newXXFile.Url = "contosoxx.png";
            newXXFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadXXFile = assetLibrary.RootFolder.Files.Add(newXXFile);
            web.Context.Load(uploadXXFile);            

            // Get the path to the second file which we are about to deploy
            string yyFile = System.Web.Hosting.HostingEnvironment.MapPath(
                                string.Format("~/{0}", "resources/contosoyy.png"));

            // Use CSOM to upload the image file
            FileCreationInformation newYYFile = new FileCreationInformation();
            newYYFile.Content = System.IO.File.ReadAllBytes(yyFile);
            newYYFile.Url = "contosoyy.png";
            newYYFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadYYFile = assetLibrary.RootFolder.Files.Add(newYYFile);
            web.Context.Load(uploadYYFile);
            
            // Get the path to the second file which we are about to deploy
            string zzFile = System.Web.Hosting.HostingEnvironment.MapPath(
                                string.Format("~/{0}", "resources/contosozz.png"));

            // Use CSOM to upload the image file
            FileCreationInformation newZZFile = new FileCreationInformation();
            newZZFile.Content = System.IO.File.ReadAllBytes(zzFile);
            newZZFile.Url = "contosozz.png";
            newZZFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadZZFile = assetLibrary.RootFolder.Files.Add(newZZFile);
            web.Context.Load(uploadZZFile);

            // Batch update
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Adds JsLink
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="web"></param>
        public void AddPersonalizeJsLink(ClientContext ctx, Web web)
        {
            string scenarioUrl = String.Format("{0}://{1}:{2}/Scripts", this.Request.Url.Scheme,
                                                this.Request.Url.DnsSafeHost, this.Request.Url.Port);
            string revision = Guid.NewGuid().ToString().Replace("-", "");
            string personalizeJsLink = string.Format("{0}/{1}?rev={2}", scenarioUrl, "personalize.js", revision);

            StringBuilder scripts = new StringBuilder(@"
                var headID = document.getElementsByTagName('head')[0]; 
                var");

            scripts.AppendFormat(@"
                newScript = document.createElement('script');
                newScript.type = 'text/javascript';
                newScript.src = '{0}';
                headID.appendChild(newScript);", personalizeJsLink);
            string scriptBlock = scripts.ToString();

            var existingActions = web.UserCustomActions;
            ctx.Load(existingActions);
            ctx.ExecuteQuery();

            var actions = existingActions.ToArray();
            foreach (var action in actions)
            {
                if (action.Description == "personalize" &&
                    action.Location == "ScriptLink")
                {
                    action.DeleteObject();
                    ctx.ExecuteQuery();
                }
            }

            var newAction = existingActions.Add();
            newAction.Description = "personalize";
            newAction.Location = "ScriptLink";
            
            newAction.ScriptBlock = scriptBlock;
            newAction.Update();
            ctx.Load(web, s => s.UserCustomActions);
            ctx.ExecuteQuery();
        }

       
        /// <summary>
        /// Deletes personalizeJsLink
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="web"></param>
        public void DeletePersonalizeJsLink(ClientContext ctx, Web web)
        {
            status.Items.Clear();
            status.Items.Add("Remove Customization clicked...");

            var existingActions = web.UserCustomActions;
            ctx.Load(existingActions);
            ctx.ExecuteQuery();
            var actions = existingActions.ToArray();
            foreach (var action in actions)
            {
                if (action.Description == "personalize" &&
                    action.Location == "ScriptLink")
                {
                    action.DeleteObject();
                    ctx.ExecuteQuery();
                }
            }
            status.Items.Add("Remove Customization completed...");

        }        

        /// <summary>
        /// Creates sample codes list for demo purposes
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="web"></param>
        public void SetupCodesList(ClientContext ctx, Web web)
        {
            string newListName = "CodesList";

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (ctx = spContext.CreateUserClientContextForSPHost())
            {
                if (!ctx.Web.ListExists(newListName))
                {
                    ctx.Web.AddList(ListTemplateType.GenericList, newListName, false, true);

                    List newlist = ctx.Web.Lists.GetByTitle(newListName);

                    FieldCollection collField = newlist.Fields;

                    // Add url field for links to site assets images
                    collField.AddFieldAsXml("<Field DisplayName='CodesImageUrl' Name='CodesImageUrl' Type='URL' />",
                                                               true,
                                                               AddFieldOptions.DefaultValue);
                    ctx.Load(collField);
                    ctx.ExecuteQuery();

                    // Create sample list items needed for demo purposes only
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    Microsoft.SharePoint.Client.ListItem newXXItem = newlist.AddItem(itemCreateInfo);
                    newXXItem["Title"] = "XX";
                    newXXItem["CodesImageUrl"] = "/SiteAssets/contosoxx.png";
                    newXXItem.Update();                    

                    itemCreateInfo = new ListItemCreationInformation();
                    Microsoft.SharePoint.Client.ListItem newYYItem = newlist.AddItem(itemCreateInfo);
                    newYYItem["Title"] = "YY";
                    newYYItem["CodesImageUrl"] = "/SiteAssets/contosoyy.png";
                    newYYItem.Update();                    

                    itemCreateInfo = new ListItemCreationInformation();
                    Microsoft.SharePoint.Client.ListItem newZZItem = newlist.AddItem(itemCreateInfo);
                    newZZItem["Title"] = "ZZ";
                    newZZItem["CodesImageUrl"] = "/SiteAssets/contosoZZ.png";
                    newZZItem.Update();

                    // Batch update
                    ctx.ExecuteQuery();
                }
            }
        }

        /// <summary>
        /// Clean up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                DeletePersonalizeJsLink(ctx, ctx.Web);
            }
        }

    }
}