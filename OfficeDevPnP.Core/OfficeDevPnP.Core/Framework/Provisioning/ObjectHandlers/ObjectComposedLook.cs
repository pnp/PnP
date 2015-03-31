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
            TokenParser parser = new TokenParser(web);
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }
            var relativeUrl = web.ServerRelativeUrl;
            if (!string.IsNullOrEmpty(template.ComposedLook.AlternateCSS))
            {
                var alternateCssUrl = parser.Parse(template.ComposedLook.AlternateCSS);
                web.AlternateCssUrl = alternateCssUrl;
                web.Update();
            }
            if (!string.IsNullOrEmpty(template.ComposedLook.SiteLogo))
            {
                var siteLogoUrl = parser.Parse(template.ComposedLook.SiteLogo);
                web.SiteLogoUrl = siteLogoUrl;
                web.Update();
            }
            if (!string.IsNullOrEmpty(template.ComposedLook.MasterPage))
            {
                var masterUrl = parser.Parse(template.ComposedLook.MasterPage);
                web.MasterUrl = masterUrl;
            }
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

            web.ApplyTheme(colorFile, fontFile, backgroundFile, true);
            web.Context.ExecuteQueryRetry();

            // TODO: Add theme handling
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
                    if (template.ComposedLook.BackgroundFile != null)
                    {
                        template.Files.Add(GetComposedLookFile(template.ComposedLook.BackgroundFile));
                    }
                    if (template.ComposedLook.ColorFile != null)
                    {
                        template.Files.Add(GetComposedLookFile(template.ComposedLook.ColorFile));
                    }
                    if (template.ComposedLook.FontFile != null)
                    {
                        template.Files.Add(GetComposedLookFile(template.ComposedLook.FontFile));
                    }
                    if (template.ComposedLook.SiteLogo != null)
                    {
                        template.Files.Add(GetComposedLookFile(template.ComposedLook.SiteLogo));
                    }

                    // If a base template is specified then use that one to "cleanup" the generated template model
                    if (baseTemplate != null)
                    {
                        template = CleanupEntities(template, baseTemplate);
                    }
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
                return url;
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
