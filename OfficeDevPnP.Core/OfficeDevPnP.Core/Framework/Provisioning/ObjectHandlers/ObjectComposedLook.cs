using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectComposedLook : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            if (template.ComposedLook != null)
            {
                bool executeQueryNeeded = false;
                TokenParser parser = new TokenParser(web);
                
                // Apply alternate CSS
                if (!string.IsNullOrEmpty(template.ComposedLook.AlternateCSS))
                {
                    var alternateCssUrl = parser.Parse(template.ComposedLook.AlternateCSS);
                    web.AlternateCssUrl = alternateCssUrl;
                    web.Update();
                    executeQueryNeeded = true;
                }
                
                // Apply Site logo
                if (!string.IsNullOrEmpty(template.ComposedLook.SiteLogo))
                {
                    var siteLogoUrl = parser.Parse(template.ComposedLook.SiteLogo);
                    web.SiteLogoUrl = siteLogoUrl;
                    web.Update();
                    executeQueryNeeded = true;
                }

                if (executeQueryNeeded)
                {
                    web.Context.ExecuteQueryRetry();
                }

                if (template.ComposedLook.ColorFile.Length == 0 &&
                    template.ComposedLook.FontFile.Length == 0 &&
                    template.ComposedLook.BackgroundFile.Length == 0)
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
                        colorFile = parser.Parse(template.ComposedLook.ColorFile);
                    }
                    string backgroundFile = null;
                    if (!string.IsNullOrEmpty(template.ComposedLook.BackgroundFile))
                    {
                        backgroundFile = parser.Parse(template.ComposedLook.BackgroundFile);
                    }
                    string fontFile = null;
                    if (!string.IsNullOrEmpty(template.ComposedLook.FontFile))
                    {
                        fontFile = parser.Parse(template.ComposedLook.FontFile);
                    }

                    string masterUrl = null;
                    if (!string.IsNullOrEmpty(template.ComposedLook.MasterPage))
                    {
                        masterUrl = parser.Parse(template.ComposedLook.MasterPage);
                    }
                    web.CreateComposedLookByUrl(template.ComposedLook.Name, colorFile, fontFile, backgroundFile, masterUrl);
                    web.SetComposedLookByUrl(template.ComposedLook.Name, colorFile, fontFile, backgroundFile, masterUrl);
                }
            }
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
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
            template.ComposedLook.AlternateCSS = Tokenize(web.AlternateCssUrl, web.Url);
            template.ComposedLook.MasterPage = Tokenize(web.MasterUrl, web.Url);
            template.ComposedLook.SiteLogo = Tokenize(web.SiteLogoUrl, web.Url);

            var theme = web.GetCurrentComposedLook();

            if (theme != null)
            {
                template.ComposedLook.Name = theme.Name;

                if (theme.IsCustomComposedLook)
                {
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
                    if (baseTemplate != null)
                    {
                        template = CleanupEntities(template, baseTemplate);
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
                    return url.Substring(url.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/theme", "~themecatalog");
                }
                if (url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    return url.Substring(url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/masterpage", "~masterpagecatalog");
                }
                if (url.IndexOf(webUrl, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    return url.Replace(webUrl, "~site");
                }
                else
                {
                    Uri r = new Uri(webUrl);
                    if (url.IndexOf(r.PathAndQuery, StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        return url.Replace(r.PathAndQuery, "~site");
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
