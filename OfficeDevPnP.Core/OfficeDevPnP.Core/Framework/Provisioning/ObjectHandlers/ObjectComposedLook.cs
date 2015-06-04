using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.IO;
using OfficeDevPnP.Core.Utilities;
using System.Text.RegularExpressions;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectComposedLook : ObjectHandlerBase
    {

        public override string Name
        {
            get { return "Composed Looks"; }
        }

       

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {

            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_ComposedLooks);
            if (template.ComposedLook != null &&
                !template.ComposedLook.Equals(ComposedLook.Empty))
            {
                bool executeQueryNeeded = false;

                // Apply alternate CSS
                if (!string.IsNullOrEmpty(template.ComposedLook.AlternateCSS))
                {
                    var alternateCssUrl = template.ComposedLook.AlternateCSS.ToParsedString();
                    web.AlternateCssUrl = alternateCssUrl;
                    web.Update();
                    executeQueryNeeded = true;
                }

                // Apply Site logo
                if (!string.IsNullOrEmpty(template.ComposedLook.SiteLogo))
                {
                    var siteLogoUrl = template.ComposedLook.SiteLogo.ToParsedString();
                    web.SiteLogoUrl = siteLogoUrl;
                    web.Update();
                    executeQueryNeeded = true;
                }

                if (executeQueryNeeded)
                {
                    web.Context.ExecuteQueryRetry();
                }

                if (String.IsNullOrEmpty(template.ComposedLook.ColorFile) &&
                    String.IsNullOrEmpty(template.ComposedLook.FontFile) &&
                    String.IsNullOrEmpty(template.ComposedLook.BackgroundFile))
                {
                    // Apply OOB theme
                    web.SetComposedLookByUrl(template.ComposedLook.Name);
                }
                else
                {
                    // Apply custom theme
                    string colorFile = null;
                    if (!string.IsNullOrEmpty(template.ComposedLook.ColorFile))
                    {
                        colorFile = template.ComposedLook.ColorFile.ToParsedString();
                    }
                    string backgroundFile = null;
                    if (!string.IsNullOrEmpty(template.ComposedLook.BackgroundFile))
                    {
                        backgroundFile = template.ComposedLook.BackgroundFile.ToParsedString();
                    }
                    string fontFile = null;
                    if (!string.IsNullOrEmpty(template.ComposedLook.FontFile))
                    {
                        fontFile = template.ComposedLook.FontFile.ToParsedString();
                    }

                    string masterUrl = null;
                    if (!string.IsNullOrEmpty(template.ComposedLook.MasterPage))
                    {
                        masterUrl = template.ComposedLook.MasterPage.ToParsedString();
                    }
                    web.CreateComposedLookByUrl(template.ComposedLook.Name, colorFile, fontFile, backgroundFile, masterUrl);
                    web.SetComposedLookByUrl(template.ComposedLook.Name, colorFile, fontFile, backgroundFile, masterUrl);
                }
            }
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            // Load object if not there
            bool executeQueryNeeded = false;
            if (!web.IsPropertyAvailable("Url"))
            {
                web.Context.Load(web, w => w.Url);
                executeQueryNeeded = true;
            }
            if (!web.IsPropertyAvailable("MasterUrl"))
            {
                web.Context.Load(web, w => w.MasterUrl);
                executeQueryNeeded = true;
            }
#if !CLIENTSDKV15
            if (!web.IsPropertyAvailable("AlternateCssUrl"))
            {
                web.Context.Load(web, w => w.AlternateCssUrl);
                executeQueryNeeded = true;
            }
            if (!web.IsPropertyAvailable("SiteLogoUrl"))
            {
                web.Context.Load(web, w => w.SiteLogoUrl);
                executeQueryNeeded = true;
            }
#endif
            if (executeQueryNeeded)
            {
                web.Context.ExecuteQuery();
            }

            // Information coming from the site
            template.ComposedLook.MasterPage = Tokenize(web.MasterUrl, web.Url);
#if !CLIENTSDKV15
            template.ComposedLook.AlternateCSS = Tokenize(web.AlternateCssUrl, web.Url);
            template.ComposedLook.SiteLogo = Tokenize(web.SiteLogoUrl, web.Url);
#else
            template.ComposedLook.AlternateCSS = null;
            template.ComposedLook.SiteLogo = null;
#endif
            var theme = web.GetCurrentComposedLook();


            if (theme != null)
            {
                // Don't exclude the DesignPreviewThemedCssFolderUrl property bag, if any
                creationInfo.PropertyBagPropertiesToPreserve.Add("DesignPreviewThemedCssFolderUrl");

                template.ComposedLook.Name = theme.Name;

                if (theme.IsCustomComposedLook)
                {
                    if (creationInfo != null && creationInfo.PersistComposedLookFiles && creationInfo.FileConnector != null)
                    {
                        Site site = (web.Context as ClientContext).Site;
                        if (!site.IsObjectPropertyInstantiated("Url"))
                        {
                            web.Context.Load(site);
                            web.Context.ExecuteQueryRetry();
                        }

                        // Let's create a SharePoint connector since our files anyhow are in SharePoint at this moment
                        SharePointConnector spConnector = new SharePointConnector(web.Context, web.Url, "dummy");

                        // to get files from theme catalog we need a connector linked to the root site
                        SharePointConnector spConnectorRoot;
                        if (!site.Url.Equals(web.Url, StringComparison.InvariantCultureIgnoreCase))
                        {
                            spConnectorRoot = new SharePointConnector(web.Context.Clone(site.Url), site.Url, "dummy");
                        }
                        else
                        {
                            spConnectorRoot = spConnector;
                        }

                        // Download the theme/branding specific files
                        DownLoadFile(spConnector, spConnectorRoot, creationInfo.FileConnector, web.Url, web.AlternateCssUrl);
                        DownLoadFile(spConnector, spConnectorRoot, creationInfo.FileConnector, web.Url, web.SiteLogoUrl);
                        DownLoadFile(spConnector, spConnectorRoot, creationInfo.FileConnector, web.Url, theme.BackgroundImage);
                        DownLoadFile(spConnector, spConnectorRoot, creationInfo.FileConnector, web.Url, theme.Theme);
                        DownLoadFile(spConnector, spConnectorRoot, creationInfo.FileConnector, web.Url, theme.Font);
                    }

                    template.ComposedLook.BackgroundFile = FixFileUrl(Tokenize(theme.BackgroundImage, web.Url));
                    template.ComposedLook.ColorFile = FixFileUrl(Tokenize(theme.Theme, web.Url));
                    template.ComposedLook.FontFile = FixFileUrl(Tokenize(theme.Font, web.Url));

                    // Create file entries for the custom theme files  
                    if (!string.IsNullOrEmpty(template.ComposedLook.BackgroundFile))
                    {
                        template.Files.Add(GetComposedLookFile(template.ComposedLook.BackgroundFile));
                    }
                    if (!string.IsNullOrEmpty(template.ComposedLook.ColorFile))
                    {
                        template.Files.Add(GetComposedLookFile(template.ComposedLook.ColorFile));
                    }
                    if (!string.IsNullOrEmpty(template.ComposedLook.FontFile))
                    {
                        template.Files.Add(GetComposedLookFile(template.ComposedLook.FontFile));
                    }
                    if (!string.IsNullOrEmpty(template.ComposedLook.SiteLogo))
                    {
                        template.Files.Add(GetComposedLookFile(template.ComposedLook.SiteLogo));
                    }

                    // If a base template is specified then use that one to "cleanup" the generated template model
                    if (creationInfo != null && creationInfo.BaseTemplate != null)
                    {
                        template = CleanupEntities(template, creationInfo.BaseTemplate);
                    }
                }
                else
                {
                    template.ComposedLook.BackgroundFile = "";
                    template.ComposedLook.ColorFile = "";
                    template.ComposedLook.FontFile = "";
                }
            }
            else
            {
                template.ComposedLook = null;
            }

            return template;
        }

        private void DownLoadFile(SharePointConnector reader, SharePointConnector readerRoot, FileConnectorBase writer, string webUrl, string asset)
        {

            // No file passed...leave
            if (String.IsNullOrEmpty(asset))
            {
                return;
            }

            SharePointConnector readerToUse;
            Model.File f = GetComposedLookFile(asset);

            // Strip the /sites/root part from /sites/root/lib/folder structure
            Uri u = new Uri(webUrl);
            if (f.Folder.IndexOf(u.PathAndQuery, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                f.Folder = f.Folder.Replace(u.PathAndQuery, "");
            }

            // in case of a theme catalog we need to use the root site reader as that list only exists on root site level
            if (f.Folder.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                readerToUse = readerRoot;
            }
            else
            {
                readerToUse = reader;
            }

            using (Stream s = readerToUse.GetFileStream(f.Src, f.Folder))
            {
                if (s != null)
                {
                    writer.SaveFileStream(f.Src, s);
                }
            }
        }

        private String FixFileName(string originalFileName)
        {
            // if we've found the file use the provided writer to persist the downloaded file
            String regexStrip = @"(\\|/|:|\*|\?|""|>|<|\||=)*";
            String result = Regex.Replace(originalFileName.Substring(0,
                originalFileName.IndexOf("?") > 0 ? originalFileName.IndexOf("?") : originalFileName.Length),
                regexStrip, "", RegexOptions.IgnorePatternWhitespace);

            return (result);
        }
        private String FixFileUrl(string originalFileUrl)
        {
            if (string.IsNullOrEmpty(originalFileUrl))
            {
                return "";
            }

            String fileUrl = originalFileUrl.Substring(0, originalFileUrl.LastIndexOf("/"));
            String fileName = FixFileName(originalFileUrl.Substring(originalFileUrl.LastIndexOf("/") + 1));

            String result = String.Format("{0}/{1}", fileUrl, fileName);

            return (result);
        }

        private Model.File GetComposedLookFile(string asset)
        {
            int index = asset.LastIndexOf("/");
            Model.File file = new Model.File();
            file.Src = FixFileName(asset.Substring(index + 1));
            file.Folder = asset.Substring(0, index);
            file.Overwrite = true;

            return file;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            return template;
        }

        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = (template.ComposedLook != null && !template.ComposedLook.Equals(ComposedLook.Empty));
            }
            return _willProvision.Value;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (!_willExtract.HasValue)
            {
                _willExtract = true;
            }
            return _willExtract.Value;
        }
    }
}
