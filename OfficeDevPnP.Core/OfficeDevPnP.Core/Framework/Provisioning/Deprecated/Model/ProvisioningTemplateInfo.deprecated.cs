using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public partial class ProvisioningTemplateInfo
    {
        #region Will be deprecated in June 2015 release

        [Obsolete("Use TemplateId to set the identity of the template. This deprecated property will be removed in the June 2015 release.")]
        public string TemplateID { get { return _templateId; } set { _templateId = value; } }

        #endregion

    }
}