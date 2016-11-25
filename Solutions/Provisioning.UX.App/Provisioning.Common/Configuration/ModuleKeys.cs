using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration
{
    /// <summary>
    /// Constants for working Module Keys.
    /// Module Keys are defined in your config file.
    /// </summary>
    public sealed class ModuleKeys
    {
        public static readonly string REPOSITORYMANGER_KEY = "RepositoryManager";
        public static readonly string MASTERTEMPLATEPROVIDER_KEY = "MasterTemplateProvider";
        public static readonly string PROVISIONINGCONNECTORS_KEY = "ProvisioningConnectors";
        public static readonly string PROVISIONINGPROVIDER_KEY = "ProvisioningProviders";
        public static readonly string METADATAMANGER_KEY = "MetadataManager";
        public static readonly string APPSETTINGSMANAGER_KEY = "AppSettingsManager";
        public static readonly string SITEEDITMANAGER_KEY = "SiteEditManager";
    }
}
