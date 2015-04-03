using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class ProvisioningTemplateInfo
    {
        public string TemplateID { get; set; }
        public Double TemplateVersion { get; set; }
        public string TemplateSitePolicy { get; set; }
        public DateTime ProvisioningTime { get; set; }
        public bool Result { get; set; }
    }
}
