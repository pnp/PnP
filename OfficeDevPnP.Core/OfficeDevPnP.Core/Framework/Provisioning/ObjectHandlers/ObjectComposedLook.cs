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
            web.AlternateCssUrl = template.ComposedLook.AlternateCSS;
            web.SiteLogoUrl = template.ComposedLook.SiteLogo;
    //        web.MasterUrl = template.ComposedLook.MasterPage;

            web.SetComposedLookByUrl(template.ComposedLook.Name,template.ComposedLook.ColorFile,template.ComposedLook.FontFile,template.ComposedLook.BackgroundFile,template.ComposedLook.MasterPage);

    

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

            var theme = web.GetComposedLook("Current");
           
            // Get needed data from the site
            // TODO: Access currently set theme for details
            template.ComposedLook.AlternateCSS = web.AlternateCssUrl;
            template.ComposedLook.BackgroundFile = theme.BackgroundImage;
            template.ComposedLook.ColorFile = theme.Theme;
            template.ComposedLook.FontFile = theme.Font;
            template.ComposedLook.MasterPage = web.MasterUrl;
            template.ComposedLook.Name = "";
            template.ComposedLook.SiteLogo = web.SiteLogoUrl;

            return template;
        }
    }
}
