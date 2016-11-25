using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a Date and Time information
    /// </summary>
    public class TimeInfo
    {
        /// <summary>
        /// The date and time
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The TimeZone of the time
        /// </summary>
        public String TimeZone { get; set; }

    }
}