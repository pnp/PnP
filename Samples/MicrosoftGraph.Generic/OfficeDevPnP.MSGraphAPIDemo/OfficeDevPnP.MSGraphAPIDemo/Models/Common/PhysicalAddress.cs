using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a physical address
    /// </summary>
    public class PhysicalAddress
    {
        /// <summary>
        /// The Street part of the address
        /// </summary>
        public String Street { get; set; }

        /// <summary>
        /// The City part of the address
        /// </summary>
        public String City { get; set; }

        /// <summary>
        /// The State part of the address
        /// </summary>
        public String State { get; set; }

        /// <summary>
        /// The Country or Region part of the address
        /// </summary>
        public String CountryOrRegion { get; set; }

        /// <summary>
        /// The Postal Code part of the address
        /// </summary>
        public String PostalCode { get; set; }
    }
}