using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class ProvisionedPlan
    {
        public String CapabilityStatus { get; set; }

        public String ProvisioningStatus { get; set; }

        public String Service { get; set; }
    }
}