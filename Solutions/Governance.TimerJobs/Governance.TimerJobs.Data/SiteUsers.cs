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
    /// SiteUser represents a SharePoint user
    /// </summary>
    [Table("SiteUsers")]
    public class SiteUser
    {      
        /// <summary>
        /// Auto-increased database id
        /// </summary>
        [Key]
        public int Id
        {
            get;
            set;
        }
        
        /// <summary>
        /// The login name of the user
        /// </summary>
        [Index(IsUnique=true)]
        [StringLength(128)]
        public string LoginName
        {
            get;
            set;
        }
        
        /// <summary>
        /// The display name of the user
        /// </summary>
        [NotMapped]
        public string DisplayName
        {
            get;
            set;
        }
        
        /// <summary>
        /// User email address
        /// </summary>
        [StringLength(128)]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Email address of the user's direct manager
        /// </summary>
        [NotMapped]
        public string ManagerEmail
        {
            get;
            set;
        }
        
        /// <summary>
        /// If the current user can be resolved in UPA
        /// </summary>
        [NotMapped]
        public bool IsResolved
        {
            get;
            set;
        }        
    }
}
