using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public partial class ProvisioningTemplateInfo
    {
        #region Private Properties

        private string _templateId;

        #endregion

        public string TemplateId { get { return _templateId; } set { _templateId = value; } }

        public Double TemplateVersion { get; set; }
        public string TemplateSitePolicy { get; set; }
        public DateTime ProvisioningTime { get; set; }
        public bool Result { get; set; }
    }
}
