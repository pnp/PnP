using System;
using Newtonsoft.Json;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public partial class ProvisioningTemplateInfo
    {
        #region Will be deprecated in June 2015 release

        [Obsolete("Use TemplateId to set the identity of the template. This deprecated property will be removed in the June 2015 release.")]
        [JsonIgnore]
        public string TemplateID { get { return _templateId; } set { _templateId = value; } }

        [JsonProperty("TemplateID")]
        private string TemplateIDAlternateSetter
        {
            // get is intentionally omitted here
            set { TemplateID = value; }
        }
        #endregion

    }
}