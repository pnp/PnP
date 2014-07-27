using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.DialogWeb
{
    public partial class Default : System.Web.UI.Page
    {
        private ClientContext cc;

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

            if (Page.IsPostBack)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                cc = spContext.CreateUserClientContextForSPHost();
            }
        }

        protected void btnAddCustomAction_Click(object sender, EventArgs e)
        {
            //Prepare the javascript for the open in dialog action
            StringBuilder modelDialogScript = new StringBuilder(10);
            modelDialogScript.Append("javascript:var dlg=SP.UI.ModalDialog.showModalDialog({url: '");
            modelDialogScript.Append(String.Format("{0}", SetIsDlg("1")));
            modelDialogScript.Append("', dialogReturnValueCallback:function(res, val) {} });");       
            
            //Create a custom action
            CustomActionEntity customAction = new CustomActionEntity()
            {
                Title = "Office AMS Dialog sample",                
                Description = "Shows how to launch an app inside a dialog",
                Location = "Microsoft.SharePoint.StandardMenu",
                Group = "SiteActions",
                Sequence = 10000,
                Url = modelDialogScript.ToString(),
            };

            //Add the custom action to the site
            cc.Web.AddCustomAction(customAction);
        }

        /// <summary>
        /// Updates the IsDlg url parameter value and returns an updated url
        /// </summary>
        /// <param name="isDlgValue">value for IsDlg url param (1 or 0)</param>
        /// <returns>An updated url</returns>
        private string SetIsDlg(string isDlgValue)
        {
            var urlParams = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            urlParams.Set("IsDlg", isDlgValue);
            return string.Format("{0}://{1}:{2}{3}?{4}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, Request.Url.AbsolutePath, urlParams.ToString());
        }

        protected void btnRemoveCustomAction_Click(object sender, EventArgs e)
        {
            //Remove the custom action. Lookup of an existing action is done based on the description and location fields. When an action is 
            //added we always remove the old one and then add a new one. If Remove=true is set then the method bails out after the removal part
            CustomActionEntity customAction = new CustomActionEntity()
            {
                Description = "Shows how to launch an app inside a dialog",
                Location = "Microsoft.SharePoint.StandardMenu",
                Remove = true,
            };
            cc.Web.AddCustomAction(customAction);

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // redirect to host web home page
            Response.Redirect(Page.Request["SPHostUrl"]);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            if (Page.Request["IsDlg"].Equals("0", StringComparison.InvariantCultureIgnoreCase))
            {
                // redirect to host web home page
                Response.Redirect(Page.Request["SPHostUrl"]);
            }
            else
            {
                // refresh the page from which the dialog was opened. Normally this is always the SPHostUrl
                ClientScript.RegisterStartupScript(this.GetType(), "RedirectToSite", "navigateParent('" + Page.Request["SPHostUrl"] + "');", true);
            }
        }

        /// <summary>
        /// Returns the web part XML for the script editor web part
        /// </summary>
        /// <returns>web part XML for the script editor web part</returns>
        private string ScriptEditorWebPart()
        {
            StringBuilder sb = new StringBuilder(20);
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.Append("<webParts>");
            sb.Append("	<webPart xmlns=\"http://schemas.microsoft.com/WebPart/v3\">");
            sb.Append("		<metaData>");
            sb.Append("			<type name=\"Microsoft.SharePoint.WebPartPages.ScriptEditorWebPart, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\" />");
            sb.Append("			<importErrorMessage>Cannot import this Web Part.</importErrorMessage>");
            sb.Append("		</metaData>");
            sb.Append("		<data>");
            sb.Append("			<properties>");
            sb.Append("				<property name=\"ExportMode\" type=\"exportmode\">All</property>");
            sb.Append("				<property name=\"HelpUrl\" type=\"string\" />");
            sb.Append("				<property name=\"Hidden\" type=\"bool\">False</property>");
            sb.Append("				<property name=\"Description\" type=\"string\">Allows authors to insert HTML snippets or scripts.</property>");
            sb.Append("             <property name=\"Content\" type=\"string\">&lt;a id=\"newSiteLink\" onclick=\"javascript: var dlg=SP.UI.ModalDialog.showModalDialog({url:'" + HttpUtility.HtmlEncode(SetIsDlg("1")) + "', dialogReturnValueCallback:function(res, val) {} }); CancelEvent(event); return false;\" href=\"#\"&gt;Open in dialog&lt;/a&gt;");
            sb.Append("</property>");
            sb.Append("				<property name=\"CatalogIconImageUrl\" type=\"string\" />");
            sb.Append("				<property name=\"Title\" type=\"string\">Script Editor</property>");
            sb.Append("				<property name=\"AllowHide\" type=\"bool\">True</property>");
            sb.Append("				<property name=\"AllowMinimize\" type=\"bool\">True</property>");
            sb.Append("				<property name=\"AllowZoneChange\" type=\"bool\">True</property>");
            sb.Append("				<property name=\"TitleUrl\" type=\"string\" />");
            sb.Append("				<property name=\"ChromeType\" type=\"chrometype\">None</property>");
            sb.Append("				<property name=\"AllowConnect\" type=\"bool\">True</property>");
            sb.Append("				<property name=\"Width\" type=\"unit\" />");
            sb.Append("				<property name=\"Height\" type=\"unit\" />");
            sb.Append("				<property name=\"HelpMode\" type=\"helpmode\">Navigate</property>");
            sb.Append("				<property name=\"AllowEdit\" type=\"bool\">True</property>");
            sb.Append("				<property name=\"TitleIconImageUrl\" type=\"string\" />");
            sb.Append("				<property name=\"Direction\" type=\"direction\">NotSet</property>");
            sb.Append("				<property name=\"AllowClose\" type=\"bool\">True</property>");
            sb.Append("				<property name=\"ChromeState\" type=\"chromestate\">Normal</property>");
            sb.Append("			</properties>");
            sb.Append("		</data>");
            sb.Append("	</webPart>");
            sb.Append("</webParts>");

            return sb.ToString();
        }

        protected void btnAddDialogLinkOnPage_Click(object sender, EventArgs e)
        {
            string scenario1Page = String.Format("scenario1-{0}.aspx", DateTime.Now.Ticks);
            string scenario1PageUrl = cc.Web.AddWikiPage("Site Pages", scenario1Page);
            cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.OneColumn, scenario1Page);
            WebPartEntity scriptEditorWp = new WebPartEntity();
            scriptEditorWp.WebPartXml = ScriptEditorWebPart();
            scriptEditorWp.WebPartIndex = 1;
            scriptEditorWp.WebPartTitle = "Script editor test";            
            cc.Web.AddWebPartToWikiPage("SitePages", scriptEditorWp, scenario1Page, 1, 1, false);
            this.hplScenario1.NavigateUrl = string.Format("{0}/{1}", Request.QueryString["SPHostUrl"], scenario1PageUrl);
        }

        protected void btnCleanup_Click(object sender, EventArgs e)
        {
            DeleteDemoPages(cc, cc.Web, "SitePages");
            this.hplScenario1.NavigateUrl = "";
        }

        public void DeleteDemoPages(ClientContext cc, Web web, string folder)
        {
            //Note: getfilebyserverrelativeurl did not work...not sure why not
            Microsoft.SharePoint.Client.Folder pagesLib = web.GetFolderByServerRelativeUrl(folder);
            cc.Load(pagesLib.Files);
            cc.ExecuteQuery();

            List<File> toDelete = new List<File>();

            foreach (Microsoft.SharePoint.Client.File aspxFile in pagesLib.Files)
            {
                if (aspxFile.Name.StartsWith("scenario1", StringComparison.InvariantCultureIgnoreCase))
                {
                    toDelete.Add(aspxFile);
                }
            }

            for (int i = 0; i < toDelete.Count; i++)
            {
                toDelete[i].DeleteObject();
            }

            cc.ExecuteQuery();
        }

    }
}