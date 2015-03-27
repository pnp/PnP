using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    class ObjectComposedLook : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, ProvisioningTemplate template)
        {
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }
            var relativeUrl = web.ServerRelativeUrl;
            if (template.ComposedLook.AlternateCSS != null)
            {
                var alternateCssUrl = UrlUtility.Combine(relativeUrl, template.ComposedLook.AlternateCSS);
                web.AlternateCssUrl = alternateCssUrl;
                web.Update();
            }
            if (template.ComposedLook.SiteLogo != null)
            {
                var siteLogoUrl = UrlUtility.Combine(relativeUrl, template.ComposedLook.SiteLogo);
                web.SiteLogoUrl = siteLogoUrl;
                web.Update();
            }
            string masterUrl = null;
            if (template.ComposedLook.MasterPage != null)
            {
                masterUrl = UrlUtility.Combine(relativeUrl, template.ComposedLook.MasterPage);
                web.MasterUrl = masterUrl;
            }
            string colorFile = null;
            if (template.ComposedLook.ColorFile != null)
            {
                colorFile = UrlUtility.Combine(relativeUrl, template.ComposedLook.ColorFile);
            }
            string backgroundFile = null;
            if (template.ComposedLook.BackgroundFile != null)
            {
                backgroundFile = UrlUtility.Combine(relativeUrl, template.ComposedLook.BackgroundFile);
            }
            string fontFile = null;
            if (template.ComposedLook.FontFile != null)
            {
                fontFile = UrlUtility.Combine(relativeUrl, template.ComposedLook.FontFile);
            }

            web.ApplyTheme(colorFile, fontFile, backgroundFile, true);

            web.Context.ExecuteQueryRetry();

            // TODO: Add theme handling
        }

        public override ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, ProvisioningTemplate template)
        {
            // Load object if not there
            if (!web.IsObjectPropertyInstantiated("AlternateCssUrl"))
            {
                web.Context.Load(web);
                web.Context.ExecuteQuery();
            }

            // TODO: review
            template.ComposedLook = null;

            //var theme = web.GetComposedLook("Current");

            //// Get needed data from the site
            //// TODO: Access currently set theme for details
            //template.ComposedLook.AlternateCSS = web.AlternateCssUrl;
            //template.ComposedLook.BackgroundFile = theme.BackgroundImage;
            //template.ComposedLook.ColorFile = theme.Theme;
            //template.ComposedLook.FontFile = theme.Font;
            //template.ComposedLook.MasterPage = web.MasterUrl;
            //template.ComposedLook.Name = "";
            //template.ComposedLook.SiteLogo = web.SiteLogoUrl;

            return template;
        }
    }
}
