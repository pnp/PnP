using Microsoft.Online.SharePoint.TenantAdministration;
using OfficeDevPnP.Core.Framework.TimerJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace Core.TimerJobs.Samples.TenantAPIJob
{
    public class TenantAPIJob: TimerJob
    {
        public TenantAPIJob()
            : base("TenantAPIJob", "1.0")
        {
            TimerJobRun += TenantAPIJob_TimerJobRun;
        }

        void TenantAPIJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            Tenant t = new Tenant(e.TenantClientContext);
            var sites = t.GetSiteProperties(0, true);
            e.TenantClientContext.Load(sites);
            e.TenantClientContext.ExecuteQueryRetry();

            foreach(var site in sites)
            {
                Console.WriteLine(site.Template);
            }

        }
    }
}
