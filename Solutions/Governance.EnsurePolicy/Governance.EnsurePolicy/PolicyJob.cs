using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Framework.TimerJobs;
using System;

namespace Governance.EnsurePolicy
{
    public class PolicyJob: TimerJob
    {

        public string ProvisioningTemplateToApply { get; set; }

        public PolicyJob(): base("PolicyJob", "1.0")
        {
            TimerJobRun += PolicyJob_TimerJobRun;
        }

        private void PolicyJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            try
            {
                Console.WriteLine($"Processing site {e.Url}");

                ProvisioningTemplateApplyingInformation ptai = new ProvisioningTemplateApplyingInformation()
                {
                    HandlersToProcess = Handlers.SiteSecurity,
                };

                XMLTemplateProvider provider = new XMLFileSystemTemplateProvider(@".", "");
                ProvisioningTemplate sourceTemplate = provider.GetTemplate(ProvisioningTemplateToApply);

                e.WebClientContext.Web.ApplyProvisioningTemplate(sourceTemplate, ptai);
            }
            catch(Exception ex)
            {
                // Catch exceptions to avoid ending the program run since we're typically processing multiple site collections
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
