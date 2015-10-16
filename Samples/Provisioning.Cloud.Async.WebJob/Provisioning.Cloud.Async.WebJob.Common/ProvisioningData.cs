using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Cloud.Async.WebJob.Common
{
    public class ProvisioningData
    {
        public SiteRequestData RequestData { get; set; }

        public SiteBrandingData BrandingData { get; set; }

        public string TenantName { get; set; }
    }
}
