using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.IO;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectComposedLook : ObjectHandlerBase
    {

        public override string Name
        {
            get { return "Composed Looks"; }
        }



        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {

            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING,"Composed Looks");
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
            if (!web.IsObjectPropertyInstantiated("AlternateCssUrl"))
            {
                web.Context.Load(web);
                executeQueryNeeded = true;
            }
            if (!web.IsObjectPropertyInstantiated("Url"))
            {
                web.Context.Load(web);
                executeQueryNeeded = true;
            }

            if (executeQueryNeeded)
            {
                web.Context.ExecuteQuery();
            }

            // Information coming from the site
            template.ComposedLook.AlternateCSS = web.IsObjectPropertyInstantiated("AlternateCssUrl") ? Tokenize(web.AlternateCssUrl, web.Url) : null;
            template.ComposedLook.MasterPage = Tokenize(web.MasterUrl, web.Url);
            template.ComposedLook.SiteLogo = web.IsObjectPropertyInstantiated("SiteLogoUrl") ? Tokenize(web.SiteLogoUrl, web.Url) : null;

            var theme = web.GetCurrentComposedLook();

            if (theme != null)
            {
                template.ComposedLook.Name = theme.Name;

                if (theme.IsCustomComposedLook)
                {
                    if (creationInfo.PersistComposedLookFiles && creationInfo.FileConnector != null)
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

                    template.ComposedLook.BackgroundFile = Tokenize(theme.BackgroundImage, web.Url);
                    template.ComposedLook.ColorFile = Tokenize(theme.Theme, web.Url);
                    template.ComposedLook.FontFile = Tokenize(theme.Font, web.Url);

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
                    if (creationInfo.BaseTemplate != null)
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
                    // if we've found the file use the provided writer to persist the downloaded file
                    writer.SaveFileStream(f.Src, s);
                }
            }
        }


        private Model.File GetComposedLookFile(string asset)
        {
            int index = asset.LastIndexOf("/");
            Model.File file = new Model.File();
            file.Src = asset.Substring(index + 1);
            file.Folder = asset.Substring(0, index);
            file.Overwrite = true;

            return file;
        }

        private string Tokenize(string url, string webUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }
            else
            {
                if (url.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    return url.Substring(url.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/theme", "{themecatalog}");
                }
                if (url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    return url.Substring(url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/masterpage", "{masterpagecatalog}");
                }
                if (url.IndexOf(webUrl, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    return url.Replace(webUrl, "{site}");
                }
                else
                {
                    Uri r = new Uri(webUrl);
                    if (url.IndexOf(r.PathAndQuery, StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        return url.Replace(r.PathAndQuery, "{site}");
                    }
                }

                // nothing to tokenize...
                return url;
            }
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            return template;
        }
    }
}
