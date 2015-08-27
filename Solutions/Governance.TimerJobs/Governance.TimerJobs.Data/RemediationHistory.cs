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
    /// RemediationHistory class describes how an SCA remediated a non-compliance issue of a site collection
    /// </summary>
    [Table("RemediationHistory")]
    public class RemediationHistory
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
        /// When the remediation was taken
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime ActionPerformedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Who resolved the non-compliance issue
        /// </summary>
        [StringLength(128)]
        public string ActionPerformedBy
        {
            get;
            set;
        }
        
        /// <summary>
        /// Detailed action description
        /// </summary>
        [StringLength(512)]
        public string ActionLog
        {
            get;
            set;
        }

        /// <summary>
        /// The back reference id of the incompliant site collection
        /// </summary>
        [Required]
        public int TargetSiteId
        {
            get;
            set;
        }
        
        /// <summary>
        /// The back reference object of the incompliant site collection
        /// </summary>
        public virtual SiteInformation TargetSite { get; set; }
    }
}
