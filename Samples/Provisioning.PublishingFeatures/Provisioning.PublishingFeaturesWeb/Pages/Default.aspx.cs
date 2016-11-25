using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.PublishingFeaturesWeb
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
            string ContosoWebPageCTId = "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB064584E219954237AF390064DEA0F50FC8C147B0B6EA0636C4A7D4002CA362904d604607B0F1E39BE59D76E0";

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Create content type for the page layouts
                if (!clientContext.Web.ContentTypeExistsByName("ContosoWebPage"))
                {
                    // Let's create a content type which is inherited from oob welcome page
                    clientContext.Web.CreateContentType("ContosoWebPage",
                                                        ContosoWebPageCTId,
                                                        "Contoso Web Content Types");
                }

                // Upload page layouts to the master page gallery
                clientContext.Web.DeployPageLayout(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/ContosoLinksBelow.aspx")),
                                                    "Contoso Links Below", "Contoso Links Below", ContosoWebPageCTId);

                clientContext.Web.DeployPageLayout(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/ContosoLinksRight.aspx")),
                                                    "Contoso Links Right", "Contoso Links Right", ContosoWebPageCTId);

                // Add content type to Pages library
                clientContext.Web.AddContentTypeToListById("Pages", ContosoWebPageCTId);

                // Deploy addditional JS to site
                DeployJStoContosoFoldersInStyleLibrary(clientContext);

                // Create a new page based on the page layout
                List pages = clientContext.Web.Lists.GetByTitle("Pages");
                Microsoft.SharePoint.Client.ListItemCollection existingPages = pages.GetItems(CamlQuery.CreateAllItemsQuery());
                clientContext.Load(pages);
                clientContext.Load(existingPages, items => items.Include(item => item.DisplayName).Where(obj => obj.DisplayName == "demo"));
                clientContext.ExecuteQuery();

                // Check if page already exists and delete old version if existed
                if (existingPages != null && existingPages.Count > 0)
                {
                    existingPages[0].DeleteObject();
                    clientContext.ExecuteQuery();
                }

                // Solve layout and create new page
                Microsoft.SharePoint.Client.ListItem pageLayout = clientContext.Web.GetPageLayoutListItemByName("ContosoLinksRight.aspx");
                PublishingWeb pWeb = PublishingWeb.GetPublishingWeb(clientContext, clientContext.Web);
                PublishingPageInformation publishingPageInfo = new PublishingPageInformation();
                publishingPageInfo.Name = "demo.aspx";
                publishingPageInfo.PageLayoutListItem = pageLayout;
                PublishingPage publishingPage = pWeb.AddPublishingPage(publishingPageInfo);
                if (pages.ForceCheckout || pages.EnableVersioning)
                {
                    publishingPage.ListItem.File.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                    publishingPage.ListItem.File.Publish(string.Empty);
                    if (pages.EnableModeration)
                    {
                        publishingPage.ListItem.File.Approve(string.Empty);
                    }
                }
                clientContext.ExecuteQuery();

                lblStatus1.Text = string.Format("New content type created, page layouts uploaded and new page created to the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString() + "/pages/demo.aspx");

            }
        }

        /// <summary>
        /// Differentiated to keep the button click clean. Creates the folder structure for Style Library
        /// </summary>
        /// <param name="clientContext"></param>
        private void DeployJStoContosoFoldersInStyleLibrary(ClientContext clientContext)
        {
            List styleLib = clientContext.Web.GetListByTitle("Style Library");
            Folder rootFolder = styleLib.RootFolder;
            if (!rootFolder.FolderExists("Contoso"))
            {
                rootFolder.Folders.Add("Contoso");
                clientContext.ExecuteQuery();
            }
            Folder contosoFolder = rootFolder.ResolveSubFolder("Contoso");
            if (!contosoFolder.FolderExists("Scripts"))
            {
                contosoFolder.Folders.Add("Scripts");
                clientContext.ExecuteQuery();
            }
            Folder contosoScriptFolder = contosoFolder.ResolveSubFolder("Scripts");

            // Get the file stream
            var fileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/UserProfileData.js")));

            // Use CSOM to upload the file to specific folder
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = fileBytes;
            newFile.Url = UrlUtility.Combine(contosoScriptFolder.ServerRelativeUrl, "UserProfileData.js");
            newFile.Overwrite = true;

            Microsoft.SharePoint.Client.File uploadFile = contosoScriptFolder.Files.Add(newFile);
            clientContext.Load(uploadFile);
            clientContext.ExecuteQuery();
        }

        protected void btnScenario2Master_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Deploy a master page to the master page gallery
                clientContext.Web.DeployMasterPage(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/contoso.master")),
                                                   "Contoso",
                                                   "Contoso master page");

                // Assign master page to the host web
                clientContext.Web.SetMasterPagesByName("contoso.master", "contoso.master");

                lblStatus2.Text = string.Format("Custom master page called 'contoso.master' has been uploaded and applied to the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            }
        }

        protected void btnScenario2Theme_Click(object sender, EventArgs e)
        {
            // Deploy custom theme to the site
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                Web web = clientContext.Web;
                // Let's first upload the contoso theme to host web, if it does not exist there
                var colorFile = web.UploadThemeFile(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/Themes/Garage/garagewhite.spcolor")));
                var backgroundFile = web.UploadThemeFile(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/Themes/Garage/garagebg.jpg")));
                web.CreateComposedLookByUrl("Garage", colorFile.ServerRelativeUrl, null, backgroundFile.ServerRelativeUrl, string.Empty);

                // Setting the Contoos theme to host web
                web.SetComposedLookByUrl("Garage");

                lblStatus2.Text = string.Format("Custom theme called 'Garage' has been uploaded and applied to the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Set master page as seattle.master
                clientContext.Web.SetMasterPageByName("seattle.master");

                // Set theme as Office
                clientContext.Web.SetComposedLookByUrl("Office");
                lblStatus2.Text = string.Format("Master page set as seattle and out of the box 'Office' theme has been uploaded and applied to the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            }
        }

        protected void btnScenario3Apply_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Apply sub site template options
                List<WebTemplateEntity> templates = new List<WebTemplateEntity>();
                templates.Add(new WebTemplateEntity() { LanguageCode = "1035", TemplateName = "STS#0" });
                templates.Add(new WebTemplateEntity() { LanguageCode = "", TemplateName = "STS#0" });
                templates.Add(new WebTemplateEntity() { LanguageCode = "", TemplateName = "BLOG#0" });
                clientContext.Web.SetAvailableWebTemplates(templates);

                // Apply available page layouts
                List<string> pageLayouts = new List<string>();
                pageLayouts.Add("ContosoLinksBelow.aspx");
                pageLayouts.Add("ContosoLinksRight.aspx");
                clientContext.Web.SetAvailablePageLayouts(clientContext.Web, pageLayouts);

                // Set default page layout for the site
                clientContext.Web.SetDefaultPageLayoutForSite(clientContext.Web, "ContosoLinksBelow.aspx");

                lblStatus3.Text = string.Format("Sub site templates filtered to specific sites. Supported page layouts and default one also changed. Check the settings from <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString() + "/_layouts/15/AreaTemplateSettings.aspx");

            }
        }

        protected void btnScenario3Clear_Click(object sender, EventArgs e)
        {
            // Clear the detault publishing settings
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Let's clear the filters, whcih will remove filtering
                clientContext.Web.ClearAvailableWebTemplates();
                // Clear page layout filter
                clientContext.Web.AllowAllPageLayouts();

            }
            lblStatus3.Text = string.Format("Sub site and page layout optiosn have been cleared. Check the settings from <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString() + "/_layouts/15/AreaTemplateSettings.aspx");

        }
    }
}