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
    /// WebInformation class describes the status of a SharePoint site
    /// </summary>
    [Table("SiteInformation")]
    public class WebInformation
    {
        #region SQL Columns
        
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
        /// SharePoint site creation date
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate
        {
            get;
            set;
        }
        
        /// <summary>
        /// Database record last modified date
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Service name or user login name of the record creator
        /// </summary>
        [StringLength(128)]
        public string CreatedBy
        {
            get;
            set;
        }

        /// <summary>
        /// Service name or user login name of whom last modified this record
        /// </summary>
        [StringLength(128)]
        public string ModifiedBy
        {
            get;
            set;
        }

        #endregion

        #region SharePoint Columns

        /// <summary>
        /// SharePoint site title
        /// </summary>
        [StringLength(256)]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// SharePoint site description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// SharePoint site domain name
        /// </summary>
        [StringLength(128)]
        public string UrlDomain
        {
            get;
            set;
        }
        
        /// <summary>
        /// SharePoint site URL managed path
        /// </summary>
        [StringLength(128)]
        public string UrlPath
        {
            get;
            set;
        }

        /// <summary>
        /// SharePoint site name
        /// </summary>
        [StringLength(512)]
        public string Name
        {
            get;
            set;

        }

        /// <summary>
        /// SharePoint site URL
        /// </summary>
        [NotMapped]
        public virtual string Url
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}{1}{2}", UrlDomain, UrlPath, Name);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Url");
                var url = new Uri(value);
                UrlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);
                if (UrlDomain.Length == value.Length)
                {
                    UrlPath = "/";
                    Name = string.Empty;
                    return;
                }
                var pathAndQuery = url.OriginalString.Substring(UrlDomain.Length);
                int idx = pathAndQuery.Substring(1).IndexOf("/") + 2;
                UrlPath = pathAndQuery.Substring(0, idx);
                Name = pathAndQuery.Substring(idx);
            }
        }

        /// <summary>
        /// The root site url of the current SharePoint site
        /// </summary>
        [NotMapped]
        public string SiteUrl
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}{1}{2}", UrlDomain, UrlPath, Name.Split("/".ToCharArray())[0]);
            }
        }

        #endregion

        #region Governance Columns

        /// <summary>
        /// The business impact of the current site
        /// </summary>
        [StringLength(128)]
        public string BusinessImpact
        {
            get;
            set;
        }

        /// <summary>
        /// The last business impact of the current site
        /// </summary>
        [StringLength(128)]
        public string LastBusinessImpact
        {
            get;
            set;
        }

        public bool HasBroadAccess
        {
            get;
            set;
        }

        public string BroadAccessGroups
        {
            get;
            set;
        }
        
        #endregion
    }
}
