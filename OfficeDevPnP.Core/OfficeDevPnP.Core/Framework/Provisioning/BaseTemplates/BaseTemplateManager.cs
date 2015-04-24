using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// This class will be used to provide access to the right base template configuration
    /// </summary>
    public static class BaseTemplateManager
    {

        public static ProvisioningTemplate GetBaseTemplate(this Web web)
        {
            web.Context.Load(web, p => p.WebTemplate, p => p.Configuration);
            web.Context.ExecuteQueryRetry();

            ProvisioningTemplate provisioningTemplate = null;

            try
            {
                string baseTemplate = string.Format("OfficeDevPnP.Core.Framework.Provisioning.BaseTemplates.v{0}.{1}{2}Template.xml", GetSharePointVersion(), web.WebTemplate, web.Configuration);
                using (Stream stream = typeof(BaseTemplateManager).Assembly.GetManifestResourceStream(baseTemplate))
                {
                    // Get the XML document from the stream
                    ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(XMLPnPSchemaVersion.V201503);

                    // And convert it into a ProvisioningTemplate
                    provisioningTemplate = formatter.ToProvisioningTemplate(stream);
                }
            }
            catch(Exception)
            {
                //TODO: log message
            }

            return provisioningTemplate;
        }

        private static int GetSharePointVersion()
        {
            Assembly asm = Assembly.GetAssembly(typeof(Site));
            return asm.GetName().Version.Major;
        }

    }
}
