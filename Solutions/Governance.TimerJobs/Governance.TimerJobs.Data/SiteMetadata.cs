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
    /// SiteMetadata class represent a custom metadata to be collected for a site collection
    /// </summary>
    [Table("SiteMetadata")]
    public class SiteMetadata
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
        /// The key (name) of the metadata field
        /// </summary>
        [StringLength(128)]
        public string MetadataKey
        {
            get;
            set;
        }

        /// <summary>
        /// The value of the metadata field
        /// </summary>
        [StringLength(256)]
        public string MetadataValue
        {
            get;
            set;
        }
        
        /// <summary>
        /// The back reference id of the attached site collection
        /// </summary>
        [Required]
        public int TargetSiteId
        {
            get;
            set;
        }

        /// <summary>
        /// The back reference id of the attached site collection
        /// </summary>
        public virtual SiteInformation TargetSite { get; set; }
    }
}
