using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class SiteMetadata
    {
        #region Instance Members
        private List<string> _additionalAdmins = new List<string>();
        #endregion

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "hostPath")]
        public string HostPath { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "tenantAdminUrl")]
        public string TenantAdminUrl { get; set; }

        [DataMember(Name = "enableExternalSharing")]
        public bool EnableExternalSharing { get; set; }

        [DataMember(Name = "tenantSharingEnabled")]
        public bool TenantSharingEnabled { get; set; }

        [DataMember(Name = "siteSharingEnabled")]
        public bool SiteSharingEnabled { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "lcid")]
        public uint lcid { get; set; }

        [DataMember(Name = "timezoneID")]
        public int TimeZoneID { get; set; }

        [DataMember(Name = "primaryOwnerEmail")]
        public string PrimaryOwnerEmail { get; set; }

        [DataMember(Name = "primaryOwnerName")]
        public string PrimaryOwnerName { get; set; }

        [DataMember(Name = "sharePointOnPremises")]
        public bool SharePointOnPremises { get; set; }

        [DataMember(Name = "businessUnit")]
        public string BusinessUnit { get; set; }

        [DataMember(Name = "region")]
        public string Region { get; set; }

        [DataMember(Name = "function")]
        public string Function { get; set; }

        [DataMember(Name = "sitePolicy")]
        public string SitePolicy { get; set; }

        [DataMember(Name = "sitePolicyName")]
        public string SitePolicyName { get; set; }

        [DataMember(Name = "sitePolicyExpirationDate")]
        public string SitePolicyExpirationDate { get; set; }

        [DataMember(Name = "division")]
        public string Division { get; set; }        

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }
    }
}