using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Services.Discovery;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.InformationPolicy;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Utilities;
using Field = OfficeDevPnP.Core.Framework.Provisioning.Model.Field;
using SPField = Microsoft.SharePoint.Client.Field;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectSitePolicy : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Site Policy"; }
        }
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_SitePolicy);

            if (template.SitePolicy != null)
            {
                if (web.GetSitePolicyByName(template.SitePolicy) != null) // Site Policy Available?
                {
                    web.ApplySitePolicy(template.SitePolicy);
                }
            }
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {

            var sitePolicyEntity = web.GetAppliedSitePolicy();

            if (sitePolicyEntity != null)
            {
                template.SitePolicy = sitePolicyEntity.Name;
            }
            return template;
        }

    }
}

