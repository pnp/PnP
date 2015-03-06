using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core
{
    /// <summary>
    /// Constant Class
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Class for Property Bag Entries
        /// </summary>
        public static class PropertyBags
        {
            /// <summary>
            /// Property Bag key for Branding Verision
            /// </summary>
            public const string BRANDING_VERSION = "_pnp_sp_branding_version";
            /// <summary>
            /// Property Bag key for branding theme
            /// </summary>
            public const string BRANDING_THEME_NAME = "_pnp_sp_branding_themename";

            /// <summary>
            /// Property Bag key for site custom template type
            /// </summary>
            public const string SITE_TEMPLATE_TYPE = "_pnp_sp_site_template";
        }
    }
}
