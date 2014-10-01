using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.SharePoint.Client.Utilities;
using Contoso.Provisioning.Hybrid.Contract;
using System.IO;
using OfficeDevPnP.Core.Utilities;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;

namespace Contoso.Provisioning.Hybrid.Core.SiteTemplates
{
    public class ContosoCollaboration : SiteProvisioningBase
    {
        public override bool Execute()
        {
            bool processed = false;

            string generalSiteDirectoryUrl = GetConfiguration("General.SiteDirectoryUrl");
            string generalSiteDirectoryListName = GetConfiguration("General.SiteDirectoryListName");
            string generalSiteCollectionUrl = GetConfiguration("General.SiteCollectionUrl");
            string generalMailSMTPServer = GetConfiguration("General.MailSMTPServer");
            string generalMailUser = GetConfiguration("General.MailUser");
            string generalMailUserPassword = GetConfiguration("General.MailUserPassword");
            string generalMailSiteAvailable = GetConfiguration("General.MailSiteAvailable");
            string generalEncryptionThumbPrint = GetConfiguration("General.EncryptionThumbPrint");
            //Decrypt mail password
            generalMailUserPassword = EncryptionUtility.Decrypt(generalMailUserPassword, generalEncryptionThumbPrint);
            string contosoCollaborationPromotedSiteName = GetConfiguration("ContosoCollaboration.PromotedSiteName");
            string contosoCollaborationPromotedSiteUrl = GetConfiguration("ContosoCollaboration.PromotedSiteUrl");    
            string contosoCollaborationThemeName = GetConfiguration("ContosoCollaboration.ThemeName");                
            //On-Prem settings
            string generalOnPremWebApplication = GetConfiguration("General.OnPremWebApplication");

            try
            {
                SiteDirectoryManager siteDirectoryManager = new SiteDirectoryManager();
                //FeatureManager featureManager = new FeatureManager();
                //ListManager listManager = new ListManager();
                //PageManager pageManager = new PageManager();
                //SecurityManager securityManager = new SecurityManager();
                //NavigationManager navigationManager = new NavigationManager();
                //BrandingManager brandingManager = new BrandingManager();


                string tempSharePointUrl = this.SharePointProvisioningData.Url;
                string siteCollectionUrl = this.CreateOnPremises ? generalOnPremWebApplication : generalSiteCollectionUrl;

                // issue the final SharePoint url
                SharePointProvisioningData.Url = this.GetNextSiteCollectionUrl(generalSiteDirectoryUrl, generalSiteDirectoryListName, siteCollectionUrl);
                
                //update site directory status
                siteDirectoryManager.UpdateSiteDirectoryStatus(this.SiteDirectorySiteContext, this.SiteDirectorySiteContext.Web, generalSiteDirectoryUrl, generalSiteDirectoryListName, tempSharePointUrl, this.SharePointProvisioningData.Url, "Provisioning");

                //complete the site data
                this.SharePointProvisioningData.Template = "STS#0";
                this.SharePointProvisioningData.SiteOwner = this.SharePointProvisioningData.Owners[0];
                this.SharePointProvisioningData.Lcid = 1033;
                this.SharePointProvisioningData.TimeZoneId = 3;
                this.SharePointProvisioningData.StorageMaximumLevel = 100;
                this.SharePointProvisioningData.StorageWarningLevel = 80;

                //create the site collection
                this.AddSiteCollection(this.SharePointProvisioningData);

                //enable features
                // Document ID Service (DocID) site collection feature
                this.CreatedSiteContext.Site.ActivateFeature(new Guid("b50e3104-6812-424f-a011-cc90e6327318"));
                // Search Server Web Parts and Templates (SearchMaster) site collection feature
                this.CreatedSiteContext.Site.ActivateFeature(new Guid("9c0834e1-ba47-4d49-812b-7d4fb6fea211"));
                // Workflows (Workflows) site collection feature
                this.CreatedSiteContext.Site.ActivateFeature(new Guid("0af5989a-3aea-4519-8ab0-85d91abe39ff"));
                // Metadata Navigation and Filtering (MetaDataNav) site feature
                this.CreatedSiteContext.Web.ActivateFeature(new Guid("7201d6a4-a5d3-49a1-8c19-19c4bac6e668"));
                // Community Site Feature (CommunitySite) site feature
                this.CreatedSiteContext.Web.ActivateFeature(new Guid("961d6a9c-4388-4cf2-9733-38ee8c89afd4"));
                // Project Functionality (ProjectFunctionality) site feature
                this.CreatedSiteContext.Web.ActivateFeature(new Guid("e2f2bb18-891d-4812-97df-c265afdba297"));

                // Picture library called Media
                this.CreatedSiteContext.Web.AddList(Microsoft.SharePoint.Client.ListTemplateType.PictureLibrary, "Media", false);
                // Promoted Links library called Links
                this.CreatedSiteContext.Web.AddList(170, new Guid("192efa95-e50c-475e-87ab-361cede5dd7f"), "Links", false);

                // Update existing list settings for the documents library and the blog post library
                this.CreatedSiteContext.Web.UpdateListVersioning("Documents", true);

                //Remove the "Project Summary" web part
                this.CreatedSiteContext.Web.DeleteWebPart("SitePages", "Project Summary", "home.aspx");
                //Remove the "Get started with your site" web part
                this.CreatedSiteContext.Web.DeleteWebPart("SitePages", "Get started with your site", "home.aspx");
                //Remove the "Documents" web part
                this.CreatedSiteContext.Web.DeleteWebPart("SitePages", "Documents", "home.aspx");

                //Add links web part to the home page
                Guid linksID = this.CreatedSiteContext.Web.GetListID("Links");
                WebPartEntity promotedLinksWP = new WebPartEntity();
                promotedLinksWP.WebPartXml = WpPromotedLinks(linksID, string.Format("{0}{1}/Lists/{2}", this.SharePointProvisioningData.Url, this.SharePointProvisioningData.Name, "Links"), string.Format("{0}{1}/SitePages/{2}", this.SharePointProvisioningData.Url, this.SharePointProvisioningData.Name, "home.aspx"), "$Resources:core,linksList");
                promotedLinksWP.WebPartIndex = 2;
                promotedLinksWP.WebPartTitle = "Links";
                this.CreatedSiteContext.Web.AddWebPartToWikiPage("SitePages", promotedLinksWP, "home.aspx", 2, 2, false);

                //Add html to the home page wiki
                this.CreatedSiteContext.Web.AddHtmlToWikiPage("SitePages", "Hello <strong>SharePoint Conference</strong> from spc403<br/><br/><br/>", "home.aspx", 1, 1);

                //add additional pages
                string siteMembersPage = "site members.aspx";
                string siteMembersUrl = this.CreatedSiteContext.Web.AddWikiPage("Site Pages", siteMembersPage);
                this.CreatedSiteContext.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.TwoColumns, siteMembersPage);
                //Add site members web parts
                WebPartEntity wpSiteUsers = new WebPartEntity();
                wpSiteUsers.WebPartXml = WpSiteUsers("Site owners", this.CreatedSiteContext.Web.GetGroupID(String.Format("{0} {1}", this.SharePointProvisioningData.Title, "Owners")));
                wpSiteUsers.WebPartIndex = 0;
                wpSiteUsers.WebPartTitle = "Site owners";
                this.CreatedSiteContext.Web.AddWebPartToWikiPage("SitePages", wpSiteUsers, siteMembersPage, 1, 1, false);

                wpSiteUsers.WebPartXml = WpSiteUsers("Site visitors", this.CreatedSiteContext.Web.GetGroupID(String.Format("{0} {1}", this.SharePointProvisioningData.Title, "Visitors")));
                wpSiteUsers.WebPartIndex = 1;
                wpSiteUsers.WebPartTitle = "Site visitors";
                this.CreatedSiteContext.Web.AddWebPartToWikiPage("SitePages", wpSiteUsers, siteMembersPage, 1, 1, true);

                wpSiteUsers.WebPartXml = WpSiteUsers("Site members", this.CreatedSiteContext.Web.GetGroupID(String.Format("{0} {1}", this.SharePointProvisioningData.Title, "Members")));
                wpSiteUsers.WebPartIndex = 0;
                wpSiteUsers.WebPartTitle = "Site members";
                this.CreatedSiteContext.Web.AddWebPartToWikiPage("SitePages", wpSiteUsers, siteMembersPage, 1, 2, false);

                //Update the quick launch navigation
                //First delete all quicklaunch entries
                this.CreatedSiteContext.Web.DeleteAllQuickLaunchNodes();

                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:core,nav_Home", null, "", true);
                //csomService.AddNavigationNode(this.SiteToProvision, "$Resources:core,BlogQuickLaunchTitle", new Uri(this.BlogSite.Url), "News & Trending", true);
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:spscore,DiscussionsTab", new Uri(string.Format("{0}/Lists/Community%20Discussion/AllItems.aspx", this.SharePointProvisioningData.Url)), "", true);
                string notebookPath = string.Format("{0}/SiteAssets/{1} Notebook", this.SharePointProvisioningData.Url, this.SharePointProvisioningData.Title);
                notebookPath = HttpUtility.UrlPathEncode(notebookPath, false).Replace("/", "%2F");
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:core,SiteNotebookLink", new Uri(string.Format("{0}/_layouts/15/WopiFrame.aspx?sourcedoc={1}&action=editnew", this.SharePointProvisioningData.Url, notebookPath)), "", true);
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:core,taskList", new Uri(string.Format("{0}/Lists/Tasks/AllItems.aspx", this.SharePointProvisioningData.Url)), "", true);
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:core,calendarList", new Uri(string.Format("{0}/Lists/Calendar/calendar.aspx", this.SharePointProvisioningData.Url)), "", true);
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:core,shareddocuments_Title_15", new Uri(string.Format("{0}/Shared Documents/Forms/AllItems.aspx", this.SharePointProvisioningData.Url)), "", true);
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:core,linksList", new Uri(string.Format("{0}/Lists/Links/Tiles.aspx", this.SharePointProvisioningData.Url)), "", true);
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:core,GrpMedia", new Uri(string.Format("{0}/Media/Forms/Thumbnails.aspx", this.SharePointProvisioningData.Url)), "", true);
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:spscore,Members_QuickLaunch", new Uri(string.Format("{0}/{1}", this.SharePointProvisioningData.Url, siteMembersUrl)), "", true);
                this.CreatedSiteContext.Web.AddNavigationNode("$Resources:core,category_SiteContents", new Uri(string.Format("{0}/_layouts/15/viewlsts.aspx", this.SharePointProvisioningData.Url)), "", true);

                // Insert demo promoted links list item
                if (contosoCollaborationPromotedSiteName.Length > 0 || contosoCollaborationPromotedSiteUrl.Length > 0)
                {
                    AddPromotedSiteLink(this.CreatedSiteContext, this.CreatedSiteContext.Web, "Links", contosoCollaborationPromotedSiteName, contosoCollaborationPromotedSiteUrl);
                }

                if (!this.CreateOnPremises)
                {
                    // add owners to site collection administrators
                    string[] ownerLogins = new string[this.SharePointProvisioningData.Owners.Length];
                    int i = 0;
                    foreach (SharePointUser owner in this.SharePointProvisioningData.Owners)
                    {
                        ownerLogins[i] = owner.Login;
                        i++;
                    }
                    this.AppOnlyClientContext.Web.AddAdministratorsTenant(ownerLogins, new Uri(this.SharePointProvisioningData.Url));

                    // Everyone reader
                    this.CreatedSiteContext.Web.AddReaderAccess();
                }

                // Apply themes
                string themeRoot = Path.Combine(this.AppRootPath, String.Format(@"Themes\{0}", contosoCollaborationThemeName));
                string spColorFile = Path.Combine(themeRoot, string.Format("{0}.spcolor", contosoCollaborationThemeName));
                string spFontFile = Path.Combine(themeRoot, string.Format("{0}.spfont", contosoCollaborationThemeName));
                string backgroundFile = Path.Combine(themeRoot, string.Format("{0}bg.jpg", contosoCollaborationThemeName));
                string logoFile = Path.Combine(themeRoot, string.Format("{0}logo.png", contosoCollaborationThemeName));
                this.CreatedSiteContext.Web.DeployThemeToWeb(contosoCollaborationThemeName, spColorFile, spFontFile, backgroundFile, "");
                this.CreatedSiteContext.Web.SetThemeToWeb(contosoCollaborationThemeName);
                
                //Seems to be broken at the moment...to be investigated
                //brandingManager.SetSiteLogo(this.CreatedSiteContext, this.CreatedSiteContext.Web, logoFile);

                // Update status
                siteDirectoryManager.UpdateSiteDirectoryStatus(this.SiteDirectorySiteContext, this.SiteDirectorySiteContext.Web, generalSiteDirectoryUrl, generalSiteDirectoryListName, this.SharePointProvisioningData.Url, "Available");

                // Send mail to owners
                List<String> mailTo = new List<string>();
                string ownerNames = "";
                string ownerAccounts = "";

                foreach (SharePointUser owner in this.SharePointProvisioningData.Owners)
                {
                    mailTo.Add(owner.Email);

                    if (ownerNames.Length > 0)
                    {
                        ownerNames = ownerNames + ", ";
                        ownerAccounts = ownerAccounts + ", ";
                    }
                    ownerNames = ownerNames + owner.Name;
                    ownerAccounts = ownerAccounts + owner.Login;
                }

                // send email to notify the use of successful provisioning
                string mailBody = String.Format(generalMailSiteAvailable, this.SharePointProvisioningData.Title, this.SharePointProvisioningData.Url, ownerNames, ownerAccounts);
                MailUtility.SendEmail(generalMailSMTPServer, generalMailUser, generalMailUserPassword, mailTo, null, "Your SharePoint site is ready to be used", mailBody);

            }
            catch (Exception ex)
            {
                //log error
                new SiteDirectoryManager().UpdateSiteDirectoryStatus(this.SiteDirectorySiteContext, this.SiteDirectorySiteContext.Web, generalSiteDirectoryUrl, generalSiteDirectoryListName, this.SharePointProvisioningData.Url, "Error during provisioning", ex);
            }

