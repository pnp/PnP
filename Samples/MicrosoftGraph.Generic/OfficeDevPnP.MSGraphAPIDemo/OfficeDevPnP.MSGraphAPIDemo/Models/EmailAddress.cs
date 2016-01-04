using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines an email address
    /// </summary>
    public class EmailAddress
    {
        /// <summary>
        /// The email address
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// The description of the email address
        /// </summary>
        public String Name { get; set; }
    }
}