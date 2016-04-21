using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Provisioning.Common
{
    /// <summary>
    /// Domain Object for working with Site Requests
    /// </summary>
    [DataContract]
    public class SiteInformation
    {
        #region instance members
        private int _timeZoneId = 13;
        private uint _lcid = 1033;
        private List<SiteUser> _additionalAdmins = new List<SiteUser>();
        private bool _externalSharingEnabled = false;
        private bool _sharePointOnPrem = false;
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

        /// <summary>
        /// The Site Template
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "template")]
        public string Template
        {
            get;
            set;
        }
        /// <summary>
        /// The Site Template
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "baseTemplate")]
        public string BaseTemplate
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
        /// Additional Site Administrators
        /// </summary>
        [XmlArray(ElementName = "AdditionalAdministrators")]
        [XmlArrayItem("SharePointUser", typeof(SiteUser))]
        [JsonProperty(PropertyName = "additionalAdministrators")]
        public List<SiteUser> AdditionalAdministrators
        {
            get { return _additionalAdmins; }
            set { _additionalAdmins = value; }
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

        [JsonProperty(PropertyName = "approvedDate")]
        public DateTime ApprovedDate
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "submitDate")]
        public DateTime SubmitDate
        {
            get;
            internal set;
        }

        /// <summary>
        /// A string that represents the status of the request
        /// </summary>
        [JsonProperty(PropertyName = "requestStatus")]
        public string RequestStatus
        {
            get { return EnumStatus.ToString(); }
            set { EnumStatus = (SiteRequestStatus)Enum.Parse(typeof(SiteRequestStatus), value); }
        }

        [JsonProperty(PropertyName = "requestStatusMessage")]
        public string RequestStatusMessage
        {
            get;
            set;
        }


        /// <summary>
        /// Status of the Request
        /// </summary>
        [XmlIgnore]
        public Enum EnumStatus
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
        /// Business Case of the site request;
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "businessCase")]
        public string BusinessCase { get; set; }
       
        /// <summary>
        /// Additional site Metadata stored in the repository as a JSON string
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "siteMetaData")]
        public string SiteMetadataJson { get; set; }
        #endregion
    }
}
