using Provisioning.Common.Configuration.Template;
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
    /// Domain Object for working with SharePoint Site Requests
    /// </summary>
    [DataContract]
    public class SiteRequestInformation
    {
        #region instance members
        private int _timeZoneId = 13;
        private uint _lcid = 1033;
        private List<SharePointUser> _additionalAdmins = new List<SharePointUser>();
        private bool _externalSharingEnabled = false;
        #endregion

        #region Properties
        /// <summary>
        /// The Site Collection URL
        /// </summary>
        [DataMember]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Collection Title
        /// </summary>
        [DataMember]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Collection Description
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Template
        /// </summary>
        [DataMember]
        public string Template
        {
            get;
            set;
        }

        [DataMember]
        /// <summary>
        /// The Owner of the Site Collection
        /// </summary>
        public SharePointUser SiteOwner
        {
            get;
            set;
        }

        /// <summary>
        /// Addition Site Administrators
        /// </summary>
        [XmlArray(ElementName = "AdditionalAdministrators")]
        [XmlArrayItem("SharePointUser", typeof(SharePointUser))]
        public List<SharePointUser> AdditionalAdministrators
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Policy to Apply
        /// </summary>
        [DataMember]
        public string SitePolicy
        {
            get;
            set;
        }

        /// <summary>
        /// A string that represents the status of the request
        /// </summary>
        public string RequestStatus
        {
            get { return EnumStatus.ToString(); }
            set { EnumStatus = (SiteRequestStatus)Enum.Parse(typeof(SiteRequestStatus), value); }
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
        public bool EnableExternalSharing
        {
            get { return this._externalSharingEnabled; }
            set { this._externalSharingEnabled = value; }
        }

       
        #endregion
    }
}
