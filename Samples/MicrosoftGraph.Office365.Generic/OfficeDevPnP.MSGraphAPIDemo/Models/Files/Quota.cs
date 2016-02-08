using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines the Quota of a drive in OneDrive for Business
    /// </summary>
    public class Quota
    {
        /// <summary>
        /// Space allocated by the recycle bin
        /// </summary>
        public Int64 Deleted { get; set; }

        /// <summary>
        /// Space available
        /// </summary>
        public Int64 Remaining { get; set; }

        /// <summary>
        /// Current state of the storage quota
        /// </summary>
        public String State { get; set; }

        /// <summary>
        /// Total allocated space for storage
        /// </summary>
        public Int64 Total { get; set; }

        /// <summary>
        /// Total used space of storage
        /// </summary>
        public Int64 Used { get; set; }
    }
}