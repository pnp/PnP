using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class ProvisioningTemplateInfo
    {
        public string TemplateID { get; set; }
        public DateTime ProvisioningTime { get; set; }
        public bool Result { get; set; }
    }
}
