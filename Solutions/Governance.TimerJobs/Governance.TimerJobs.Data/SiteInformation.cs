using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Governance.TimerJobs.Data
{
    /// <summary>
    /// SiteInformation class describes the status of a SharePoint site collection, where the base class properties are representing its root site.
    /// </summary>
    [Table("SiteInformation")]
    public class SiteInformation : WebInformation
    {
        #region Governance Columns

        /// <summary>
        /// The targeting audience scope of the site collection. e.g. Enterprise, Organization, Team, Project
        /// </summary>
        [StringLength(128)]
        public string AudienceScope
        {
            get;
            set;
        }

        /// <summary>
        /// The governance workflow related status of the site collection
        /// </summary>
        public ComplianceState ComplianceState
        {
            get;
            set;
        }

        /// <summary>
        /// The additional site collection metadata
        /// </summary>
        public ICollection<SiteMetadata> SiteMetadata
        {
            get;
            set;
        }

        /// <summary>
        /// The site collection non-compliance remediation history records
        /// </summary>
        public ICollection<RemediationHistory> RemediationHistory
        {
            get;
            set;
        }

        /// <summary>
        /// Should we exceptionally skip governance the site collection
        /// </summary>
        public bool IsSkipGovernance
        {
            get;
            set;
        }

        #endregion

        #region SharePoint Columns

        /// <summary>
        /// SharePoint site collection GUID
        /// </summary>
        public Guid Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Site collection locale id 
        /// </summary>
        public int Lcid
        {
            get;
            set;
        }

        /// <summary>
        /// Site collection tempalte (STS#0, etc)
        /// </summary>
        [StringLength(128)]
        public string Template
        {
            get;
            set;
        }

        /// <summary>
        /// Site collection time zone id
        /// </summary>
        public int TimeZoneId
        {
            get;
            set;
        }

        /// <summary>
        /// The max storage bytes of the site collection
        /// </summary>
        public long StorageMaximumLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The warning storage bytes of the site collection
        /// </summary>
        public long StorageWarningLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The max resources of the site collection
        /// </summary>
        public double UserCodeMaximumLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The warning resources of the site collection
        /// </summary>
        public double UserCodeWarningLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The site collection administrators of the site collection
        /// </summary>
        public ICollection<SiteUser> Administrators
        {
            get;
            set;
        }

        /// <summary>
        /// The external sharing status of the site collection
        /// 0 - Disabled
        /// 1 - ExternalUserSharingOnly
        /// 2 - ExternalUserAndGuestSharing
        /// </summary>
        public int? SharingStatus
        {
            get;
            set;
        }

        /// <summary>
        /// If the currrent site collection cab be shared with external users
        /// </summary>
        [NotMapped]
        public bool IsExternalSharingEnabled
        {
            get
            {
                if (SharingStatus.HasValue)
                {
                    if (SharingStatus != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            private set
            {
            }
        }

        /// <summary>
        /// The external users if the site collection
        /// </summary>
        public ICollection<SiteUser> ExternalUsers
        {
            get;
            set;
        }

        #endregion

    }
}
