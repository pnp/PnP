using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoUserDiscovery
{
    /// <summary>
    /// Class holding information over the returned geo location
    /// </summary>
    public class GeoProperties
    {
        /// <summary>
        /// Geo location code, 3 letter code indicating the location (e.g. NAM, EUR, APC, CAN, GBR,...)
        /// </summary>
        public string GeoLocation { get; set; }
        /// <summary>
        /// Url of the root site collection (e.g. https://contosoeur.sharepoint.com)
        /// </summary>
        public string RootSiteUrl { get; set; }
        /// <summary>
        /// Url of the personal site host (e.g. https://contosoeur-my.sharepoint.com)
        /// </summary>
        public string MySiteHostUrl { get; set; }
        /// <summary>
        /// Url of the tenant admin site (e.g. https://contosoeur-admin.sharepoint.com)
        /// </summary>
        public string TenantAdminUrl { get; set; }
    }
}
