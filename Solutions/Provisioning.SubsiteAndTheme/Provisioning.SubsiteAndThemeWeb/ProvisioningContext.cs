using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace Provisioning.SubsiteAndThemeWeb {
    public class ProvisioningContext {
        const string CONTEXT_NAME = "ProvisioningContext";
        const string FILE_PATH = "/SiteConfiguration.xml";
        static object _lockObject = new object();
        ProvisioningConfiguration _configuration;

        ProvisioningContext() { }

        public static ProvisioningContext Current {
            get {
                if (HttpContext.Current.Items[CONTEXT_NAME] == null) {
                    lock (_lockObject) {
                        HttpContext.Current.Items[CONTEXT_NAME] = new ProvisioningContext();
                    }
                }
                return (ProvisioningContext)HttpContext.Current.Items[CONTEXT_NAME];
            }
        }

        public ProvisioningConfiguration Configuration {
            get {
                if (_configuration == null)
                    _configuration = ReadConfiguration();
                return _configuration;
            }
        }

        internal static void WriteConfiguration(ProvisioningConfiguration configuration) {
            var serializer = new XmlSerializer(typeof(ProvisioningConfiguration));
            var filePath = HttpContext.Current.Server.MapPath(FILE_PATH);
            using (var file = new FileStream(filePath, FileMode.OpenOrCreate)) {
                serializer.Serialize(file, configuration);
            }
        }
        static ProvisioningConfiguration ReadConfiguration() {
            var serializer = new XmlSerializer(typeof(ProvisioningConfiguration));
            var filePath = HttpContext.Current.Server.MapPath(FILE_PATH);
            using (var file = new FileStream(filePath, FileMode.OpenOrCreate)) {
                var config = (ProvisioningConfiguration)serializer.Deserialize(file);
                return config;
            }
        }
        internal static void RenderChromeScript(System.Web.UI.Page page) {

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
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "BasePageScript", script, true);
        }

        public void CreateSite(
            string siteUrl,
            string title,
            string path,
            string description,
            string themeName,
            string templateId) {
            
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
            using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                #region create new web
                var newWebInfo = new WebCreationInformation(){
                    Title = title,
                    Description = description,
                    //Language = 1033,
                    Url = path,
                    UseSamePermissionsAsParentSite = true,
                    WebTemplate = templateId
                };
                
                var newWeb = clientContext.Web.Webs.Add(newWebInfo);
                clientContext.Load(clientContext.Web);
                clientContext.Load(clientContext.Web.Webs);
                clientContext.Load(clientContext.Site, s => s.RootWeb, s => s.Url);
                clientContext.Load(newWeb);
                clientContext.ExecuteQuery();
                #endregion

                #region add and apply theme
                ApplyTheme(newWeb, clientContext.Site.RootWeb, themeName);
                #endregion
                
                #region set site logo
                SetSiteLogo(newWeb, clientContext.Site.RootWeb);
                #endregion
                HttpContext.Current.Response.Redirect(newWeb.Url, false);
            }
        }

        public void ApplyTheme(Web targetWeb, Web rootWeb, string themeName, bool alreadyUploaded = false) {
            var theme = ProvisioningContext.Current.Configuration.Branding.Themes.First(t => t.Name == themeName);
            var server = HttpContext.Current.Server;

            if (!targetWeb.ComposedLookExists(themeName)) {
                var hasColorFile = System.IO.File.Exists(server.MapPath(theme.ColorFile));
                var hasFontFile = System.IO.File.Exists(server.MapPath(theme.FontFile));
                var hasBackgroundFile = System.IO.File.Exists(server.MapPath(theme.BackgroundFile));

                // upload the color file
                if (hasColorFile)
                    UploadThemeFile(rootWeb, server.MapPath(theme.ColorFile));

                // upload the font file
                if (hasFontFile)
                    UploadThemeFile(rootWeb, server.MapPath(theme.FontFile));

                // upload the background file
                if (hasBackgroundFile)
                    UploadThemeFile(rootWeb, server.MapPath(theme.BackgroundFile));

                var colorFileName = hasColorFile ? Path.GetFileName(theme.ColorFile) : null;
                var fontFileName = hasFontFile ? Path.GetFileName(theme.FontFile) : null;
                var backgroundFileName = hasBackgroundFile ? Path.GetFileName(theme.BackgroundFile) : null;

                CreateComposedLook(
                        targetWeb,
                        rootWeb.ServerRelativeUrl,
                        themeName,
                        colorFileName,
                        fontFileName,
                        "seattle.master",
                        backgroundImagePath: null,
                        displayOrder: 1
                    );
            }

            targetWeb.SetComposedLookByUrl(themeName);
        }



        public string SetSiteLogo(Web newWeb, Web rootWeb, string alreadyUploadedLogoUrl = null) {
            var context = newWeb.Context;
            string logoUrl;

            if (!string.IsNullOrEmpty(Configuration.Branding.LogoUrl)) 
                logoUrl = Configuration.Branding.LogoUrl;

            else if (!string.IsNullOrEmpty(alreadyUploadedLogoUrl))
                logoUrl = alreadyUploadedLogoUrl;

            else {
                // upload logo file
                var serverFilePath = HttpContext.Current.Server.MapPath(Configuration.Branding.LogoFilePath);
                var siteAssetsList = rootWeb.Lists.GetByTitle("Site Assets");

                using (var file = new FileStream(serverFilePath, FileMode.Open)) {
                    var outFile = siteAssetsList.RootFolder.Files.Add(new FileCreationInformation() {
                        ContentStream = file,
                        Overwrite = true,
                        Url = Path.GetFileName(serverFilePath)
                    });
                    context.Load(outFile);
                    context.ExecuteQuery();
                    logoUrl = outFile.ServerRelativeUrl;
                }
            }

            newWeb.SiteLogoUrl = logoUrl;
            newWeb.Update();
            context.Load(newWeb);
            context.ExecuteQuery();
            return logoUrl;
        }

        void UploadThemeFile(Web rootWeb, string filePath) {
            var themeCatalog = rootWeb.GetCatalog((int)ListTemplateType.ThemeCatalog);
            var v15Folder = themeCatalog.RootFolder.Folders.GetByUrl("15");
            rootWeb.Context.Load(themeCatalog);
            rootWeb.Context.Load(v15Folder);
            rootWeb.Context.ExecuteQuery();

            using (var file = new FileStream(filePath, FileMode.Open)){
                v15Folder.Files.Add(new FileCreationInformation() {
                    ContentStream = file,
                    Url = Path.GetFileName(filePath),
                    Overwrite = true
                });
                rootWeb.Context.ExecuteQuery();
            }
        }

        void CreateComposedLook(Web targetWeb, string rootWebServerRelativeUrl, string name, string paletteFileName, string fontFileName, string masterPageName, string backgroundImagePath, uint displayOrder) {
            if (string.IsNullOrEmpty(rootWebServerRelativeUrl))
                throw new ArgumentNullException("rootWebServerRelativeUrl");
            if (string.IsNullOrEmpty(paletteFileName))
                throw new ArgumentNullException("paletteFileName");
            if (string.IsNullOrEmpty(masterPageName))
                throw new ArgumentNullException("masterPageName");

            var composedLooks = targetWeb.Lists.GetByTitle("Composed Looks");
            var context = targetWeb.Context;

            context.Load(composedLooks);
            context.ExecuteQuery();

            var item = composedLooks.AddItem(new ListItemCreationInformation());
            item["Title"] = name;
            item["Name"] = name;
            item["ImageUrl"] = backgroundImagePath;
            item["MasterPageUrl"] = UrlUtility.Combine(targetWeb.ServerRelativeUrl, "_catalogs/masterpage", masterPageName);
            item["ThemeUrl"] = UrlUtility.Combine(rootWebServerRelativeUrl, "_catalogs/theme/15", paletteFileName);
            item["FontSchemeUrl"] = fontFileName != null ? UrlUtility.Combine(rootWebServerRelativeUrl, "_catalogs/theme/15", fontFileName) : null;
            item.Update();

            context.Load(item);
            context.ExecuteQuery();
        }
    }
}
