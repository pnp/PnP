using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class SiteRequest
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

        [DataMember(Name = "enableExternalSharing")]
        public bool EnableExternalSharing { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "lcid")]
        public uint lcid { get; set; }

        [DataMember(Name = "timezoneID")]
        public int TimeZoneID { get; set; }

        [DataMember(Name = "primaryOwner")]
        public string PrimaryOwner { get; set; }

        [DataMember(Name = "additionalAdministrators")]
        public List<string> AdditionalAdministrators
        {
            get { return this._additionalAdmins; }
            set { this._additionalAdmins = value; }
        }

        [DataMember(Name = "template")]
        public string Template { get; set; }

        [DataMember(Name = "sitePolicy")]
        public string SitePolicy { get; set; }

        [DataMember(Name = "sharePointOnPremises")]
        public bool SharePointOnPremises { get; set; }

        [DataMember(Name = "businessCase")]
        public string BusinessCase { get; set; }

        [DataMember(Name = "properties")]
        public Dictionary<string,string> Properties { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}