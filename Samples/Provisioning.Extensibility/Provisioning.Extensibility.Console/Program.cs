using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Extensibility.Providers.Extensions;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace Provisioning.Extensibility.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string tenantAdminUser = "user@your_tenant.onmicrosoft.com";
            string tenantAdminPassword = "XXXXXX";
            string siteCollectionUrl = "https://your_tenant.sharepoint.com/sites/entwikipnp";

            XMLTemplateProvider provider =
                        new XMLFileSystemTemplateProvider(Environment.CurrentDirectory, "");

            var template = provider.GetTemplate("Templates\\PublishingPageProviderDemo.xml");

            using (ClientContext context = new ClientContext(siteCollectionUrl))
            {
                context.Credentials =
                    new SharePointOnlineCredentials(
                        tenantAdminUser, 
                        tenantAdminPassword.ToSecureString());

                context.Web.ApplyProvisioningTemplate(template);
            }
        }
    }
}
