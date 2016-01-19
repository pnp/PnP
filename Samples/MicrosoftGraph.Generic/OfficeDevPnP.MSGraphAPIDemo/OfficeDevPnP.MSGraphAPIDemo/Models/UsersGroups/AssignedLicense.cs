using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class AssignedLicense
    {
        public List<Guid> DisabledPlans { get; set; }

        public Guid SkuId { get; set; }
    }
}