using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Contoso.Provisioning.SiteCollectionCreationWeb.Models
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
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Collection Description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Template
        /// </summary>
        public string Template
        {
            get;
            set;
        }

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
        public ICollection<SharePointUser> AdditionalAdministrators
        {
            get;
            set;
        }

        /// <summary>
        /// The Site Policy to Apply
        /// </summary>
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
        public Enum EnumStatus
        {
            get;
            set;
        }

        /// <summary>
        /// The site locale. See http://technet.microsoft.com/en-us/library/ff463597.aspx for a complete list of Lcid's
        /// The default value is 1033 for en-us
        /// </summary>
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
        #endregion
    }

    [DataContract]
    public class SharePointUser
    {
        [DataMember]
        public string Login
        {
            get;
            set;
        }
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        [DataMember]
        public string Email
        {
            get;
            set;
        }
    }

    public enum SiteRequestStatus
    {
        Complete,
        Exception,
        New,
        Processing,
        Pending,
        Approved
    }
}