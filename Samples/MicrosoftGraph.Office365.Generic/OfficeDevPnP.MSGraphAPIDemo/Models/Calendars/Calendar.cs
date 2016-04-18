using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a user's calendar
    /// </summary>
    public class Calendar : BaseModel
    {
        /// <summary>
        /// The color of the calendar
        /// </summary>
        public String Color { get; set; }

        /// <summary>
        /// The name of the calendar
        /// </summary>
        public String Name { get; set; }
    }
}