            return processed;
        }

        /// <summary>
        /// Constructs the webpart XML needed to inject the promoted links web part to the home page of the team site
        /// </summary>
        /// <param name="listID">ID of the promoted links list</param>
        /// <param name="listUrl">URL of the list</param>
        /// <param name="pageUrl">URL of the page that will host the webpart</param>
        /// <param name="title">Title of the web part</param>
        /// <returns>The constructed web part XML</returns>
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

        /// <summary>
        /// Constructs the webpart XML needed to inject the site users web part to the site members page of the team site
        /// </summary>
        /// <param name="title">Title of the web part</param>
        /// <param name="groupID">Integer group id of the SharePoint group to be shown in the site users web part</param>
        /// <returns>The constructed web part XML</returns>
        private string WpSiteUsers(string title, int groupID)
        {
            StringBuilder sb = new StringBuilder(25);

            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<WebPart xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/WebPart/v2\">");
            sb.Append(string.Format("	<Title>{0}</Title>", title));
            sb.Append("	<FrameType>Default</FrameType>");
            sb.Append("	<Description>Use the Site Users Web Part to see a list of the site users and their online status.</Description>");
            sb.Append("	<IsIncluded>true</IsIncluded>");
            sb.Append("	<ZoneID>wpz</ZoneID>");
            sb.Append("	<PartOrder>1</PartOrder>");
            sb.Append("	<FrameState>Normal</FrameState>");
            sb.Append("	<Height />");
            sb.Append("	<Width />");
            sb.Append("	<AllowRemove>true</AllowRemove>");
            sb.Append("	<AllowZoneChange>true</AllowZoneChange>");
            sb.Append("	<AllowMinimize>true</AllowMinimize>");
            sb.Append("	<AllowConnect>true</AllowConnect>");
            sb.Append("	<AllowEdit>true</AllowEdit>");
            sb.Append("	<AllowHide>true</AllowHide>");
            sb.Append("	<IsVisible>true</IsVisible>");
            sb.Append("	<DetailLink />");
            sb.Append("	<HelpLink />");
            sb.Append("	<HelpMode>Modeless</HelpMode>");
            sb.Append("	<Dir>Default</Dir>");
            sb.Append("	<PartImageSmall />");
            sb.Append("	<MissingAssembly>Cannot import this Web Part.</MissingAssembly>");
            sb.Append("	<PartImageLarge>/_layouts/15/images/msmeml.gif</PartImageLarge>");
            sb.Append("	<IsIncludedFilter />");
            sb.Append("	<Assembly>Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>");
            sb.Append("	<TypeName>Microsoft.SharePoint.WebPartPages.MembersWebPart</TypeName>");
            sb.Append("	<DisplayType xmlns=\"http://schemas.microsoft.com/WebPart/v2/Members\">GroupMembership</DisplayType>");
            sb.Append(string.Format("	<MembershipGroupId xmlns=\"http://schemas.microsoft.com/WebPart/v2/Members\">{0}</MembershipGroupId>", groupID));
            sb.Append("	<Toolbar xmlns=\"http://schemas.microsoft.com/WebPart/v2/Members\">false</Toolbar>");
            sb.Append("</WebPart>");

            return sb.ToString();
        }

        /// <summary>
        /// Inserts an item to the promoted links list
        /// </summary>
        /// <param name="listName">List to operate on</param>
        /// <param name="title">Title of the promoted link</param>
        /// <param name="url">Url of the promoted link</param>
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
    }
}
