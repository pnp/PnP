using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.UPAWebService;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class SiteToTemplateConversion
    {
        
        /// <summary>
        /// Actual implementation of extracting configuration from existing site.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="baseTemplate"></param>
        /// <returns></returns>
        internal ProvisioningTemplate GetRemoteTemplate(Web web, ProvisioningTemplateCreationInformation creationInfo)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_StartExtraction);
            
            ProvisioningProgressDelegate progressDelegate = null;
            
            if (creationInfo != null)
            {
                progressDelegate = creationInfo.ProgressDelegate;
            }

            // Create empty object
            ProvisioningTemplate template = new ProvisioningTemplate();

            // Hookup connector, is handy when the generated template object is used to apply to another site
            template.Connector = creationInfo.FileConnector;

            List<ObjectHandlerBase> objectHandlers = new List<ObjectHandlerBase>();

            objectHandlers.Add(new ObjectSitePolicy());
            objectHandlers.Add(new ObjectSiteSecurity());
            objectHandlers.Add(new ObjectTermGroups());
            objectHandlers.Add(new ObjectField());
            objectHandlers.Add(new ObjectContentType());
            objectHandlers.Add(new ObjectListInstance());
            objectHandlers.Add(new ObjectCustomActions());
            objectHandlers.Add(new ObjectFeatures());
            objectHandlers.Add(new ObjectComposedLook());
            objectHandlers.Add(new ObjectFiles());
            objectHandlers.Add(new ObjectPages());
            objectHandlers.Add(new ObjectPropertyBagEntry());
            objectHandlers.Add(new ObjectRetrieveTemplateInfo());

            int step = 1;

            var count = objectHandlers.Count(o => o.ReportProgress);

            foreach (var handler in objectHandlers)
            {
                if (handler.ReportProgress && progressDelegate != null)
                {
                    progressDelegate(handler.Name, step, count);
                    step++;
                }
                template = handler.CreateEntities(web, template, creationInfo);
            }
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_FinishExtraction);
            return template;
        }

        /// <summary>
        /// Actual implementation of the apply templates
        /// </summary>
        /// <param name="web"></param>
        /// <param name="template"></param>
        internal void ApplyRemoteTemplate(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation provisioningInfo)
        {
            ProvisioningProgressDelegate progressDelegate = null;
            if (provisioningInfo != null)
            {
                progressDelegate = provisioningInfo.ProgressDelegate;
            }

            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_StartProvisioning);

            List<ObjectHandlerBase> objectHandlers = new List<ObjectHandlerBase>();

            objectHandlers.Add(new ObjectSitePolicy());
            objectHandlers.Add(new ObjectSiteSecurity());
            objectHandlers.Add(new ObjectFeatures());
            objectHandlers.Add(new ObjectTermGroups());
            objectHandlers.Add(new ObjectField());
            objectHandlers.Add(new ObjectContentType());
            objectHandlers.Add(new ObjectListInstance());
            objectHandlers.Add(new ObjectLookupFields());
            objectHandlers.Add(new ObjectFiles());
            objectHandlers.Add(new ObjectPages());
            objectHandlers.Add(new ObjectCustomActions());
            objectHandlers.Add(new ObjectComposedLook());
            objectHandlers.Add(new ObjectPropertyBagEntry());
            objectHandlers.Add(new ObjectExtensibilityProviders());
            objectHandlers.Add(new ObjectPersistTemplateInfo());

            TokenParser.Initialize(web, template);

            int step = 1;

            var count = objectHandlers.Count(o => o.ReportProgress);

            foreach (var handler in objectHandlers)
            {
                if (handler.ReportProgress && progressDelegate != null)
                {
                    progressDelegate(handler.Name, step, count);
                    step++;
                }
                handler.ProvisionObjects(web, template);
            }

            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_FinishProvisioning);
        }
    }
}
