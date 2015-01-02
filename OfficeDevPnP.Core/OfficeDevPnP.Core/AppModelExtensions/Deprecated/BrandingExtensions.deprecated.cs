using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LanguageTemplateHash = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>;

namespace Microsoft.SharePoint.Client
{

    /// <summary>
    /// Class that holds the deprecated branding methods
    /// </summary>
    public static partial class BrandingExtensions
    {
        [Obsolete("Use web.UploadThemeFile and web.CreateComposedLook separately")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void DeployThemeToWeb(this Web web, string themeName, string colorFilePath, string fontFilePath, string backgroundImagePath, string masterPageName)
        {
            DeployThemeToWebImplementation(web, web, themeName, colorFilePath, fontFilePath, backgroundImagePath, masterPageName);
        }

        [Obsolete("Use web.UploadThemeFile and web.CreateComposedLook separately")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void DeployThemeToSubWeb(this Web web, Web rootWeb, string themeName, string colorFilePath, string fontFilePath, string backgroundImagePath, string masterPageName)
        {
            DeployThemeToWebImplementation(web, rootWeb, themeName, colorFilePath, fontFilePath, backgroundImagePath, masterPageName);
        }

        private static void DeployThemeToWebImplementation(Web web, Web rootWeb, string themeName, string colorFilePath, string fontFilePath, string backgroundImagePath, string masterPageName)
        {
            LoggingUtility.Internal.TraceInformation((int)EventId.DeployTheme, CoreResources.BrandingExtension_DeployTheme, themeName, web.Context.Url);

            // Deploy files one by one to proper location
            if (!string.IsNullOrEmpty(colorFilePath) && System.IO.File.Exists(colorFilePath))
            {
                rootWeb.UploadThemeFile(colorFilePath);
            }

            if (!string.IsNullOrEmpty(fontFilePath) && System.IO.File.Exists(fontFilePath))
            {
                rootWeb.UploadThemeFile(fontFilePath);
            }

            if (!string.IsNullOrEmpty(backgroundImagePath) && System.IO.File.Exists(backgroundImagePath))
            {
                rootWeb.UploadThemeFile(backgroundImagePath);
            }

            // Let's also add entry to the Theme catalog. This is not actually required, but provides visibility for the theme option, if manually changed
            web.CreateComposedLookByName(themeName, colorFilePath, fontFilePath, backgroundImagePath, masterPageName);
        }

        [Obsolete("Use web.ComposedLookExists")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        /// Note: this method will not work to check for the OOB themes, only custom teams are retrievable
        public static bool ThemeEntryExists(this Web web, string themeName)
        {
            return ComposedLookExists(web, themeName);
        }

        [Obsolete("Use web.ComposedLookExists")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool ThemeEntryExists(this Web web, string themeName, List themeGalleryList)
        {
            return ComposedLookExists(web, themeName);
        }

        [Obsolete("Use web.CreateComposedLook")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddNewThemeOptionToSubWeb(this Web web, Web rootWeb, string themeName, string colorFileName, string fontFileName, string backgroundName, string masterPageName)
        {
            CreateComposedLookByName(web, themeName, colorFileName, fontFileName, backgroundName, masterPageName, displayOrder: 11);
        }

        [Obsolete("Use web.CreateComposedLook")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddNewThemeOptionToWeb(this Web web, string themeName, string colorFileName, string fontFileName, string backgroundName, string masterPageName)
        {
            CreateComposedLookByName(web, themeName, colorFileName, fontFileName, backgroundName, masterPageName, displayOrder: 11);
        }

        [Obsolete("Use web.CreateComposedLook")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddNewThemeOptionToSite(this Web web, string themeName, string colorFileName, string fontFileName, string backgroundName, string masterPageName)
        {
            CreateComposedLookByName(web, themeName, colorFileName, fontFileName, backgroundName, masterPageName, displayOrder: 11);
        }

        [Obsolete("Use web.SetComposedLook")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetThemeToWeb(this Web web, string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                throw (themeName == null)
                  ? new ArgumentNullException("themeName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "themeName");
            }
            SetComposedLookByUrl(web, themeName);
        }

        [Obsolete("Use web.SetComposedLook")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetThemeToSubWeb(this Web web, Web rootWeb, string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                throw (themeName == null)
                  ? new ArgumentNullException("themeName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "themeName");
            }
            SetComposedLookByUrl(web, themeName);
        }

        //TODO: to be replaced by new site logo CSOM once we've the April 2014 CU
        //Note: does seem to broken on the current SPO implementation (20/03/2014) as there's no _themes folder anymore in the root web
        [Obsolete("Use Web.SiteLogoUrl property")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetSiteLogo(this Web web, string fullPathToLogo)
        {
            if (string.IsNullOrEmpty(fullPathToLogo) || !System.IO.File.Exists(fullPathToLogo))
            {
                return;
            }
            // Not natively supported, but we can update the themed site icon. If initial theme was just applied, image is at
            // _themes/0/siteIcon-2129F729.themedpng
            Folder rootFolder = web.RootFolder;
            Folder themeFolder = rootFolder.ResolveSubFolder("_themes");
            Folder themeAssetsFolder = themeFolder.ResolveSubFolder("0");

            // Use CSOM to upload the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(fullPathToLogo);
            newFile.Url = themeAssetsFolder.ServerRelativeUrl + "/siteIcon-2129F729.themedpng";
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = themeAssetsFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();
        }

        [Obsolete("Use web.UploadThemeFile")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void DeployFileToThemeFolderSite(this Web web, string sourceFileAddress, string themeFolderVersion = "15")
        {
            var themesList = web.GetCatalog((int)ListTemplateType.ThemeCatalog);
            var themesFolder = themesList.RootFolder.EnsureFolder(themeFolderVersion);
            themesFolder.UploadFile(sourceFileAddress);
        }

        [Obsolete("Use web.UploadThemeFile")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void DeployFileToThemeFolderSite(this Web web, byte[] fileBytes, string fileName, string themeFolderVersion = "15")
        {
            if (fileBytes == null || fileBytes.Length == 0) { throw new ArgumentNullException("fileBytes"); }

            var themesList = web.GetCatalog((int)ListTemplateType.ThemeCatalog);
            var themesFolder = themesList.RootFolder.EnsureFolder(themeFolderVersion);
            using (var ms = new MemoryStream(fileBytes))
            {
                themesFolder.UploadFile(fileName, ms);
            }
        }

        /// <summary>
        /// Can be used to set master page and custom master page in single command
        /// </summary>
        /// <param name="web"></param>
        /// <param name="masterPageName"></param>
        /// <param name="customMasterPageName"></param>
        /// <exception cref="System.ArgumentException">Thrown when masterPageName or customMasterPageName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when masterPageName or customMasterPageName is null</exception>
        [Obsolete("Use SetMasterPagesByName")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetMasterPagesForSiteByName(this Web web, string masterPageName, string customMasterPageName)
        {
            web.SetMasterPagesByName(masterPageName, customMasterPageName);
        }

        /// <summary>
        /// Can be used to set master page and custom master page in single command
        /// </summary>
        /// <param name="web"></param>
        /// <param name="masterPageName"></param>
        /// <param name="customMasterPageName"></param>
        /// <exception cref="System.ArgumentException">Thrown when masterPageName or customMasterPageName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when masterPageName or customMasterPageName is null</exception>
        [Obsolete("Use SetMasterPagesByUrl")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetMasterPagesForSiteByUrl(this Web web, string masterPageUrl, string customMasterPageUrl)
        {
            web.SetMasterPagesByUrl(masterPageUrl, customMasterPageUrl);
        }

        /// <summary>
        /// Master page is set by using master page name. Master page is set from the current web.
        /// </summary>
        /// <param name="web">Current web</param>
        /// <param name="masterPageName">Name of the master page. Path is resolved from this.</param>
        /// <exception cref="System.ArgumentException">Thrown when masterPageName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when masterPageName is null</exception>  
        [Obsolete("Use SetMasterPageByName")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetMasterPageForSiteByName(this Web web, string masterPageName)
        {
            web.SetMasterPageByName(masterPageName);
        }

        /// <summary>
        /// Master page is set by using master page name. Master page is set from the current web.
        /// </summary>
        /// <param name="web">Current web</param>
        /// <param name="masterPageName">Name of the master page. Path is resolved from this.</param>
        /// <exception cref="System.ArgumentException">Thrown when masterPageName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when masterPageName is null</exception>  
        [Obsolete("Use SetCustomMasterPageByName")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetCustomMasterPageForSiteByName(this Web web, string masterPageName)
        {
            web.SetCustomMasterPageByName(masterPageName);
        }

        [Obsolete("Use web.GetCurrentLook")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static ThemeEntity GetCurrentTheme(this Web web)
        {
            var theme = GetCurrentComposedLook(web);
            web.Context.Load(web, w => w.MasterUrl, w => w.CustomMasterUrl);
            web.Context.ExecuteQuery();
            if (string.IsNullOrEmpty(theme.MasterPage))
            {
                theme.MasterPage = web.MasterUrl;
            }
            theme.CustomMasterPage = web.CustomMasterUrl;
            return theme;
        }

        /// <summary>
        /// Returns the current theme of a web
        /// </summary>
        /// <param name="web">Web to check</param>
        /// <returns>Entity with attributes of current composed look, or null if none</returns>
        [Obsolete("Use GetCurrentComposedLook")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static ThemeEntity GetCurrentLook(this Web web)
        {
            return GetComposedLook(web, CurrentLookName);
        }

        [Obsolete("Use SetMasterPageByUrl")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetMasterPageForSiteByUrl(this Web web, string masterPageUrl)
        {
            web.SetMasterPageByUrl(masterPageUrl, updateRootOnly: true);
        }

        [Obsolete("Use Web.SetCustomMasterPageByUrl()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetCustomMasterPageForSiteByUrl(this Web web, string masterPageUrl)
        {
            web.SetCustomMasterPageByUrl(masterPageUrl, updateRootOnly: true);
        }

        [Obsolete("Use SolveSiteRelativeUrl")]
        private static string SolveSiteRelateveUrl(Web web, string url)
        {
            return SolveSiteRelativeUrl(web, url);
        }




    }
}