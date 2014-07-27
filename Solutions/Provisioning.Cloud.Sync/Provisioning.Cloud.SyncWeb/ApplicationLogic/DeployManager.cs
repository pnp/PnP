using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Xml.Linq;

namespace Contoso.Provisioning.Cloud.SyncWeb.ApplicationLogic
{
    /// <summary>
    /// Actual code on manipulating the created sites based on templates
    /// </summary>
    public class DeployManager
    {

        public const string ScriptLocation = "ScriptLink";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostWebUrl"></param>
        /// <param name="txtUrl"></param>
        /// <param name="template"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="cc"></param>
        /// <param name="page"></param>
        /// <param name="baseConfiguration"></param>
        /// <returns></returns>
        public Web CreateSiteCollection(string hostWebUrl, string txtUrl, string template, string title, string description,
                                    Microsoft.SharePoint.Client.ClientContext cc, Page page, XDocument baseConfiguration)
        {
            //get the template element
            XElement templateConfig = GetTemplateConfig(template, baseConfiguration);
            string siteTemplate = SolveUsedTemplate(template, templateConfig);

            //get the base tenant admin urls
            var tenantStr = hostWebUrl.ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));

            //get the current user to set as owner
            var currUser = cc.Web.CurrentUser;
            cc.Load(currUser);
            cc.ExecuteQuery();

