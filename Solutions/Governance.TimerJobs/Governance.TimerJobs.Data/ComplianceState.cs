using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Governance.TimerJobs.Data
{
    /// <summary>
    /// The governance workflow related status of the site collection
    /// </summary>
    [ComplexType]
    public class ComplianceState
    {
        /// <summary>
        /// When the first lock notification was sent
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime FirstLockNotificationSentDate { get; set; }

        /// <summary>
        /// When the second lock notification was sent
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime SecondLockNotificationSentDate { get; set; }

        /// <summary>
        /// When the delete notification was sent
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime DeleteNotificationSentDate { get; set; }

        /// <summary>
        /// If the site is complaint with all governance policies
        /// </summary>
        public bool IsCompliant { get; set; }

        /// <summary>
        /// When the site collection will be locked
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime LockedDate { get; set; }

        /// <summary>
        /// When the site collection will be expired according to the lifecycle policy
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// When the site collection will be deleted
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime DeleteDate { get; set; }

        /// <summary>
        /// When was the governance job last check the site collection status
        /// </summary>
        [Column(TypeName = "datetime2")]
        public System.DateTime LastCheckDate
        {
            get;
            set;
        }

        /// <summary>
        /// When was the external users membership last reviewed
        /// </summary>
        [Column(TypeName = "datetime2")]
        public System.DateTime LastMembershipReviewDate
        {
            get;
            set;
        }
                
        /// <summary>
        /// If the first lock notification has been sent during the current decommission cycle
        /// </summary>
        public bool FirstLockNotificationSent
        {
            get;
            set;
        }

        /// <summary>
        /// If the second lock notification has been sent during the current decommission cycle
        /// </summary>
        public bool SecondLockNotificationSent
        {
            get;
            set;
        }

        /// <summary>
        /// If delete notification has been sent during the current decommission cycle
        /// </summary>
        public bool DeleteNotificationSent
        {
            get;
            set;
        }

        /// <summary>
        /// If the site collection is locked up
        /// </summary>
        public bool IsLocked
        {
            get;
            set;
        }

        /// <summary>
        /// If the site collection is readonly
        /// </summary>
        public bool IsReadonly
        {
            get;
            set;
        }
    }
}
