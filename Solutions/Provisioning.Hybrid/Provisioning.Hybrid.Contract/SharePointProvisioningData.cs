using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Provisioning.Hybrid.Contract
{
    [DataContract]
    public class SharePointProvisioningData
    {
        /// <summary>
        /// The SPO url
        /// </summary>
        [DataMember]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// The site name
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The site title
        /// </summary>
        [DataMember]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The site description
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        [DataMember]
        public SharePointUser SiteOwner
        {
            get;
            set;
        }

        [DataMember]
        public SharePointUser[] Owners
        {
            get;
            set;
        }

        /// <summary>
        /// The site locale. See http://technet.microsoft.com/en-us/library/ff463597.aspx for a complete list of Lcid's
        /// </summary>
        [DataMember]
        public uint Lcid
        {
            get;
            set;
        }

        /// <summary>
        /// Site quota in MB
        /// </summary>
        [DataMember]
        public long StorageMaximumLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Site quota warning level in MB
        /// </summary>
        [DataMember]
        public long StorageWarningLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Site template being used
        /// </summary>
        [DataMember]
        public string Template
        {
            get;
            set;
        }

        /// <summary>
        /// Data classificaiton (LBI/MBI/HBI) for this site
        /// </summary>
        [DataMember]
        public string DataClassification
        {
            get;
            set;
        }

        /// <summary>
        /// TimeZoneID for the site. "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris" = 3 
        /// </summary>
        [DataMember]
        public int TimeZoneId
        {
            get;
            set;
        }

        /// <summary>
        /// The user code quota in points
        /// </summary>
        [DataMember]
        public double UserCodeMaximumLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The user code quota warning level in points
        /// </summary>
        [DataMember]
        public double UserCodeWarningLevel
        {
            get;
            set;
        }

    }
}