            //create site collection using the Tenant object
            var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, templateConfig.Attribute("ManagedPath").Value, txtUrl);
            var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantStr));
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                var tenant = new Tenant(adminContext);
                var properties = new SiteCreationProperties()
                {
                    Url = webUrl,
                    Owner = currUser.Email,
                    Title = title,
                    Template = siteTemplate,
                    StorageMaximumLevel = Convert.ToInt32(templateConfig.Attribute("StorageMaximumLevel").Value),
                    UserCodeMaximumLevel = Convert.ToDouble(templateConfig.Attribute("UserCodeMaximumLevel").Value)
                };

                //start the SPO operation to create the site
                SpoOperation op = tenant.CreateSite(properties);
                adminContext.Load(tenant);
                adminContext.Load(op, i => i.IsComplete);
                adminContext.ExecuteQuery();

                //check if site creation operation is complete
                while (!op.IsComplete)
                {
                    //wait 30seconds and try again
                    System.Threading.Thread.Sleep(30000);
                    op.RefreshLoad();
                    adminContext.ExecuteQuery();
                }
            }

            //get the new site collection
            var siteUri = new Uri(webUrl);
            token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;
            using (var newWebContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), token))
            {
                var newWeb = newWebContext.Web;
                newWebContext.Load(newWeb);
                newWebContext.ExecuteQuery();

                //process the remiander of the template configuration
                DeployFiles(newWebContext, newWeb, templateConfig);
                DeployCustomActions(newWebContext, newWeb, templateConfig);
                DeployLists(newWebContext, newWeb, templateConfig);
                DeployNavigation(newWebContext, newWeb, templateConfig);
                DeployTheme(newWebContext, newWeb, templateConfig, baseConfiguration);
                SetSiteLogo(newWebContext, newWeb, templateConfig);

                // All done, let's return the newly created site
                return newWeb;
            }
        }

        /// <summary>
        /// This is simple demo on sub site creation based on selected "template" with configurable options
        /// </summary>
        /// <param name="txtUrl"></param>
        /// <param name="template"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="cc"></param>
        /// <param name="page"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public Web CreateSubSite(string txtUrl, string template, string title, string description,
                                    Microsoft.SharePoint.Client.ClientContext cc, Page page, XDocument baseConfiguration,
                                    bool isChildSite = false, Web subWeb = null)
        {
            // Resolve the template configuration to be used for chosen template
            XElement templateConfig = GetTemplateConfig(template, baseConfiguration);
            string siteTemplate = SolveUsedTemplate(template, templateConfig);

            // Create web creation configuration
            WebCreationInformation information = new WebCreationInformation();
            information.WebTemplate = siteTemplate;
            information.Description = description;
            information.Title = title;
            information.Url = txtUrl;
            // Currently all english, could be extended to be configurable based on language pack usage
            information.Language = 1033;

            Microsoft.SharePoint.Client.Web newWeb = null;
            //if it's child site from xml, let's do somethign else
            if (!isChildSite)
            {
                // Load host web and add new web to it.
                Microsoft.SharePoint.Client.Web web = cc.Web;
                cc.Load(web);
                cc.ExecuteQuery();
                newWeb = web.Webs.Add(information);
            }
            else
            {
                newWeb = subWeb.Webs.Add(information);
            }
            cc.ExecuteQuery();
            cc.Load(newWeb);
            cc.ExecuteQuery();

            DeployFiles(cc, newWeb, templateConfig);
            DeployCustomActions(cc, newWeb, templateConfig);
            DeployLists(cc, newWeb, templateConfig);
            DeployNavigation(cc, newWeb, templateConfig);
            DeployTheme(cc, newWeb, templateConfig, baseConfiguration);
            SetSiteLogo(cc, newWeb, templateConfig);

            if (!isChildSite)
            {
                DeploySubSites(cc, newWeb, templateConfig, page, baseConfiguration);
            }

            // All done, let's return the newly created site
            return newWeb;
        }

        private void SetSiteLogo(ClientContext cc, Web web, XElement templateConfig)
        {
            Web rootWeb = null;
            if (EnsureWeb(cc, web, "ServerRelativeUrl").ServerRelativeUrl.ToLowerInvariant() !=
                    EnsureSite(cc, cc.Site, "ServerRelativeUrl").ServerRelativeUrl.ToLowerInvariant())
            {
                // get instances to root web, since we are processign currently sub site 
                rootWeb = cc.Site.RootWeb;
                cc.Load(rootWeb);
                cc.ExecuteQuery();
            }
            else
            {
                // Let's double check that the web is available
                rootWeb = EnsureWeb(cc, web, "Title");
            }

            string fullPathToLogo = GetWebRelativeFolderPath(templateConfig.Attribute("SiteLogoUrl").Value);
            // Not natively supported, but we can update the themed site icon. If initial theme was just applied, image is at
            // _themes/themed/themeID/siteIcon-2129F729.themedpng

            // New model for themes
            //Folder rootFolder = rootWeb.RootFolder;
            //Folder themeFolder = ResolveSubFolder(cc, rootFolder, "_themes");
            //Folder themeAssetsFolder = ResolveSubFolder(cc, themeFolder, "Themed");
            //Folder themedFolder = ResolveFirstSubFolder(cc, themeAssetsFolder);

            // Old tenant model
            Folder rootFolder = web.RootFolder;
            Folder themeFolder = ResolveSubFolder(cc, rootFolder, "_themes");
            Folder themeAssetsFolder = ResolveSubFolder(cc, themeFolder, "0");

            // Use CSOM to uplaod the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(fullPathToLogo);
            newFile.Url = themeAssetsFolder.ServerRelativeUrl + "/siteIcon-2129F729.themedpng";
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = themeAssetsFolder.Files.Add(newFile);
            cc.Load(uploadFile);
            cc.ExecuteQuery();

        }

        private Folder ResolveFirstSubFolder(ClientContext clientContext, Folder folder)
        {
            clientContext.Load(folder);
            clientContext.Load(folder.Folders);
            clientContext.ExecuteQuery();
            foreach (Folder subFolder in folder.Folders)
            {
                //just return the first sub folder
                return subFolder;
            }
            return folder;
        }

        private Web EnsureWeb(ClientContext cc, Web web, string propertyToCheck)
        {
            if (!web.IsObjectPropertyInstantiated(propertyToCheck))
            {
                // get instances to root web, since we are processign currently sub site 
                cc.Load(web);
                cc.ExecuteQuery();
            }
            return web;
        }


        private Site EnsureSite(ClientContext cc, Site site, string propertyToCheck)
        {
            if (!site.IsObjectPropertyInstantiated(propertyToCheck))
            {
                // get instances to root web, since we are processign currently sub site 
                cc.Load(site);
                cc.ExecuteQuery();
            }
            return site;
        }


        private void DeployTheme(ClientContext cc, Web newWeb, XElement templateConfig, XDocument baseConfiguration)
        {
            Web rootWeb = null;

            string theme = templateConfig.Attribute("Theme").Value;
            // Solve theme URLs from config
            XElement themeStructure = SolveUsedThemeConfigElementFromXML(theme, baseConfiguration);
            string colorFile = GetWebRelativeFolderPath(themeStructure.Attribute("ColorFile").Value);
            string fontFile = GetWebRelativeFolderPath(themeStructure.Attribute("FontFile").Value);
            string backgroundImage = GetWebRelativeFolderPath(themeStructure.Attribute("BackgroundFile").Value);
            // Master page is given as the name of the master, has to be uplaoded seperately if custom one is needed
            string masterPage = themeStructure.Attribute("MasterPage").Value;

            if (EnsureWeb(cc, newWeb, "ServerRelativeUrl").ServerRelativeUrl.ToLowerInvariant() !=
                    EnsureSite(cc, cc.Site, "ServerRelativeUrl").ServerRelativeUrl.ToLowerInvariant())
            {
                // get instances to root web, since we are processign currently sub site 
                rootWeb = cc.Site.RootWeb;
                cc.Load(rootWeb);
                cc.ExecuteQuery();
            }
            else
            {
                // Let's double check that the web is available
                rootWeb = EnsureWeb(cc, newWeb, "Title");
            }

            // Deploy theme files to root web, if they are not there and set it as active theme for the site
            newWeb.DeployThemeToSubWeb(rootWeb, theme,
                                       colorFile, fontFile, backgroundImage, masterPage);

            // Setting the theme to new web
            newWeb.SetThemeToSubWeb(rootWeb, theme);
        }

        private XElement SolveUsedThemeConfigElementFromXML(string theme, XDocument configuration)
        {
            XElement templates = configuration.Root.Element("Themes");
            IEnumerable<XElement> template =
            from el in templates.Elements()
            where (string)el.Attribute("Name") == theme
            select el;

            return template.ElementAt(0);
        }

        private string GetWebRelativeFolderPath(string filePath)
        {
            string relativePath = string.Empty;
            if (!string.IsNullOrEmpty(filePath))
            {
                relativePath = HostingEnvironment.MapPath(string.Format("~/{0}", filePath));
            }
            return relativePath;
        }

        private Folder ResolveSubFolder(ClientContext clientContext, Folder folder, string folderName)
        {
            clientContext.Load(folder);
            clientContext.Load(folder.Folders);
            clientContext.ExecuteQuery();
            foreach (Folder subFolder in folder.Folders)
            {
                if (subFolder.Name.ToLowerInvariant() == folderName.ToLowerInvariant())
                {
                    return subFolder;
                }
            }
            return folder;
        }

        /// <summary>
        /// Sub site provisioning handler
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="templateConfig"></param>
        /// <param name="page"></param>
        /// <param name="baseConfiguration"></param>
        private void DeploySubSites(ClientContext clientContext, Web web, XElement templateConfig, Page page, XDocument baseConfiguration)
        {
            XElement sitesToCreate = templateConfig.Element("Sites");
            if (sitesToCreate != null)
            {
                // If we do have sub sites defined in the config, let's provision those as well
                foreach (XElement siteToCreate in sitesToCreate.Elements())
                {
                    CreateSubSite(siteToCreate.Attribute("Url").Value, siteToCreate.Attribute("Template").Value, siteToCreate.Attribute("Title").Value, siteToCreate.Attribute("Description").Value, clientContext, page, baseConfiguration, true, web);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web">Web which is already initiated</param>
        /// <param name="siteTemplate"></param>
        private void DeployNavigation(ClientContext clientContext, Web web, XElement siteTemplate)
        {
            XElement navigationNodesToCreate = siteTemplate.Element("NavigationNodes");
            if (navigationNodesToCreate != null)
            {
                foreach (XElement navigationNodeToCreate in navigationNodesToCreate.Elements())
                {
                    // Let's create the nodes based on configuration to quick launch
                    NavigationNodeCreationInformation nodeInformation = new NavigationNodeCreationInformation();
                    nodeInformation.Title = navigationNodeToCreate.Attribute("Title").Value;
                    nodeInformation.Url = navigationNodeToCreate.Attribute("Url").Value;

                    clientContext.Load(web.Navigation.QuickLaunch);
                    clientContext.ExecuteQuery();
                    web.Navigation.QuickLaunch.Add(nodeInformation);

                    clientContext.ExecuteQuery();
                }
            }
        }

        /// <summary>
        ///  Generic handler for list instances
        /// </summary>
        /// <param name="clientContext">Context to apply the changes in</param>
        /// <param name="siteTemplate">XML configuration for the template</param>
        private void DeployLists(ClientContext clientContext, Web web, XElement siteTemplate)
        {
            XElement liststoCreate = siteTemplate.Element("Lists");
            if (liststoCreate != null)
            {
                foreach (XElement listToCreate in liststoCreate.Elements())
                {
                    ListCreationInformation listInformation = new ListCreationInformation();
                    listInformation.Description = listToCreate.Attribute("Description").Value;
                    if (!string.IsNullOrEmpty(listToCreate.Attribute("DocumentTemplate").Value))
                        listInformation.DocumentTemplateType = Convert.ToInt32(listToCreate.Attribute("DocumentTemplate").Value);
                    if (!string.IsNullOrEmpty(listToCreate.Attribute("OnQuickLaunch").Value))
                        if (Convert.ToBoolean(listToCreate.Attribute("OnQuickLaunch").Value))
                            listInformation.QuickLaunchOption = QuickLaunchOptions.On;
                        else
                            listInformation.QuickLaunchOption = QuickLaunchOptions.Off;
                    if (!string.IsNullOrEmpty(listToCreate.Attribute("TemplateType").Value))
                        listInformation.TemplateType = Convert.ToInt32(listToCreate.Attribute("TemplateType").Value);
                    listInformation.Title = listToCreate.Attribute("Title").Value;
                    listInformation.Url = listToCreate.Attribute("Url").Value;

                    web.Lists.Add(listInformation);
                    clientContext.ExecuteQuery();
                }
            }
        }

        /// <summary>
        /// Generic handler for custom action entries
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="siteTemplate">XML structure for the template</param>
        private void DeployCustomActions(ClientContext clientContext, Web web, XElement siteTemplate)
        {
            XElement customActionsToDeploy = siteTemplate.Element("CustomActions");
            foreach (XElement customAction in customActionsToDeploy.Elements())
            {
                string scriptSource = customAction.Attribute("ScriptSrc").Value.Replace("~site", web.Url);
                AddJsLink(customAction.Attribute("Name").Value, scriptSource, clientContext, web);
            }
        }

        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">semi colon delimited list of links to javascript files</param>
        /// <returns>True if action was ok</returns>
        public bool AddJsLink(string key, string scriptLinks, ClientContext clientContext, Web web)
        {
            if (String.IsNullOrWhiteSpace(scriptLinks))
            {
                throw new ArgumentException("scriptLinks");
            }

            StringBuilder scripts = new StringBuilder(@"
var headID = document.getElementsByTagName('head')[0]; 
var");
            var files = scriptLinks.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var f in files)
            {
                scripts.AppendFormat(@"
newScript = document.createElement('script');
newScript.type = 'text/javascript';
newScript.src = '{0}';
headID.appendChild(newScript);", f);
            }
            bool ret = AddJsBlock(key, scripts.ToString(), clientContext, web);
            return ret;
        }

        /// <summary>
        /// Injects javascript via a adding a custom action to the site
        /// </summary>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptBlock">Javascript to be injected</param>
        /// <returns>True if action was ok</returns>
        public bool AddJsBlock(string key, string scriptBlock, ClientContext clientContext, Web web)
        {
            var jsAction = new CustomActionEntity()
            {
                Description = key,
                Location = ScriptLocation,
                ScriptBlock = scriptBlock,
            };
            bool ret = AddCustomAction(jsAction, clientContext, web);
            return ret;
        }

        /// <summary>
        /// Adds or removes a custom action from a site
        /// </summary>
        /// <param name="customAction">Information about the custom action be added or deleted</param>
        /// <example>
        /// var editAction = new CustomActionEntity()
        /// {
        ///     Title = "Edit Site Classification",
        ///     Description = "Manage business impact information for site collection or sub sites.",
        ///     Sequence = 1000,
        ///     Group = "SiteActions",
        ///     Location = "Microsoft.SharePoint.StandardMenu",
        ///     Url = EditFormUrl,
        ///     ImageUrl = EditFormImageUrl,
        ///     Rights = new BasePermissions(),
        /// };
        /// editAction.Rights.Set(PermissionKind.ManageWeb);
        /// AddCustomAction(editAction, new Uri(site.Properties.Url));
        /// </example>
        /// <returns>True if action was ok</returns>
        public bool AddCustomAction(CustomActionEntity customAction, ClientContext clientContext, Web web)
        {
            bool exist = false;
            var existingActions = web.UserCustomActions;
            clientContext.Load(existingActions);
            clientContext.ExecuteQuery();
            var actions = existingActions.ToArray();
            foreach (var action in actions)
            {
                if (action.Description == customAction.Description &&
                    action.Location == customAction.Location)
                {
                    exist = true;
                    action.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }

            if (customAction.Remove)
                return false;

            var newAction = existingActions.Add();
            newAction.Description = customAction.Description;
            newAction.Location = customAction.Location;
            if (customAction.Location == ScriptLocation)
            {
                newAction.ScriptBlock = customAction.ScriptBlock;
            }
            else
            {
                newAction.Sequence = customAction.Sequence;
                newAction.Url = customAction.Url;
                newAction.Group = customAction.Group;
                newAction.Title = customAction.Title;
                newAction.ImageUrl = customAction.ImageUrl;
                newAction.Rights = customAction.Rights;
            }
            newAction.Update();
            clientContext.Load(web, s => s.UserCustomActions);
            clientContext.ExecuteQuery();
            exist = true;
            return exist;
        }


        /// <summary>
        /// Utility method to check particular custom action already exists on the web
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="name">Name of the custom action</param>
        /// <returns></returns>
        private bool CustomActionAlreadyExists(ClientContext clientContext, Web web, string name)
        {
            clientContext.Load(web.UserCustomActions);
            clientContext.ExecuteQuery();
            for (int i = 0; i < web.UserCustomActions.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(web.UserCustomActions[i].Name) &&
                        web.UserCustomActions[i].Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Generic handler for the file deployments
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="siteTemplate">XML definition for the template</param>
        private void DeployFiles(ClientContext clientContext, Web web, XElement siteTemplate)
        {
            XElement filesToLoad = siteTemplate.Element("Files");
            foreach (XElement file in filesToLoad.Elements())
            {
                if (file.Attribute("UploadToDocumentLibray").Value == "false")
                {
                    DeployFileToWebFolder(clientContext, web, file.Attribute("Src").Value, file.Attribute("TargetFolder").Value);
                }
                // TODO - handler for the document library is missing
            }
        }

        /// <summary>
        /// Generic upload file to context
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="file"></param>
        /// <param name="folder"></param>
        public void DeployFileToWebFolder(ClientContext clientContext, Web web, string file, string folder)
        {
            Folder jsFolder;

            if (!DoesFolderExists(clientContext, web, folder))
            {
                jsFolder = web.Folders.Add(folder);

            }
            else
            {
                jsFolder = web.Folders.GetByUrl(folder);
            }
            // Load Folder instance
            clientContext.Load(jsFolder);
            clientContext.ExecuteQuery();

            UploadFileToWeb(clientContext, web, HostingEnvironment.MapPath(file), jsFolder);
        }

        /// <summary>
        /// Get configuration for specific template based on name of the template
        /// </summary>
        /// <param name="chosenTemplate"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private XElement GetTemplateConfig(string chosenTemplate, XDocument configuration)
        {
            XElement templates = configuration.Root.Element("Templates");
            IEnumerable<XElement> template =
            from el in templates.Elements()
            where (string)el.Attribute("Name") == chosenTemplate
            select el;

            return template.ElementAt(0);
        }

        /// <summary>
        /// Return the root template value from the config
        /// </summary>
        /// <param name="template"></param>
        /// <param name="templateConfig"></param>
        /// <returns></returns>
        private string SolveUsedTemplate(string template, XElement templateConfig)
        {
            // Root template is stored in RootTemplate attribute in this level
            return templateConfig.Attribute("RootTemplate").Value;
        }


        /// <summary>
        /// Utility function to check if the folder name exists already in the context web
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="targetFolderUrl"></param>
        /// <returns></returns>
        private bool DoesFolderExists(ClientContext clientContext, Web web, string targetFolderUrl)
        {
            Folder folder = web.GetFolderByServerRelativeUrl(targetFolderUrl);
            clientContext.Load(folder);
            bool exists = false;

            try
            {
                clientContext.ExecuteQuery();
                exists = true;
            }
            catch (Exception ex)
            { }
            return exists;
        }

        /// <summary>
        /// Generic uploader for the file to context web
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fullFilePath"></param>
        /// <param name="folder"></param>
        public void UploadFileToWeb(ClientContext context, Web web, string fullFilePath, Folder folder)
        {
            try
            {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = System.IO.File.ReadAllBytes(fullFilePath);
                newFile.Url = folder.ServerRelativeUrl + "/" + Path.GetFileName(fullFilePath);
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File uploadFile = folder.Files.Add(newFile);
                context.Load(uploadFile);
                context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                // TODO - Proper logging on exceptions... this is not really acceptable
                string fuu = ex.ToString();
            }

        }

        /// <summary>
        /// Get current templates from xml configuration
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        /// <remarks>Could be extended to support filtering based on current web template</remarks>
        internal IEnumerable<string> GetAvailableSubSiteTemplates(System.Xml.Linq.XDocument doc)
        {
            List<string> sites = new List<string>();
            XElement templates = doc.Root.Element("Templates");

            foreach (XElement element in templates.Elements())
            {
                sites.Add(element.Attribute("Name").Value);
            }

            return sites;
        }

        public void EnsureConfigurationListInTenant(string hostWebUrl)
        {
            //get the base tenant admin urls
            var tenantStr = hostWebUrl.ToLower().Replace("-my", "").Substring(8); //remove my if it exists...
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));

            //create site collection using the Tenant object
            var tenantRootUri = new Uri(String.Format("https://{0}.sharepoint.com", tenantStr));
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantRootUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantRootUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantRootUri.ToString(), token))
            {
                Web rootWeb = adminContext.Web;
                adminContext.Load(rootWeb);
                ListCollection listCollection = rootWeb.Lists;
                adminContext.Load(listCollection, lists => lists.Include(list => list.Title).Where(list => list.Title == "OfficeAMSConfig"));
                adminContext.ExecuteQuery();

                if (listCollection.Count == 0)
                {
                    ListCreationInformation listCreationInfo = new ListCreationInformation();
                    listCreationInfo.Title = "OfficeAMSConfig";
                    listCreationInfo.TemplateType = (int)ListTemplateType.GenericList;
                    List oList = rootWeb.Lists.Add(listCreationInfo);
                    Field oField = oList.Fields.AddFieldAsXml("<Field DisplayName='Value' Type='Text' />", true, AddFieldOptions.DefaultValue);
                    adminContext.ExecuteQuery();
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem item = oList.AddItem(itemCreateInfo);
                    item["Title"] = "SubSiteAppUrl";
                    item["Value"] = "https://localhost:44323";
                    item.Update();
                    adminContext.ExecuteQuery();
                    
                }
            }
        }
    }
}