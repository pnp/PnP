using System;
using System.Collections.Generic;
using Microsoft.Online.SharePoint.TenantAdministration;
using OfficeDevPnP.Core.Entities;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// This manager class holds security related methods
    /// </summary>
    public static partial class SecurityExtensions
    {
#if !CLIENTSDKV15
        /// <summary>
        /// Adds additional administrators to a site collection using the Tenant administration csom. See AddAdministrators for a method
        /// that does not have a dependency on the Tenant administration csom.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="adminLogins">Array of logins for the additional admins</param>
        /// <param name="siteUrl">Url of the site to operate on</param>
        [Obsolete("Use Tenant.AddAdministrators() extension method")]
        public static void AddAdministratorsTenant(this Web web, String[] adminLogins, Uri siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);

            tenant.AddAdministrators(adminLogins, siteUrl);
        }

        /// <summary>
        /// Add a site collection administrator to a site collection
        /// </summary>
        /// <param name="web">Site to operate on</param>
        /// <param name="adminLogins">Array of admins loginnames to add</param>
        /// <param name="siteUrl">Url of the site to operate on</param>
        /// <param name="addToOwnersGroup">Optionally the added admins can also be added to the Site owners group</param>
        [Obsolete("Use Tenant.AddAdministrator() extension method")]
        public static void AddAdministratorsTenant(this Web web, IEnumerable<UserEntity> adminLogins, Uri siteUrl, bool addToOwnersGroup = false)
        {
            Tenant tenant = new Tenant(web.Context);

            tenant.AddAdministrators(adminLogins, siteUrl, addToOwnersGroup);

        }
#endif
    }
}
