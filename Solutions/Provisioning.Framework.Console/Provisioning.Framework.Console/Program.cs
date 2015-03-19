using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Framework.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string templateSiteUrl = "https://vesaj.sharepoint.com/sites/template";
            string targetSiteUrl = "https://vesaj.sharepoint.com/sites/target";
            string loginId = "vesaj@veskuonline.com";

            // Get pwd from environment variable, so that we do nto need to show that.
            string pwd = System.Environment.GetEnvironmentVariable("MSOPWD", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(pwd))
            {
                System.Console.WriteLine("MSOPWD user environment variable empty, cannot continue. Press any key to abort.");
                System.Console.ReadKey();
                return;
            }
            // Template 
            ProvisioningTemplate template;

            // Get access to source site
            using (var ctx = new ClientContext(templateSiteUrl))
            {
                //Provide count and pwd for connecting to the source
                var passWord = new SecureString();
                foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(loginId, passWord);

                // Get template from existing site
                template = ctx.Web.GetProvisioningTemplate();

            }

            // Get access to target site and apply template
            using (var ctx = new ClientContext(targetSiteUrl))
            {
                //Provide count and pwd for connecting to the source
                var passWord = new SecureString();
                foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(loginId, passWord);

                // Apply template to existing site
                ctx.Web.ApplyProvisioningTemplate(template);
            }

            // Save template using XML provider
            // TODO: show provider model for config files
            XMLTemplateProvider provider = new XMLTemplateProvider();
            provider.Connector = new FileSystemConnector();
            provider.Connector.Parameters.Add("FilePath", "c:\temp");
            provider.Connector.Parameters.Add("FileName", "template.xml");
            provider.Save(template);
            
            // Load templates from provider model, notice that the FilePath is already set above
            provider.Connector.Parameters.Add("FileName", "templates.xml");
            List<ProvisioningTemplate> templates = provider.GetTemplates();

            // If connector is for Azure, parameters are different... 
        }
    }
}

