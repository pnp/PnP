using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.ModifyPagesWeb
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

            //register script in page which shows the content when chrome is loaded
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);

        }

        protected void btnCreateNewPage_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                string scenarioPage = String.Format("scenario1-{0}.aspx", DateTime.Now.Ticks);
                string scenarioPageUrl = AddWikiPage(ctx, ctx.Web, "Site Pages", scenarioPage);
                AddHtmlToWikiPage(ctx, ctx.Web, "SitePages", htmlEntry.Text, scenarioPage);
                hplPage.NavigateUrl = string.Format("{0}/{1}", Request.QueryString["SPHostUrl"], scenarioPageUrl);
            }
        }

        public string AddWikiPage(ClientContext ctx, Web web, string folder, string wikiPageName)
        {
            string wikiPageUrl = "";
            var pageLibrary = web.Lists.GetByTitle(folder);

            ctx.Load(pageLibrary.RootFolder, f => f.ServerRelativeUrl);
            ctx.ExecuteQuery();

            var pageLibraryUrl = pageLibrary.RootFolder.ServerRelativeUrl;
            var newWikiPageUrl = pageLibraryUrl + "/" + wikiPageName;
            var currentPageFile = web.GetFileByServerRelativeUrl(newWikiPageUrl);
            ctx.Load(currentPageFile, f => f.Exists);
            ctx.ExecuteQuery();

            if (!currentPageFile.Exists)
            {
                var newpage = pageLibrary.RootFolder.Files.AddTemplateFile(newWikiPageUrl, TemplateFileType.WikiPage);
                ctx.Load(newpage);
                ctx.ExecuteQuery();
                wikiPageUrl = String.Format("sitepages/{0}", wikiPageName);
            }

            return wikiPageUrl;
        }

        public void AddHtmlToWikiPage(ClientContext ctx, Web web, string folder, string html, string page)
        {
            Microsoft.SharePoint.Client.Folder pagesLib = web.GetFolderByServerRelativeUrl(folder);
            ctx.Load(pagesLib.Files);
            ctx.ExecuteQuery();

            Microsoft.SharePoint.Client.File wikiPage = null;

            foreach (Microsoft.SharePoint.Client.File aspxFile in pagesLib.Files)
            {
                if (aspxFile.Name.Equals(page, StringComparison.InvariantCultureIgnoreCase))
                {
                    wikiPage = aspxFile;
                    break;
                }
            }

            if (wikiPage == null)
            {
                return;
            }

            ctx.Load(wikiPage);
            ctx.Load(wikiPage.ListItemAllFields);
            ctx.ExecuteQuery();

            string wikiField = (string)wikiPage.ListItemAllFields["WikiField"];

            Microsoft.SharePoint.Client.ListItem listItem = wikiPage.ListItemAllFields;
            listItem["WikiField"] = html;
            listItem.Update();
            ctx.ExecuteQuery();
        }



        protected void btnCreatePageWithWebPart_Click(object sender, EventArgs e)
        {

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                if (new LabHelper().AddList(ctx, ctx.Web, 170, new Guid("192efa95-e50c-475e-87ab-361cede5dd7f"), "Links", false))
                {
                    new LabHelper().AddPromotedSiteLink(ctx, ctx.Web, "Links", "OfficeAMS on CodePlex", "http://officeams.codeplex.com");
                    new LabHelper().AddPromotedSiteLink(ctx, ctx.Web, "Links", "Bing", "http://www.bing.com");
                }

                string scenario2Page = String.Format("scenario2-{0}.aspx", DateTime.Now.Ticks);
                string scenario2PageUrl = AddWikiPage(ctx, ctx.Web, "Site Pages", scenario2Page);

                AddHtmlToWikiPage(ctx, ctx.Web, "SitePages", LabHelper.WikiPage_ThreeColumnsHeaderFooter, scenario2Page);

                Guid linksID = new LabHelper().GetListID(ctx, ctx.Web, "Links");
                WebPartEntity wp2 = new WebPartEntity();
                wp2.WebPartXml = new LabHelper().WpPromotedLinks(linksID, string.Format("{0}/Lists/{1}", 
                                                                Request.QueryString["SPHostUrl"], "Links"), 
                                                                string.Format("{0}/{1}", Request.QueryString["SPHostUrl"], 
                                                                scenario2PageUrl), "$Resources:core,linksList");
                wp2.WebPartIndex = 1;
                wp2.WebPartTitle = "Links";

                new LabHelper().AddWebPartToWikiPage(ctx, ctx.Web, "SitePages", wp2, scenario2Page, 2, 1, false);
                new LabHelper().AddHtmlToWikiPage(ctx, ctx.Web, "SitePages", htmlEntry.Text, scenario2Page, 2, 2);

                this.hplPage2.NavigateUrl = string.Format("{0}/{1}", Request.QueryString["SPHostUrl"], scenario2PageUrl);
            }
        }
    }
}