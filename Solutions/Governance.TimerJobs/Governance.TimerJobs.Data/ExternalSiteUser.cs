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
    /// ExternalSiteUser represents an external user invited to the current SharePoint tenant
    /// </summary>
    [Table("SiteUsers")]
    public class ExternalSiteUser : SiteUser
    {
        /// <summary>
        /// SharePoint external user unique id
        /// </summary>
        [StringLength(128)]
        public string ExternalUser_UniqueId
        {
            get;
            set;
        }

        /// <summary>
        /// The Microsoft Account or the associated account used to login SharePoint
        /// </summary>
        [StringLength(128)]
        public string ExternalUser_AcceptedAs
        {
            get;
            set;
        }

        /// <summary>
        /// The display name of the AcceptedAs account
        /// </summary>
        [StringLength(128)]
        public string ExternalUser_DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// The email address used to sent external user invitation email
        /// </summary>
        [StringLength(128)]
        public string ExternalUser_InvitedAs
        {
            get;
            set;
        }

        /// <summary>
        /// The user who invited this external user
        /// </summary>
        [StringLength(128)]
        public string ExternalUser_InvitedBy
        {
            get;
            set;
        }

        /// <summary>
        /// When was the external user firstly created in the tenant
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? ExternalUser_CreatedDate
        {
            get;
            set;
        }
    }
}
