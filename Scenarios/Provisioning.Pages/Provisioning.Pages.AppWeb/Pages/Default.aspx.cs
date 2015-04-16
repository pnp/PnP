using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
//using System.Web.UI.WebControls;

namespace Contoso.Provisioning.Pages.AppWeb
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

        private string SharePointUrlParameters()
        {
            return HttpUtility.ParseQueryString(this.Context.Request.Url.Query).ToString();
        }

        protected void btnScenario1_Click(object sender, EventArgs e)
        {
            string scenario1Page = String.Format("scenario1-{0}.aspx", DateTime.Now.Ticks);
            string scenario1PageUrl = cc.Web.AddWikiPage("Site Pages", scenario1Page);
            cc.Web.AddHtmlToWikiPage("SitePages", txtHtml.Text, scenario1Page);
            this.hplScenario1.NavigateUrl = string.Format("{0}/{1}", Request.QueryString["SPHostUrl"], scenario1PageUrl);
        }

        protected void btnScenario2_Click(object sender, EventArgs e)
        {
            if (!cc.Web.ListExists("Links"))
            {
                cc.Web.CreateList(new Guid("192efa95-e50c-475e-87ab-361cede5dd7f"), 170, "Links", false);

                AddPromotedSiteLink(cc, cc.Web, "Links", "Office 365 Dev PnP", "http://aka.ms/officedevpnp");
                AddPromotedSiteLink(cc, cc.Web, "Links", "Bing", "http://www.bing.com");
            }            
            
            string scenario2Page = String.Format("scenario2-{0}.aspx", DateTime.Now.Ticks);
            string scenario2PageUrl = cc.Web.AddWikiPage("Site Pages", scenario2Page);

            bool twoColumnsOrMore = false;
            bool header = false;
            switch (drpLayouts.SelectedValue)
            {
                case "OneColumn":
                    cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.OneColumn, scenario2Page);
                    break;
                case "OneColumnSideBar":
                    cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.OneColumnSideBar, scenario2Page);
                    break;
                case "TwoColumns":
                    cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.TwoColumns, scenario2Page);
                    twoColumnsOrMore = true;
                    break;
                case "TwoColumnsHeader":
                    cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.TwoColumnsHeader, scenario2Page);
                    twoColumnsOrMore = true;
                    header = true;
                    break;
                case "TwoColumnsHeaderFooter":
                    cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.TwoColumnsHeaderFooter, scenario2Page);
                    twoColumnsOrMore = true;
                    header = true;
                    break;
                case "ThreeColumns":
                    cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.ThreeColumns, scenario2Page);
                    twoColumnsOrMore = true;
                    break;
                case "ThreeColumnsHeader":
                    cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.ThreeColumnsHeader, scenario2Page);
                    twoColumnsOrMore = true;
                    header = true;
                    break;
                case "ThreeColumnsHeaderFooter":
                    cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.ThreeColumnsHeaderFooter, scenario2Page);
                    twoColumnsOrMore = true;
                    header = true;
                    break;
                default:
                    break;
            }

            Guid linksID = cc.Web.GetListID("Links");
            WebPartEntity wp2 = new WebPartEntity();
            wp2.WebPartXml = WpPromotedLinks(linksID, string.Format("{0}/Lists/{1}", Request.QueryString["SPHostUrl"], "Links"), string.Format("{0}/{1}", Request.QueryString["SPHostUrl"], scenario2PageUrl), "$Resources:core,linksList");
            wp2.WebPartIndex = 1;
            wp2.WebPartTitle = "Links";

            int webpartRow = 1;
            if (header)
            {
                webpartRow = 2;
            }

            cc.Web.AddWebPartToWikiPage("SitePages", wp2, scenario2Page, webpartRow, 1, false);
            Session.Add("LastPageName", scenario2Page);

            if (twoColumnsOrMore)
            {
                cc.Web.AddHtmlToWikiPage("SitePages", txtHtml.Text, scenario2Page, webpartRow, 2);
            }

            this.hplScenario2.NavigateUrl = string.Format("{0}/{1}", Request.QueryString["SPHostUrl"], scenario2PageUrl);
            this.btnScenario2Remove.Enabled = true;
        }

        protected void btnScenario2Remove_Click(object sender, EventArgs e)
        {
            cc.Web.DeleteWebPart("SitePages", "Links", Session["LastPageName"].ToString());
            this.btnScenario2Remove.Enabled = false;
        }

        protected void btnCleanup_Click(object sender, EventArgs e)
        {
            DeleteDemoPages(cc, cc.Web, "SitePages");
            this.hplScenario1.NavigateUrl = "";
            this.hplScenario2.NavigateUrl = "";
            this.btnScenario2Remove.Enabled = false;
        }

        private string WpPromotedLinks(Guid listID, string listUrl, string pageUrl, string title)
        {
            StringBuilder wp = new StringBuilder(100);
            wp.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            wp.Append("<webParts>");
            wp.Append("	<webPart xmlns=\"http://schemas.microsoft.com/WebPart/v3\">");
            wp.Append("		<metaData>");
            wp.Append("			<type name=\"Microsoft.SharePoint.WebPartPages.XsltListViewWebPart, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\" />");
            wp.Append("			<importErrorMessage>Cannot import this Web Part.</importErrorMessage>");
            wp.Append("		</metaData>");
            wp.Append("		<data>");
            wp.Append("			<properties>");
            wp.Append("				<property name=\"ShowWithSampleData\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"Default\" type=\"string\" />");
            wp.Append("				<property name=\"NoDefaultStyle\" type=\"string\" null=\"true\" />");
            wp.Append("				<property name=\"CacheXslStorage\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"ViewContentTypeId\" type=\"string\" />");
            wp.Append("				<property name=\"XmlDefinitionLink\" type=\"string\" />");
            wp.Append("				<property name=\"ManualRefresh\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"ListUrl\" type=\"string\" />");
            wp.Append(String.Format("				<property name=\"ListId\" type=\"System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\">{0}</property>", listID.ToString()));
            wp.Append(String.Format("				<property name=\"TitleUrl\" type=\"string\">{0}</property>", listUrl));
            wp.Append("				<property name=\"EnableOriginalValue\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"Direction\" type=\"direction\">NotSet</property>");
            wp.Append("				<property name=\"ServerRender\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"ViewFlags\" type=\"Microsoft.SharePoint.SPViewFlags, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\">None</property>");
            wp.Append("				<property name=\"AllowConnect\" type=\"bool\">True</property>");
            wp.Append(String.Format("				<property name=\"ListName\" type=\"string\">{0}</property>", ("{" + listID.ToString().ToUpper() + "}")));
            wp.Append("				<property name=\"ListDisplayName\" type=\"string\" />");
            wp.Append("				<property name=\"AllowZoneChange\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"ChromeState\" type=\"chromestate\">Normal</property>");
            wp.Append("				<property name=\"DisableSaveAsNewViewButton\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"ViewFlag\" type=\"string\" />");
            wp.Append("				<property name=\"DataSourceID\" type=\"string\" />");
            wp.Append("				<property name=\"ExportMode\" type=\"exportmode\">All</property>");
            wp.Append("				<property name=\"AutoRefresh\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"FireInitialRow\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"AllowEdit\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"Description\" type=\"string\" />");
            wp.Append("				<property name=\"HelpMode\" type=\"helpmode\">Modeless</property>");
            wp.Append("				<property name=\"BaseXsltHashKey\" type=\"string\" null=\"true\" />");
            wp.Append("				<property name=\"AllowMinimize\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"CacheXslTimeOut\" type=\"int\">86400</property>");
            wp.Append("				<property name=\"ChromeType\" type=\"chrometype\">Default</property>");
            wp.Append("				<property name=\"Xsl\" type=\"string\" null=\"true\" />");
            wp.Append("				<property name=\"JSLink\" type=\"string\" null=\"true\" />");
            wp.Append("				<property name=\"CatalogIconImageUrl\" type=\"string\">/_layouts/15/images/itgen.png?rev=26</property>");
            wp.Append("				<property name=\"SampleData\" type=\"string\" null=\"true\" />");
            wp.Append("				<property name=\"UseSQLDataSourcePaging\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"TitleIconImageUrl\" type=\"string\" />");
            wp.Append("				<property name=\"PageSize\" type=\"int\">-1</property>");
            wp.Append("				<property name=\"ShowTimelineIfAvailable\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"Width\" type=\"string\" />");
            wp.Append("				<property name=\"DataFields\" type=\"string\" />");
            wp.Append("				<property name=\"Hidden\" type=\"bool\">False</property>");
            wp.Append(String.Format("				<property name=\"Title\" type=\"string\">{0}</property>", title));
            wp.Append("				<property name=\"PageType\" type=\"Microsoft.SharePoint.PAGETYPE, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\">PAGE_NORMALVIEW</property>");
            wp.Append("				<property name=\"DataSourcesString\" type=\"string\" />");
            wp.Append("				<property name=\"AllowClose\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"InplaceSearchEnabled\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"WebId\" type=\"System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\">00000000-0000-0000-0000-000000000000</property>");
            wp.Append("				<property name=\"Height\" type=\"string\" />");
            wp.Append("				<property name=\"GhostedXslLink\" type=\"string\">main.xsl</property>");
            wp.Append("				<property name=\"DisableViewSelectorMenu\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"DisplayName\" type=\"string\" />");
            wp.Append("				<property name=\"IsClientRender\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"XmlDefinition\" type=\"string\">");
            wp.Append(string.Format("&lt;View Name=\"{1}\" Type=\"HTML\" Hidden=\"TRUE\" ReadOnly=\"TRUE\" OrderedView=\"TRUE\" DisplayName=\"\" Url=\"{0}\" Level=\"1\" BaseViewID=\"1\" ContentTypeID=\"0x\" &gt;&lt;Query&gt;&lt;OrderBy&gt;&lt;FieldRef Name=\"TileOrder\" Ascending=\"TRUE\"/&gt;&lt;FieldRef Name=\"Modified\" Ascending=\"FALSE\"/&gt;&lt;/OrderBy&gt;&lt;/Query&gt;&lt;ViewFields&gt;&lt;FieldRef Name=\"Title\"/&gt;&lt;FieldRef Name=\"BackgroundImageLocation\"/&gt;&lt;FieldRef Name=\"Description\"/&gt;&lt;FieldRef Name=\"LinkLocation\"/&gt;&lt;FieldRef Name=\"LaunchBehavior\"/&gt;&lt;FieldRef Name=\"BackgroundImageClusterX\"/&gt;&lt;FieldRef Name=\"BackgroundImageClusterY\"/&gt;&lt;/ViewFields&gt;&lt;RowLimit Paged=\"TRUE\"&gt;30&lt;/RowLimit&gt;&lt;JSLink&gt;sp.ui.tileview.js&lt;/JSLink&gt;&lt;XslLink Default=\"TRUE\"&gt;main.xsl&lt;/XslLink&gt;&lt;Toolbar Type=\"Standard\"/&gt;&lt;/View&gt;</property>", pageUrl, ("{" + Guid.NewGuid().ToString() + "}")));
            wp.Append("				<property name=\"InitialAsyncDataFetch\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"AllowHide\" type=\"bool\">True</property>");
            wp.Append("				<property name=\"ParameterBindings\" type=\"string\">");
            wp.Append("  &lt;ParameterBinding Name=\"dvt_sortdir\" Location=\"Postback;Connection\"/&gt;");
            wp.Append("            &lt;ParameterBinding Name=\"dvt_sortfield\" Location=\"Postback;Connection\"/&gt;");
            wp.Append("            &lt;ParameterBinding Name=\"dvt_startposition\" Location=\"Postback\" DefaultValue=\"\"/&gt;");
            wp.Append("            &lt;ParameterBinding Name=\"dvt_firstrow\" Location=\"Postback;Connection\"/&gt;");
            wp.Append("            &lt;ParameterBinding Name=\"OpenMenuKeyAccessible\" Location=\"Resource(wss,OpenMenuKeyAccessible)\" /&gt;");
            wp.Append("            &lt;ParameterBinding Name=\"open_menu\" Location=\"Resource(wss,open_menu)\" /&gt;");
            wp.Append("            &lt;ParameterBinding Name=\"select_deselect_all\" Location=\"Resource(wss,select_deselect_all)\" /&gt;");
            wp.Append("            &lt;ParameterBinding Name=\"idPresEnabled\" Location=\"Resource(wss,idPresEnabled)\" /&gt;&lt;ParameterBinding Name=\"NoAnnouncements\" Location=\"Resource(wss,noXinviewofY_LIST)\" /&gt;&lt;ParameterBinding Name=\"NoAnnouncementsHowTo\" Location=\"Resource(wss,noXinviewofY_DEFAULT)\" /&gt;</property>");
            wp.Append("				<property name=\"DataSourceMode\" type=\"Microsoft.SharePoint.WebControls.SPDataSourceMode, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\">List</property>");
            wp.Append("				<property name=\"AutoRefreshInterval\" type=\"int\">60</property>");
            wp.Append("				<property name=\"AsyncRefresh\" type=\"bool\">False</property>");
            wp.Append("				<property name=\"HelpUrl\" type=\"string\" />");
            wp.Append("				<property name=\"MissingAssembly\" type=\"string\">Cannot import this Web Part.</property>");
            wp.Append("				<property name=\"XslLink\" type=\"string\" null=\"true\" />");
            wp.Append("				<property name=\"SelectParameters\" type=\"string\" />");
            wp.Append("			</properties>");
            wp.Append("		</data>");
            wp.Append("	</webPart>");
            wp.Append("</webParts>");
            return wp.ToString();
        }

        public void AddPromotedSiteLink(ClientContext cc, Web web, string listName, string title, string url)
        {
            List listToInsertTo = web.Lists.GetByTitle(listName);
            ListItemCreationInformation lici = new ListItemCreationInformation();
            ListItem listItem = listToInsertTo.AddItem(lici);
            listItem["Title"] = title;
            listItem["LinkLocation"] = url;
            listItem["LaunchBehavior"] = "New tab";
            listItem["TileOrder"] = 1;
            listItem.Update();
            cc.ExecuteQuery();
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
                if (aspxFile.Name.StartsWith("scenario1", StringComparison.InvariantCultureIgnoreCase) || aspxFile.Name.StartsWith("scenario2", StringComparison.InvariantCultureIgnoreCase))
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