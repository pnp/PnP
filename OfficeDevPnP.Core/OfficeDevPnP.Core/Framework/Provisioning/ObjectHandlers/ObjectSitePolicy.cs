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
    internal class ObjectSitePolicy : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Site Policy"; }
        }
        public override void ProvisionObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation)
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

        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {

            var sitePolicyEntity = web.GetAppliedSitePolicy();

            if (sitePolicyEntity != null)
            {
                template.SitePolicy = sitePolicyEntity.Name;
            }
            return template;
        }


        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = template.SitePolicy != null;
            }
            return _willProvision.Value;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (!_willExtract.HasValue)
            {
                var sitePolicyEntity = web.GetAppliedSitePolicy();

                _willExtract = sitePolicyEntity != null;
            }
            return _willExtract.Value;
        }
    }
}

