using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OfficeDevPnP.Core.Entities;

namespace Provisioning.Common
{
    /// <summary>
    /// Domain Object for working with Site Metadata
    /// </summary>
    [DataContract]
    public class SiteEditMetadata
    {
        #region instance members
        private int _timeZoneId = 13;
        private uint _lcid = 1033;
        private bool _externalSharingEnabled = false;
        private bool _sharePointOnPrem = false;
        private string _businessUnit;
        private string _region;
        private string _function;
        private string _division;
        private bool _success;
        private string _errorMessage;
        private bool _tenantSharingEnabled;
        private bool _siteSharingEnabled;
        #endregion

        #region Properties
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }
     
        /// <summary>
        /// The Site Collection URL
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "url")]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// The Tenant Admin URL
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "tenantAdminUrl")]
        public string TenantAdminUrl
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Title
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "title")]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Description
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get;
            set;
        }
       
        [DataMember]
        /// <summary>
        /// The Owner of the Site
        /// </summary>
        [JsonProperty(PropertyName = "siteOwner")]
        public SiteUser SiteOwner
        {
            get;
            set;
        }
               
        /// <summary>
        /// The Site Policy to Apply
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "sitePolicy")]
        public string SitePolicy
        {
            get;
            set;
        }       

        /// <summary>
        /// The site locale. See http://technet.microsoft.com/en-us/library/ff463597.aspx for a complete list of Lcid's
        /// The default value is 1033 for en-us
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "lcid")]
        public uint Lcid
        {
            get
            {
                return this._lcid;
            }
            set
            {
                this._lcid = value;
            }
        }

        /// <summary>
        /// Specifies the time zone of the site collection. For more information, see SPRegionalSettings.TimeZones Property (http://go.microsoft.com/fwlink/p/?LinkId=242912).
        /// The default value is 13 (GMT-08:00) Pacific Time (US and Canada)
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "timeZoneId")]
        public int TimeZoneId
        {
            get
            {
                return this._timeZoneId;
            }

            set
            {
                this._timeZoneId = value;
            }
        }

        /// <summary>
        /// Specifies the Business Unit set for the site collection        
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "businessUnit")]
        public string BusinessUnit
        {
            get
            {
                return this._businessUnit;
            }
            set
            {
                this._businessUnit = value;
            }
        }

        /// <summary>
        /// Specifies the Region set for the site collection        
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "region")]
        public string Region
        {
            get
            {
                return this._region;
            }
            set
            {
                this._region = value;
            }
        }

        /// <summary>
        /// Specifies the (business) Function set for the site collection        
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "function")]
        public string Function
        {
            get
            {
                return this._function;
            }
            set
            {
                this._function = value;
            }
        }

        /// <summary>
        /// Specifies the (business) Division set for the site collection        
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "division")]
        public string Division
        {
            get
            {
                return this._division;
            }
            set
            {
                this._division = value;
            }
        }

        /// <summary>
        /// Indicates if External Sharing should be enabled. This option is not available in on-premises build.
        /// Default value is false
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "enableExternalSharing")]
        public bool EnableExternalSharing
        {
            get { return this._externalSharingEnabled; }
            set { this._externalSharingEnabled = value; }
        }

        /// <summary>
        /// Indicates if External Sharing should be enabled. This option is not available in on-premises build.
        /// Default value is false
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "tenantSharingEnabled")]
        public bool TenantSharingEnabled
        {
            get { return this._tenantSharingEnabled; }
            set { this._tenantSharingEnabled = value; }
        }

        /// <summary>
        /// Indicates if External Sharing should be enabled. This option is not available in on-premises build.
        /// Default value is false
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "siteSharingEnabled")]
        public bool SiteSharingEnabled
        {
            get { return this._siteSharingEnabled; }
            set { this._siteSharingEnabled = value; }
        }

        /// <summary>
        /// Indicates if the Site Request is targeting on-premises builds of SharePoint
        /// Default value is false
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "sharePointOnPremises")]
        public bool SharePointOnPremises
        {
            get { return this._sharePointOnPrem; }
            set { this._sharePointOnPrem = value; }
        }

        /// <summary>
        /// Site Policies
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "appliedSitePolicy")]
        public SitePolicyEntity AppliedSitePolicy
        {
            get;
            set;
        }

        /// <summary>
        /// Site Policy Expiration Date
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "appliedSitePolicyExpirationDate")]
        public string AppliedSitePolicyExpirationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Site Policy Name
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "appliedSitePolicyName")]
        public string AppliedSitePolicyName
        {
            get;
            set;
        }

        /// <summary>
        /// Site Policies
        /// </summary>
        [XmlArray(ElementName = "SitePolicies")]
        [XmlArrayItem("SitePolicy", typeof(SitePolicyEntity))]
        [JsonProperty(PropertyName = "sitePolicies")]
        public List<SitePolicyEntity> SitePolicies
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if the Site Request is targeting on-premises builds of SharePoint
        /// Default value is false
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "success")]
        public bool Success
        {
            get { return this._success; }
            set { this._success = value; }
        }

        /// <summary>
        /// Indicates if External Sharing should be enabled. This option is not available in on-premises build.
        /// Default value is false
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage
        {
            get { return this._errorMessage; }
            set { this._errorMessage = value; }
        }
        #endregion

    }
}
