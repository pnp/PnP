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
    /// Class that deals with branding features
    /// </summary>
    public static class BrandingExtensions
    {
        const string AvailablePageLayouts = "__PageLayouts";
        const string DefaultPageLayout = "__DefaultPageLayout";
        const string AvailableWebTemplates = "__WebTemplates";
        const string InheritWebTemplates = "__InheritWebTemplates";
        const string InheritMaster = "__InheritMasterUrl";
        const string InheritCustomMaster = "__InheritCustomMasterUrl";
        const string InheritTheme = "__InheritsThemedCssFolderUrl";
        const string Inherit = "__Inherit";
        const string CurrentLookName = "Current";
        const string CAML_QUERY_FIND_BY_FILENAME = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Name' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";

        [Obsolete("Use site.UploadThemeFile and web.CreateComposedLook separately")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void DeployThemeToWeb(this Web web, string themeName, string colorFilePath, string fontFilePath, string backgroundImagePath, string masterPageName)
        {
            DeployThemeToWebImplementation(web, web, themeName, colorFilePath, fontFilePath, backgroundImagePath, masterPageName);
        }

        [Obsolete("Use site.UploadThemeFile and web.CreateComposedLook separately")]
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
                rootWeb.DeployFileToThemeFolderSite(colorFilePath);
            }
            if (!string.IsNullOrEmpty(fontFilePath) && System.IO.File.Exists(fontFilePath))
            {
                rootWeb.DeployFileToThemeFolderSite(fontFilePath);
            }
            if (!string.IsNullOrEmpty(backgroundImagePath) && System.IO.File.Exists(backgroundImagePath))
            {
                rootWeb.DeployFileToThemeFolderSite(backgroundImagePath);
            }

            // Let's also add entry to the Theme catalog. This is not actually required, but provides visibility for the theme option, if manually changed
            web.AddNewThemeOptionToSubWeb(rootWeb, themeName, colorFilePath, fontFilePath, backgroundImagePath, masterPageName);
        }

        /// <summary>
        /// Checks to see if the theme already exists.
        /// </summary>
        /// <param name="web">Site to be processed</param>
        /// <param name="themeName">Name for the new theme</param>
        /// <returns>True if theme exists, false otherwise</returns>
        public static bool ThemeEntryExists(this Web web, string themeName)
        {
            // Let's get instance to the composite look gallery
            List themesList = web.GetCatalog((int)ListTemplateType.DesignCatalog);
            web.Context.Load(themesList);
            web.Context.ExecuteQuery();

            return web.ThemeEntryExists(themeName, themesList);
        }

        /// <summary>
        /// Checks to see if the theme already exists
        /// </summary>
        /// <param name="web">Site to be processed</param>
        /// <param name="themeName">Name for the new theme</param>
        /// <param name="themeGalleryList">SharePoint theme gallery list</param>
        /// <returns>True if theme exists, false otherwise</returns>
        public static bool ThemeEntryExists(this Web web, string themeName, List themeGalleryList)
        {
            CamlQuery query = new CamlQuery();
            query.ViewXml = string.Format(CAML_QUERY_FIND_BY_FILENAME, themeName);
            var found = themeGalleryList.GetItems(query);
            web.Context.Load(found);
            web.Context.ExecuteQuery();
            if (found.Count > 0)
            {
                return true;
            }
            return false;
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
            CreateComposedLookByName(web, themeName, colorFileName, fontFileName, backgroundName, masterPageName, displayOrder:11);
        }

        /// <summary>
        /// Creates (or updates) a composed look in the web site; usually this is done in the root site of the collection.
        /// </summary>
        /// <param name="web">Web to create the composed look in</param>
        /// <param name="lookName">Name of the theme</param>
        /// <param name="paletteFileName">File name of the palette file in the theme catalog of the site collection; path component ignored.</param>
        /// <param name="fontFileName">File name of the font file in the theme catalog of the site collection; path component ignored.</param>
        /// <param name="backgroundFileName">File name of the background image file in the theme catalog of the site collection; path component ignored.</param>
        /// <param name="masterFileName">File name of the master page in the mastepage catalog of the web site; path component ignored.</param>
        /// <param name="displayOrder">Display order of the composed look</param>
        /// <param name="replaceContent">Replace composed look if it already exists (default true)</param>
        public static void CreateComposedLookByName(this Web web, string lookName, string paletteFileName, string fontFileName, string backgroundFileName, string masterFileName, int displayOrder = 1, bool replaceContent = true)
        {
            var paletteUrl = default(string);
            var fontUrl = default(string);
            var backgroundUrl = default(string);
            var masterUrl = default(string);
            using (var innerContext = new ClientContext(web.Context.Url) { Credentials = web.Context.Credentials })
            {
                var rootWeb = innerContext.Site.RootWeb;
                Utility.EnsureWeb(innerContext, rootWeb, "ServerRelativeUrl");

                if (!string.IsNullOrEmpty(paletteFileName))
                {
                    paletteUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMES_DIRECTORY, Path.GetFileName(paletteFileName)));
                }
                if (!string.IsNullOrEmpty(fontFileName))
                {
                    fontUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMES_DIRECTORY, Path.GetFileName(fontFileName)));
                }
                if (!string.IsNullOrEmpty(backgroundFileName))
                {
                    backgroundUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMES_DIRECTORY, Path.GetFileName(backgroundFileName)));
                }
            }
            if (!string.IsNullOrEmpty(masterFileName))
            {
                masterUrl = UrlUtility.Combine(web.ServerRelativeUrl, string.Format(Constants.MASTERPAGE_DIRECTORY, Path.GetFileName(masterFileName)));
            }

            CreateComposedLookByUrl(web, lookName, paletteUrl, fontUrl, backgroundUrl, masterUrl, displayOrder, replaceContent);
        }

        /// <summary>
        /// Creates (or updates) a composed look in the web site; usually this is done in the root site of the collection.
        /// </summary>
        /// <param name="web">Web to create the composed look in</param>
        /// <param name="lookName">Name of the theme</param>
        /// <param name="paletteServerRelativeUrl">URL of the palette file, usually in the theme catalog of the site collection</param>
        /// <param name="fontServerRelativeUrl">URL of the font file, usually in the theme catalog of the site collection</param>
        /// <param name="backgroundServerRelativeUrl">URL of the background image file, usually in /_layouts/15/images</param>
        /// <param name="masterServerRelativeUrl">URL of the master page, usually in the masterpage catalog of the web site</param>
        /// <param name="displayOrder">Display order of the composed look</param>
        /// <param name="replaceContent">Replace composed look if it already exists (default true)</param>
        public static void CreateComposedLookByUrl(this Web web, string lookName, string paletteServerRelativeUrl, string fontServerRelativeUrl, string backgroundServerRelativeUrl, string masterServerRelativeUrl, int displayOrder = 1, bool replaceContent = true)
        {
            Utility.EnsureWeb(web.Context, web, "ServerRelativeUrl");
            var composedLooksList = web.GetCatalog((int)ListTemplateType.DesignCatalog);

            // Check for existing, by name
            CamlQuery query = new CamlQuery();
            query.ViewXml = string.Format(CAML_QUERY_FIND_BY_FILENAME, lookName);
            var existingCollection = composedLooksList.GetItems(query);
            web.Context.Load(existingCollection);
            web.Context.ExecuteQuery();
            ListItem item = existingCollection.FirstOrDefault();

            if (item == null)
            {
                LoggingUtility.Internal.TraceInformation((int)EventId.CreateComposedLook, CoreResources.BrandingExtension_CreateComposedLook, lookName, web.ServerRelativeUrl);
                ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                item = composedLooksList.AddItem(itemInfo);
                item["Name"] = lookName;
                item["Title"] = lookName;
            }
            else
            {
                if (!replaceContent)
                {
                    throw new Exception("Composed look already exists, replace contents needs to be specified.");
                }
                LoggingUtility.Internal.TraceInformation((int)EventId.UpdateComposedLook, CoreResources.BrandingExtension_UpdateComposedLook, lookName, web.ServerRelativeUrl);
            }

            if (!string.IsNullOrEmpty(paletteServerRelativeUrl))
            {
                item["ThemeUrl"] = paletteServerRelativeUrl;
            }
            if (!string.IsNullOrEmpty(fontServerRelativeUrl))
            {
                item["FontSchemeUrl"] = fontServerRelativeUrl;
            }
            if (!string.IsNullOrEmpty(backgroundServerRelativeUrl))
            {
                item["ImageUrl"] = backgroundServerRelativeUrl;
            }
            // we use seattle master if anything else is not set
            if (string.IsNullOrEmpty(masterServerRelativeUrl))
            {
                item["MasterPageUrl"] = UrlUtility.Combine(web.ServerRelativeUrl, Constants.MASTERPAGE_SEATTLE);
            }
            else
            {
                item["MasterPageUrl"] = masterServerRelativeUrl;
            }

            item["DisplayOrder"] = displayOrder;
            item.Update();
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Retrieves the named composed look, overrides with specified palette, font, background and master page, and then recursively sets the specified values.
        /// </summary>
        /// <param name="web">Web to apply composed look to</param>
        /// <param name="lookName">Name of the composed look to apply; null will apply the override values only</param>
        /// <param name="paletteServerRelativeUrl">Override palette file URL to use</param>
        /// <param name="fontServerRelativeUrl">Override font file URL to use</param>
        /// <param name="backgroundServerRelativeUrl">Override background image file URL to use</param>
        /// <param name="masterServerRelativeUrl">Override master page file URL to use</param>
        /// <param name="resetSubsitesToInherit">false (default) to apply to currently inheriting subsites only; true to force all subsites to inherit</param>
        public static void SetComposedLookByUrl(this Web web, string lookName, string paletteServerRelativeUrl = null, string fontServerRelativeUrl = null, string backgroundServerRelativeUrl = null, string masterServerRelativeUrl = null, bool resetSubsitesToInherit = false)
        {
            var paletteUrl = default(string);
            var fontUrl = default(string);
            var backgroundUrl = default(string);
            var masterUrl = default(string);

            if (!string.IsNullOrWhiteSpace(lookName))
            {
                var composedLooksList = web.GetCatalog((int)ListTemplateType.DesignCatalog);

                // Check for existing, by name
                CamlQuery query = new CamlQuery();
                query.ViewXml = string.Format(CAML_QUERY_FIND_BY_FILENAME, lookName);
                var existingCollection = composedLooksList.GetItems(query);
                web.Context.Load(existingCollection);
                web.Context.ExecuteQuery();
                var item = existingCollection.FirstOrDefault();

                if (item != null)
                {
                    var lookPaletteUrl = item["ThemeUrl"] as FieldUrlValue;
                    if (lookPaletteUrl != null)
                    {
                        paletteUrl = new Uri(lookPaletteUrl.Url).AbsolutePath;
                    }
                    var lookFontUrl = item["FontSchemeUrl"] as FieldUrlValue;
                    if (lookFontUrl != null)
                    {
                        fontUrl = new Uri(lookFontUrl.Url).AbsolutePath;
                    }
                    var lookBackgroundUrl = item["ImageUrl"] as FieldUrlValue;
                    if (lookBackgroundUrl != null)
                    {
                        backgroundUrl = new Uri(lookBackgroundUrl.Url).AbsolutePath;
                    }
                    var lookMasterUrl = item["MasterPageUrl"] as FieldUrlValue;
                    if (lookMasterUrl != null)
                    {
                        masterUrl = new Uri(lookMasterUrl.Url).AbsolutePath;
                    }
                }
                else
                {
                    LoggingUtility.Internal.TraceError((int)EventId.ThemeMissing, CoreResources.BrandingExtension_ComposedLookMissing, lookName);
                }
            }

            if (!string.IsNullOrEmpty(paletteServerRelativeUrl))
            {
                paletteUrl = paletteServerRelativeUrl;
            }
            if (!string.IsNullOrEmpty(fontServerRelativeUrl))
            {
                fontUrl = fontServerRelativeUrl;
            }
            if (!string.IsNullOrEmpty(backgroundServerRelativeUrl))
            {
                backgroundUrl = backgroundServerRelativeUrl;
            }
            if (!string.IsNullOrEmpty(masterServerRelativeUrl))
            {
                masterUrl = masterServerRelativeUrl;
            }

            // Save as 'current'
            web.CreateComposedLookByUrl(CurrentLookName, paletteUrl, fontUrl, backgroundUrl, masterUrl, displayOrder: 0);

            web.SetMasterPageByUrl(masterUrl, resetSubsitesToInherit);
            web.SetCustomMasterPageByUrl(masterUrl, resetSubsitesToInherit);
            web.SetThemeByUrl(paletteUrl, fontUrl, backgroundUrl, resetSubsitesToInherit);
        }

        //public static void SetComposedLookInheritFromParent(this Web web, bool resetSubsitesToInherit = false)
        //{
        //    web.SetThemeInheritFromParent(resetSubsitesToInherit);
        //    //web.SetMasterPageInheritFromParent(resetSubsitesToInherit);
        //    //web.SetCustomMasterPageInheritFromParent(resetSubsitesToInherit);
        //}

        /// <summary>
        /// Recursively applies the specified palette, font, and background image.
        /// </summary>
        /// <param name="web">Web to apply to</param>
        /// <param name="paletteServerRelativeUrl">URL of palette file to apply</param>
        /// <param name="fontServerRelativeUrl">URL of font file to apply</param>
        /// <param name="backgroundServerRelativeUrl">URL of background image to apply</param>
        /// <param name="resetSubsitesToInherit">false (default) to apply to currently inheriting subsites only; true to force all subsites to inherit</param>
        /// <param name="updateRootOnly">false (default) to apply to subsites; true to only apply to specified site</param>
        public static void SetThemeByUrl(this Web web, string paletteServerRelativeUrl, string fontServerRelativeUrl, string backgroundServerRelativeUrl, bool resetSubsitesToInherit = false, bool updateRootOnly = false)
        {
            var websToUpdate = new List<Web>();
            web.Context.Load(web, w => w.AllProperties, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            LoggingUtility.Internal.TraceInformation((int)EventId.SetTheme, CoreResources.BrandingExtension_ApplyTheme, paletteServerRelativeUrl, web.ServerRelativeUrl);
            web.AllProperties[InheritTheme] = "False";
            web.Update();
            web.ApplyTheme(paletteServerRelativeUrl, fontServerRelativeUrl, backgroundServerRelativeUrl, shareGenerated: true);
            web.Context.ExecuteQuery();
            //web.Context.Load(web, w => w.ThemedCssFolderUrl);
            //var themedCssFolderUrl = childWeb.ThemedCssFolderUrl;
            websToUpdate.Add(web);

            if (!updateRootOnly)
            {
                var index = 0;
                while (index < websToUpdate.Count)
                {
                    var currentWeb = websToUpdate[index];
                    var websCollection = currentWeb.Webs;
                    web.Context.Load(websCollection, wc => wc.Include(w => w.AllProperties, w => w.ServerRelativeUrl));
                    web.Context.ExecuteQuery();
                    foreach (var childWeb in websCollection)
                    {
                        if (resetSubsitesToInherit || string.Equals(childWeb.AllProperties[InheritTheme].ToString(), "True", StringComparison.InvariantCultureIgnoreCase))
                        {
                            LoggingUtility.Internal.TraceVerbose("Inherited: " + CoreResources.BrandingExtension_ApplyTheme, paletteServerRelativeUrl, childWeb.ServerRelativeUrl);
                            childWeb.AllProperties[InheritTheme] = "True";
                            //childWeb.ThemedCssFolderUrl = themedCssFolderUrl;
                            childWeb.Update();
                            // TODO: CSOM does not support the ThemedCssFolderUrl property yet (Nov 2014), so must call ApplyTheme at each level.
                            // This is very slow, so replace with simply setting the ThemedCssFolderUrl property instead once available.
                            childWeb.ApplyTheme(paletteServerRelativeUrl, fontServerRelativeUrl, backgroundServerRelativeUrl, shareGenerated: true);
                            web.Context.ExecuteQuery();
                            websToUpdate.Add(childWeb);
                        }
                    }
                    index++;
                }
            }
        }

        //public static void SetThemeInheritFromParent(this Web web, bool resetSubsitesToInherit = false, bool updateRootOnly = false)
        //{
        //    throw new NotImplementedException("Need to get theme folder property from parent");
        //    // TODO: Need to get theme folder property from parent so that it can be inherited
        //    // and follow up Inherit chain until can check current property.
        //    var parentWeb = web.ParentWeb;

        //    var websToUpdate = new List<Web>();
        //    web.Context.Load(web, w => w.AllProperties, w => w.ServerRelativeUrl);
        //    web.Context.ExecuteQuery();
        //    if (!string.Equals(web.AllProperties[InheritTheme].ToString(), "True", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        LoggingUtility.Internal.TraceVerbose("Inheriting theme '{0}' in '{1}'.", paletteServerRelativeUrl, web.ServerRelativeUrl);
        //        web.AllProperties[InheritTheme] = "True";
        //        web.Update();
        //        web.ApplyTheme(paletteServerRelativeUrl, fontServerRelativeUrl, backgroundServerRelativeUrl, shareGenerated: true);
        //        web.Context.ExecuteQuery();
        //    }
        //    websToUpdate.Add(web);

        //    if (!updateRootOnly)
        //    {
        //        var index = 0;
        //        while (index < websToUpdate.Count)
        //        {
        //            var currentWeb = websToUpdate[index];
        //            var websCollection = currentWeb.Webs;
        //            web.Context.Load(websCollection, wc => wc.Include(w => w.AllProperties, w => w.ServerRelativeUrl));
        //            web.Context.ExecuteQuery();
        //            foreach (var childWeb in websCollection)
        //            {
        //                if (resetSubsitesToInherit || string.Equals(childWeb.AllProperties[InheritTheme].ToString(), "True", StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    LoggingUtility.Internal.TraceVerbose("Inheriting theme '{0}' in '{1}'.", paletteServerRelativeUrl, childWeb.ServerRelativeUrl);
        //                    childWeb.AllProperties[InheritTheme] = "True";
        //                    childWeb.Update();
        //                    childWeb.ApplyTheme(paletteServerRelativeUrl, fontServerRelativeUrl, backgroundServerRelativeUrl, shareGenerated: true);
        //                    web.Context.ExecuteQuery();
        //                    websToUpdate.Add(childWeb);
        //                }
        //            }
        //            index++;
        //        }
        //    }

        //}

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
        /// Uploads the specified file (usually an spcolor or spfont file) to the web site themes gallery 
        /// (usually only exists in the root web of a site collection).
        /// </summary>
        /// <param name="web">Web site to upload to</param>
        /// <param name="localFilePath">Location of the file to be uploaded</param>
        /// <param name="themeFolderVersion">Leaf folder name to upload to; default is "15"</param>
        /// <returns>The uploaded file, with at least the ServerRelativeUrl property available</returns>
        public static File UploadThemeFile(this Web web, string localFilePath, string themeFolderVersion = "15")
        {
            if (localFilePath == null) { throw new ArgumentNullException("localFilePath"); }
            if (string.IsNullOrWhiteSpace(localFilePath)) { throw new ArgumentException("Source file path is required.", "localFilePath"); }

            var fileName = System.IO.Path.GetFileName(localFilePath);
            using (var localStream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open))
            {
                return UploadThemeFile(web, fileName, localStream, themeFolderVersion);
            }
        }

        /// <summary>
        /// Uploads the specified file (usually an spcolor or spfont file) to the web site themes gallery 
        /// (usually only exists in the root web of a site collection).
        /// </summary>
        /// <param name="web">Web site to upload to</param>
        /// <param name="fileName">Name of the file to upload</param>
        /// <param name="localFilePath">Location of the file to be uploaded</param>
        /// <param name="themeFolderVersion">Leaf folder name to upload to; default is "15"</param>
        /// <returns>The uploaded file, with at least the ServerRelativeUrl property available</returns>
        public static File UploadThemeFile(this Web web, string fileName, string localFilePath, string themeFolderVersion = "15")
        {
            if (fileName == null) { throw new ArgumentNullException("fileName"); }
            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentException("Destination file name is required.", "fileName"); }
            if (localFilePath == null) { throw new ArgumentNullException("localFilePath"); }
            if (string.IsNullOrWhiteSpace(localFilePath)) { throw new ArgumentException("Source file path is required.", "localFilePath"); }

            using (var localStream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open))
            {
                return UploadThemeFile(web, fileName, localStream, themeFolderVersion);
            }
        }

        /// <summary>
        /// Uploads the specified file (usually an spcolor or spfont file) to the web site themes gallery 
        /// (usually only exists in the root web of a site collection).
        /// </summary>
        /// <param name="web">Web site to upload to</param>
        /// <param name="fileName">Name of the file to upload</param>
        /// <param name="localStream">Stream containing the contents of the file</param>
        /// <param name="themeFolderVersion">Leaf folder name to upload to; default is "15"</param>
        /// <returns>The uploaded file, with at least the ServerRelativeUrl property available</returns>
        public static File UploadThemeFile(this Web web, string fileName, System.IO.Stream localStream, string themeFolderVersion = "15")
        {
            if (fileName == null) { throw new ArgumentNullException("fileName"); }
            if (localStream == null) { throw new ArgumentNullException("localStream"); }
            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentException("Destination file name is required.", "fileName"); }
            // TODO: Check for any other illegal characters in SharePoint
            if (fileName.Contains('/') || fileName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single file name and cannot contain path characters.", "fileName");
            }

            // Theme catalog only exists at site collection root
            var themesList = web.GetCatalog((int)ListTemplateType.ThemeCatalog);
            var themesFolder = themesList.RootFolder.EnsureFolder(themeFolderVersion);
            return themesFolder.UploadFile(fileName, localStream);
        }


        /// <summary>
        /// Can be used to deploy page layouts to master page gallery. 
        /// <remarks>Should be only used with root web of site collection where publishing features are enabled.</remarks>
        /// </summary>
        /// <param name="web">Web as the root site of the publishing site collection</param>
        /// <param name="sourceFilePath">Full path to the file which will be uploaded</param>
        /// <param name="title">Title for the page layout</param>
        /// <param name="description">Description for the page layout</param>
        /// <param name="associatedContentTypeID">Associated content type ID</param>
        /// <param name="folderPath">Folder where the page layouts will be stored</param>
        public static void DeployPageLayout(this Web web, string sourceFilePath, string title, string description, string associatedContentTypeID, string folderPath = "")
        {
            if (string.IsNullOrEmpty(sourceFilePath))
                throw new ArgumentNullException("sourceFilePath");

            if (!System.IO.File.Exists(sourceFilePath))
                throw new FileNotFoundException("File for param sourceFilePath file does not exist", sourceFilePath);

            string fileName = Path.GetFileName(sourceFilePath);
            LoggingUtility.Internal.TraceInformation((int)EventId.DeployPageLayout, CoreResources.BrandingExtension_DeployPageLayout, fileName, web.Context.Url);

            // Get the path to the file which we are about to deploy
            List masterPageGallery = web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
            Folder rootFolder = masterPageGallery.RootFolder;
            web.Context.Load(masterPageGallery);
            web.Context.Load(rootFolder);
            web.Context.ExecuteQuery();
            
            // Create folder structure inside master page gallery, if does not exists
            // For e.g.: _catalogs/masterpage/contoso/
            // Create folder if does not exists
            if (!String.IsNullOrEmpty(folderPath))
            {
                web.EnsureFolder(rootFolder, folderPath);
            }

            var fileBytes = System.IO.File.ReadAllBytes(sourceFilePath);

            // Use CSOM to upload the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = fileBytes;
            newFile.Url = UrlUtility.Combine(rootFolder.ServerRelativeUrl, folderPath, fileName);
            newFile.Overwrite = true;

            Microsoft.SharePoint.Client.File uploadFile = rootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();

            // Check out the file if needed
            if (masterPageGallery.ForceCheckout || masterPageGallery.EnableVersioning)
            {
                if (uploadFile.CheckOutType == CheckOutType.None)
                {
                    uploadFile.CheckOut();
                }
            }
            
            // Get content type for ID to assign associated content type information
            ContentType associatedCt = web.GetContentTypeById(associatedContentTypeID);

            var listItem = uploadFile.ListItemAllFields;
            listItem["Title"] = title;
            listItem["MasterPageDescription"] = description;
            // set the item as page layout
            listItem["ContentTypeId"] = Constants.PAGE_LAYOUT_CONTENT_TYPE;
            // Set the associated content type ID property
            listItem["PublishingAssociatedContentType"] = string.Format(";#{0};#{1};#", associatedCt.Name, associatedCt.Id);
            listItem["UIVersion"] = Convert.ToString(15);
            listItem.Update();

            // Check in the page layout if needed
            if (masterPageGallery.ForceCheckout || masterPageGallery.EnableVersioning)
            {
                uploadFile.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                listItem.File.Publish(string.Empty);
            }
            web.Context.ExecuteQuery();

        }

        public static void DeployMasterPage(this Web web, string sourceFilePath, string title, string description, string uiVersion = "15", string defaultCSSFile = "", string folderPath = "")
        {
            if (string.IsNullOrEmpty(sourceFilePath))
                throw new ArgumentNullException("sourceFilePath");

            if (!System.IO.File.Exists(sourceFilePath))
                throw new FileNotFoundException("File for param sourceFilePath not found.", sourceFilePath);
                
            string fileName = Path.GetFileName(sourceFilePath);
            LoggingUtility.Internal.TraceInformation((int)EventId.DeployMasterPage, CoreResources.BrandingExtension_DeployMasterPage, fileName, web.Context.Url);

            // Get the path to the file which we are about to deploy
            List masterPageGallery = web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
            Folder rootFolder = masterPageGallery.RootFolder;
            web.Context.Load(masterPageGallery);
            web.Context.Load(rootFolder);
            web.Context.ExecuteQuery();
            
            // Create folder if does not exists
            if (!String.IsNullOrEmpty(folderPath))
            {
                web.EnsureFolder(rootFolder, folderPath);
            }

            // Get the file name from the provided path
            var fileBytes = System.IO.File.ReadAllBytes(sourceFilePath);

            // Use CSOM to upload the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = fileBytes;
            newFile.Url = UrlUtility.Combine(rootFolder.ServerRelativeUrl, folderPath, fileName);
            newFile.Overwrite = true;

            Microsoft.SharePoint.Client.File uploadFile = rootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();


            var listItem = uploadFile.ListItemAllFields;
            if (masterPageGallery.ForceCheckout || masterPageGallery.EnableVersioning)
            {
                if (uploadFile.CheckOutType == CheckOutType.None)
                {
                    uploadFile.CheckOut();
                }
            }

            listItem["Title"] = title;
            listItem["MasterPageDescription"] = description;
            // Set content type as master page
            listItem["ContentTypeId"] = Constants.MASTERPAGE_CONTENT_TYPE;
            listItem["UIVersion"] = uiVersion;
            listItem.Update();
            if (masterPageGallery.ForceCheckout || masterPageGallery.EnableVersioning)
            {
                uploadFile.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                listItem.File.Publish(string.Empty);
            }
            web.Context.Load(listItem);
            web.Context.ExecuteQuery();

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
        public static void SetMasterPagesByName(this Web web, string masterPageName, string customMasterPageName)
        {
            if (string.IsNullOrEmpty(masterPageName))
            {
                throw (masterPageName == null)
                  ? new ArgumentNullException("masterPageName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "masterPageName");
            }
            if (string.IsNullOrEmpty(customMasterPageName))
            {
                throw (customMasterPageName == null)
                  ? new ArgumentNullException("customMasterPageName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "customMasterPageName");
            }

            web.SetMasterPageByName(masterPageName);
            web.SetCustomMasterPageByName(customMasterPageName);
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
        public static void SetMasterPagesForSiteByUrl(this Web web, string masterPageUrl, string customMasterPageUrl)
        {
            web.SetMasterPagesByUrl(masterPageUrl, customMasterPageUrl);
        }

        /// <summary>
        /// Can be used to set master page and custom master page in single command
        /// </summary>
        /// <param name="web"></param>
        /// <param name="masterPageName"></param>
        /// <param name="customMasterPageName"></param>
        /// <exception cref="System.ArgumentException">Thrown when masterPageName or customMasterPageName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when masterPageName or customMasterPageName is null</exception>
        public static void SetMasterPagesByUrl(this Web web, string masterPageUrl, string customMasterPageUrl)
        {
            if (string.IsNullOrEmpty(masterPageUrl))
            {
                throw (masterPageUrl == null)
                  ? new ArgumentNullException("masterPageName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "masterPageName");
            }
            if (string.IsNullOrEmpty(customMasterPageUrl))
            {
                throw (customMasterPageUrl == null)
                  ? new ArgumentNullException("customMasterPageName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "customMasterPageName");
            }

            web.SetMasterPageByUrl(masterPageUrl);
            web.SetCustomMasterPageByUrl(customMasterPageUrl);
        }

        /// <summary>
        /// Master page is set by using master page name. Master page is set from the current web.
        /// </summary>
        /// <param name="web">Current web</param>
        /// <param name="masterPageName">Name of the master page. Path is resolved from this.</param>
        /// <exception cref="System.ArgumentException">Thrown when masterPageName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when masterPageName is null</exception>  
        [Obsolete("Use SetMasterPageByName")]
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
        public static void SetMasterPageByName(this Web web, string masterPageName)
        {
            if (string.IsNullOrEmpty(masterPageName))
            {
                throw (masterPageName == null)
                  ? new ArgumentNullException("masterPageName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "masterPageName");
            }
            string masterPageUrl = GetRelativeUrlForMasterByName(web, masterPageName);
            if (!string.IsNullOrEmpty(masterPageUrl))
            {
                SetMasterPageByUrl(web, masterPageUrl);
            }
        }

        /// <summary>
        /// Master page is set by using master page name. Master page is set from the current web.
        /// </summary>
        /// <param name="web">Current web</param>
        /// <param name="masterPageName">Name of the master page. Path is resolved from this.</param>
        /// <exception cref="System.ArgumentException">Thrown when masterPageName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when masterPageName is null</exception>  
        [Obsolete("Use SetCustomMasterPageByName")]
        public static void SetCustomMasterPageForSiteByName(this Web web, string masterPageName)
        {
            web.SetCustomMasterPageByName(masterPageName);
        }

        /// <summary>
        /// Master page is set by using master page name. Master page is set from the current web.
        /// </summary>
        /// <param name="web">Current web</param>
        /// <param name="masterPageName">Name of the master page. Path is resolved from this.</param>
        /// <exception cref="System.ArgumentException">Thrown when masterPageName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when masterPageName is null</exception>  
        public static void SetCustomMasterPageByName(this Web web, string masterPageName)
        {
            if (string.IsNullOrEmpty(masterPageName))
            {
                throw (masterPageName == null)
                  ? new ArgumentNullException("masterPageName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "masterPageName");
            }

            string masterPageUrl = GetRelativeUrlForMasterByName(web, masterPageName);
            if (!string.IsNullOrEmpty(masterPageUrl))
            {
                SetCustomMasterPageByUrl(web, masterPageUrl);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "URLs are commonly standardised to lower case.")]
        public static string GetRelativeUrlForMasterByName(this Web web, string masterPageName)
        {
            if (string.IsNullOrEmpty(masterPageName))
                throw new ArgumentNullException("masterPageName");

            List masterPageGallery = web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View><Query><Where><Contains><FieldRef Name='FileRef'/><Value Type='Text'>.master</Value></Contains></Where></Query></View>";
            ListItemCollection galleryItems = masterPageGallery.GetItems(query);
            web.Context.Load(masterPageGallery);
            web.Context.Load(galleryItems);
            web.Context.ExecuteQuery();
            foreach (var item in galleryItems)
            {
                var fileRef = item["FileRef"].ToString();
                if (fileRef.ToUpperInvariant().Contains(masterPageName.ToUpperInvariant()))
                {
                    return fileRef.ToLowerInvariant();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the current theme of a web
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static ThemeEntity GetCurrentTheme(this Web web)
        {
            ThemeEntity theme = null;

            List designCatalog = web.GetCatalog((int)ListTemplateType.DesignCatalog);
            string camlString = @"
            <View>  
                <Query> 
                    <Where><Eq><FieldRef Name='Name' /><Value Type='Text'>Current</Value></Eq></Where> 
                </Query> 
                <ViewFields>
                    <FieldRef Name='ImageUrl' />
                    <FieldRef Name='MasterPageUrl' />
                    <FieldRef Name='FontSchemeUrl' />
                    <FieldRef Name='ThemeUrl' />
                </ViewFields> 
            </View>"; 

            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = camlString;
            
            ListItemCollection themes = designCatalog.GetItems(camlQuery);
            web.Context.Load(themes);
            web.Context.ExecuteQuery();
            if(themes.Count > 0)
            {
                var themeItem = themes[0];
                theme = new ThemeEntity();
                theme.MasterPage = web.MasterUrl;
                theme.CustomMasterPage = web.CustomMasterUrl;
                if (themeItem["ThemeUrl"] != null && themeItem["ThemeUrl"].ToString().Length > 0)
                {
                    theme.Theme = (themeItem["ThemeUrl"] as FieldUrlValue).Url;
                }
                if (themeItem["MasterPageUrl"] != null && themeItem["MasterPageUrl"].ToString().Length > 0)
                {
                    theme.MasterPage = (themeItem["MasterPageUrl"] as FieldUrlValue).Url;
                }
                if (themeItem["FontSchemeUrl"] != null && themeItem["FontSchemeUrl"].ToString().Length > 0)
                {
                    theme.Font = (themeItem["FontSchemeUrl"] as FieldUrlValue).Url;
                }
                if (themeItem["ImageUrl"] != null && themeItem["ImageUrl"].ToString().Length > 0)
                {
                    theme.Font = (themeItem["ImageUrl"] as FieldUrlValue).Url;
                }
            }

            return theme;

        }

        public static ListItem GetPageLayoutListItemByName(this Web web, string pageLayoutName)
        {
            if (string.IsNullOrEmpty(pageLayoutName))
                throw new ArgumentNullException("pageLayoutName");

            List masterPageGallery = web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View><Query><Where><Contains><FieldRef Name='FileRef'/><Value Type='Text'>.aspx</Value></Contains></Where></Query></View>";
            ListItemCollection galleryItems = masterPageGallery.GetItems(query);
            web.Context.Load(masterPageGallery);
            web.Context.Load(galleryItems);
            web.Context.ExecuteQuery();
            foreach (var item in galleryItems)
            {
                var fileRef = item["FileRef"].ToString().ToUpperInvariant();
                if (fileRef.Contains(pageLayoutName.ToUpperInvariant()))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Set master page by using given URL as parameter. Suitable for example in cases where you want sub sites to reference root site master page gallery. This is typical with publishing sites.
        /// </summary>
        /// <param name="web">Context web</param>
        /// <param name="masterPageName">URL to the master page.</param>
        [Obsolete("Use SetMasterPageByUrl")]
        public static void SetMasterPageForSiteByUrl(this Web web, string masterPageUrl)
        {
            web.SetMasterPageByUrl(masterPageUrl, updateRootOnly:true);
        }

        /// <summary>
        /// Set master page by using given URL as parameter. Suitable for example in cases where you want sub sites to reference root site master page gallery. This is typical with publishing sites.
        /// </summary>
        /// <param name="web">Context web</param>
        /// <param name="masterPageServerRelativeUrl">URL to the master page.</param>
        /// <param name="resetSubsitesToInherit">false (default) to apply to currently inheriting subsites only; true to force all subsites to inherit</param>
        /// <param name="updateRootOnly">false (default) to apply to subsites; true to only apply to specified site</param>
        public static void SetMasterPageByUrl(this Web web, string masterPageServerRelativeUrl, bool resetSubsitesToInherit = false, bool updateRootOnly = false)
        {
            if (string.IsNullOrEmpty(masterPageServerRelativeUrl)) { throw new ArgumentNullException("masterPageUrl"); }

            var websToUpdate = new List<Web>();
            web.Context.Load(web, w => w.AllProperties, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            LoggingUtility.Internal.TraceInformation((int)EventId.SetMasterUrl, CoreResources.BrandingExtension_SetMasterUrl, masterPageServerRelativeUrl, web.ServerRelativeUrl);
            web.AllProperties[InheritMaster] = "False";
            web.MasterUrl = masterPageServerRelativeUrl;
            web.Update();
            web.Context.ExecuteQuery();
            websToUpdate.Add(web);

            if (!updateRootOnly)
            {
                var index = 0;
                while (index < websToUpdate.Count)
                {
                    var currentWeb = websToUpdate[index];
                    var websCollection = currentWeb.Webs;
                    web.Context.Load(websCollection, wc => wc.Include(w => w.AllProperties, w => w.ServerRelativeUrl));
                    web.Context.ExecuteQuery();
                    foreach (var childWeb in websCollection)
                    {
                        if (resetSubsitesToInherit || string.Equals(childWeb.AllProperties[InheritTheme].ToString(), "True", StringComparison.InvariantCultureIgnoreCase))
                        {
                            //LoggingUtility.Internal.TraceInformation((int)EventId.SetMasterUrl, CoreResources.BrandingExtension_SetMasterUrl, masterPageServerRelativeUrl, childWeb.ServerRelativeUrl);
                            LoggingUtility.Internal.TraceVerbose("Inherited: " + CoreResources.BrandingExtension_SetMasterUrl, masterPageServerRelativeUrl, childWeb.ServerRelativeUrl);
                            childWeb.AllProperties[InheritMaster] = "True";
                            childWeb.MasterUrl = masterPageServerRelativeUrl;
                            childWeb.Update();
                            web.Context.ExecuteQuery();
                            websToUpdate.Add(childWeb);
                        }
                    }
                    index++;
                }
            }
        }

        [Obsolete("Use Web.SetCustomMasterPageByUrl()")]
        public static void SetCustomMasterPageForSiteByUrl(this Web web, string masterPageUrl)
        {
            web.SetCustomMasterPageByUrl(masterPageUrl, updateRootOnly:true);
        }

        /// <summary>
        /// Set Custom master page by using given URL as parameter. Suitable for example in cases where you want sub sites to reference root site master page gallery. This is typical with publishing sites.
        /// </summary>
        /// <param name="web">Context web</param>
        /// <param name="masterPageName">URL to the master page.</param>
        /// <param name="resetSubsitesToInherit">false (default) to apply to currently inheriting subsites only; true to force all subsites to inherit</param>
        /// <param name="updateRootOnly">false (default) to apply to subsites; true to only apply to specified site</param>
        public static void SetCustomMasterPageByUrl(this Web web, string masterPageServerRelativeUrl, bool resetSubsitesToInherit = false, bool updateRootOnly = false)
        {
            if (string.IsNullOrEmpty(masterPageServerRelativeUrl)) { throw new ArgumentNullException("masterPageUrl"); }

            var websToUpdate = new List<Web>();
            web.Context.Load(web, w => w.AllProperties, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            LoggingUtility.Internal.TraceInformation((int)EventId.SetCustomMasterUrl, CoreResources.BrandingExtension_SetCustomMasterUrl, masterPageServerRelativeUrl, web.ServerRelativeUrl);
            web.AllProperties[InheritMaster] = "False";
            web.CustomMasterUrl = masterPageServerRelativeUrl;
            web.Update();
            web.Context.ExecuteQuery();
            websToUpdate.Add(web);

            if (!updateRootOnly)
            {
                var index = 0;
                while (index < websToUpdate.Count)
                {
                    var currentWeb = websToUpdate[index];
                    var websCollection = currentWeb.Webs;
                    web.Context.Load(websCollection, wc => wc.Include(w => w.AllProperties, w => w.ServerRelativeUrl));
                    web.Context.ExecuteQuery();
                    foreach (var childWeb in websCollection)
                    {
                        if (resetSubsitesToInherit || string.Equals(childWeb.AllProperties[InheritTheme].ToString(), "True", StringComparison.InvariantCultureIgnoreCase))
                        {
                            LoggingUtility.Internal.TraceVerbose("Inherited: " + CoreResources.BrandingExtension_SetCustomMasterUrl, masterPageServerRelativeUrl, childWeb.ServerRelativeUrl);
                            childWeb.AllProperties[InheritMaster] = "True";
                            childWeb.CustomMasterUrl = masterPageServerRelativeUrl;
                            childWeb.Update();
                            web.Context.ExecuteQuery();
                            websToUpdate.Add(childWeb);
                        }
                    }
                    index++;
                }
            }

        }

        /// <summary>
        /// Sets specific page layout the default page layout for the particular site
        /// </summary>
        /// <param name="web"></param>
        /// <param name="rootWeb"></param>
        /// <param name="pageLayoutName"></param>
        public static void SetDefaultPageLayoutForSite(this Web web, Web rootWeb, string pageLayoutName)
        {
            if (rootWeb == null)
                throw new ArgumentNullException("rootWeb");

            if (string.IsNullOrEmpty(pageLayoutName))
                throw new ArgumentNullException("pageLayoutName");

            // Save to property bag as the default page layout for the site
            XmlDocument xd = new XmlDocument();
            var node = CreateXmlNodeFromPageLayout(xd, web, rootWeb, pageLayoutName);
            web.SetPropertyBagValue(DefaultPageLayout, node.OuterXml);
        }

        private static XmlNode CreateXmlNodeFromPageLayout(XmlDocument xd, Web web, Web rootWeb, string pageLayoutName)
        {
            if (xd == null)
                throw new ArgumentNullException("xd");

            if (web == null)
                throw new ArgumentNullException("web");

            if (rootWeb == null)
                throw new ArgumentNullException("rootWeb");

            if (string.IsNullOrEmpty(pageLayoutName))
                throw new ArgumentNullException("pageLayoutName");

            ListItem pageLayout = rootWeb.GetPageLayoutListItemByName(pageLayoutName);

            // Parse the right styled xml for the layout - <layout guid="944ea6be-f287-42c6-aa11-3fd75ab1ee9e" url="_catalogs/masterpage/ArticleLeft.aspx" />
            XmlNode xmlNode = xd.CreateElement("layout");
            XmlAttribute xmlAttribute = xd.CreateAttribute("guid");
            xmlAttribute.Value = pageLayout["UniqueId"].ToString();
            XmlAttribute xmlAttribute2 = xd.CreateAttribute("url");
            // Get relative URL to the particular site collection
            xmlAttribute2.Value = SolveSiteRelativeUrl(rootWeb, pageLayout["FileRef"].ToString());
            xmlNode.Attributes.SetNamedItem(xmlAttribute);
            xmlNode.Attributes.SetNamedItem(xmlAttribute2);
            return xmlNode;
        }

        [Obsolete("Use SolveSiteRelativeUrl")]
        private static string SolveSiteRelateveUrl(Web web, string url)
        {
            return SolveSiteRelativeUrl(web, url);
        }

        private static string SolveSiteRelativeUrl(Web web, string url)
        {
            if (web == null)
                throw new ArgumentNullException("web");

            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            Utility.EnsureWeb(web.Context, web, "ServerRelativeUrl");
            string newUrl = url.Substring(web.ServerRelativeUrl.Length);
            if (newUrl.Length > 0 && newUrl[0] == '/')
            {
                newUrl = newUrl.Substring(1);
            }
            return newUrl;
        }

        /// <summary>
        /// Can be used to set the site to inherit the default page layout option from parent. Cannot be used for root site of the site collection
        /// </summary>
        /// <param name="web"></param>
        public static void SetSiteToInheritPageLayouts(this Web web)
        {
            web.SetPropertyBagValue(DefaultPageLayout, Inherit);
        }

        /// <summary>
        /// Can be used to remote filters from the available page layouts
        /// </summary>
        /// <param name="web"></param>
        public static void ClearAvailablePageLayouts(this Web web)
        {
            web.SetPropertyBagValue(AvailablePageLayouts, "");
        }


        public static void SetAvailablePageLayouts(this Web web, Web rootWeb, IEnumerable<string> pageLayouts)
        {
            XmlDocument xd = new XmlDocument();
            XmlNode xmlNode = xd.CreateElement("pagelayouts");
            xd.AppendChild(xmlNode);
            foreach (var item in pageLayouts)
            {
                var node = CreateXmlNodeFromPageLayout(xd, web, rootWeb, item);
                xmlNode.AppendChild(node);
            }
            web.SetPropertyBagValue(AvailablePageLayouts, xmlNode.OuterXml);
        }

        public static void SetAvailableWebTemplates(this Web web, List<WebTemplateEntity> availableTemplates)
        {
            string propertyValue = string.Empty;

            LanguageTemplateHash languages = new LanguageTemplateHash();
            foreach (var item in availableTemplates)
            {
                AddTemplateToCollection(languages, item);
            }

            if (availableTemplates.Count > 0)
            {
                XmlDocument xd = new XmlDocument();
                XmlNode xmlNode = xd.CreateElement("webtemplates");
                xd.AppendChild(xmlNode);
                foreach (var language in languages)
                {
                    XmlNode xmlLcidNode = xmlNode.AppendChild(xd.CreateElement("lcid"));
                    XmlAttribute xmlAttribute = xd.CreateAttribute("id");
                    xmlAttribute.Value = language.Key;
                    xmlLcidNode.Attributes.SetNamedItem(xmlAttribute);

                    foreach (string item in language.Value)
                    {
                        XmlNode xmlWTNode = xmlLcidNode.AppendChild(xd.CreateElement("webtemplate"));
                        XmlAttribute xmlAttributeName = xd.CreateAttribute("name");
                        xmlAttributeName.Value = item;
                        xmlWTNode.Attributes.SetNamedItem(xmlAttributeName);
                    }
                }
                propertyValue = xmlNode.OuterXml;
            }
            //Save the xml entry to property bag
            web.SetPropertyBagValue(AvailableWebTemplates, propertyValue);
            //Set that templates are not inherited
            web.SetPropertyBagValue(InheritWebTemplates, "False");
        }

        /// <summary>
        /// Can be used to remote filters from the available web template
        /// </summary>
        /// <param name="web"></param>
        public static void ClearAvailableWebTemplates(this Web web)
        {
            web.SetPropertyBagValue(AvailableWebTemplates, "");
        }

        private static void AddTemplateToCollection(LanguageTemplateHash languages, WebTemplateEntity item)
        {
            string key = string.Empty;
            if (string.IsNullOrEmpty(item.LanguageCode))
            {
                key = "all";
            }
            else
            {
                key = item.LanguageCode;
            }

            if (!languages.ContainsKey(key))
            {
                languages[key] = new List<string>();
            }
            (languages[key] as List<string>).Add(item.TemplateName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web">The Web to process</param>
        /// <param name="rootFolderRelativePath">The path relative to the root folder of the site, e.g. SitePages/Home.aspx</param>
        public static void SetHomePage(this Web web, string rootFolderRelativePath)
        {
            Folder folder = web.RootFolder;

            folder.WelcomePage = rootFolderRelativePath;

            folder.Update();

            web.Context.ExecuteQuery();
        }


    }
}
