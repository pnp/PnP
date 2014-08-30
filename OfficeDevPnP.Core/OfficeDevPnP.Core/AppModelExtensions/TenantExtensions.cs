using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    public static class TenantExtensions
    {
        /// <summary>
        /// Sets tenant site Properties
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="siteFullUrl"></param>
        /// <param name="title"></param>
        /// <param name="allowSelfServiceUpgrade"></param>
        /// <param name="sharingCapability"></param>
        /// <param name="storageMaximumLevel"></param>
        /// <param name="storageWarningLevel"></param>
        /// <param name="userCodeMaximumLevel"></param>
        /// <param name="userCodeWarningLevel"></param>
        public static void SetSiteProperties(this Tenant tenant, string siteFullUrl,
            string title = null,
            Nullable<bool> allowSelfServiceUpgrade = null,
            Nullable<SharingCapabilities> sharingCapability = null,
            Nullable<long> storageMaximumLevel = null,
            Nullable<long> storageWarningLevel = null,
            Nullable<double> userCodeMaximumLevel = null,
            Nullable<double> userCodeWarningLevel = null
            )
        {
            var siteProps = tenant.GetSitePropertiesByUrl(siteFullUrl, true);
            tenant.Context.Load(siteProps);
            tenant.Context.ExecuteQuery();
            if (siteProps != null)
            {
                if (allowSelfServiceUpgrade != null)
                    siteProps.AllowSelfServiceUpgrade = allowSelfServiceUpgrade.Value;
                if (sharingCapability != null)
                    siteProps.SharingCapability = sharingCapability.Value;
                if (storageMaximumLevel != null)
                    siteProps.StorageMaximumLevel = storageMaximumLevel.Value;
                if (storageWarningLevel != null)
                    siteProps.StorageWarningLevel = storageMaximumLevel.Value;
                if (userCodeMaximumLevel != null)
                    siteProps.UserCodeMaximumLevel = userCodeMaximumLevel.Value;
                if (userCodeWarningLevel != null)
                    siteProps.UserCodeWarningLevel = userCodeWarningLevel.Value;
                if (title != null)
                    siteProps.Title = title;

                siteProps.Update();
                tenant.Context.ExecuteQuery();
            }
        }
    }
}
