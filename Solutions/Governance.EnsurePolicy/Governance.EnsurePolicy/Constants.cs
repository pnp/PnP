using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Governance.EnsurePolicy
{
    public static class Constants
    {
        // App config settings
        internal const string AppSettings_AppId = "AppId";
        internal const string AppSettings_AzureTenant = "AzureTenant";
        internal const string AppSettings_PfxCertificate = "PfxCertificate";
        internal const string AppSettings_PfxCertificatePassword = "PfxCertificatePassword";
        internal const string AppSettings_TenantAdmin = "TenantAdmin";
        internal const string AppSettings_ExcludeOD4BSites = "ExcludeOD4BSites";
        internal const string AppSettings_NumberOfThreads = "NumberOfThreads";
        internal const string AppSettings_SiteFilters = "SiteFilters";
    }
}
