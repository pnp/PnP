using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class ExternalSharingRequest
    {
        [DataMember(Name = "tenantAdminUrl")]
        public string TenantAdminUrl { get; set; }

        [DataMember(Name = "siteUrl")]
        public string SiteUrl { get; set; }

        [DataMember(Name = "externalSharingEnabled")]
        public bool ExternalSharingEnabled { get; set; }

        [DataMember(Name = "siteExternalSharingEnabled")]
        public bool SiteExternalSharingEnabled { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}