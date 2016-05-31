using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class AssignedPlan
    {
        public DateTimeOffset AssignedDateTime { get; set; }

        public String CapabilityStatus { get; set; }

        public String Service { get; set; }

        public Guid ServicePlanId { get; set; }
    }
}