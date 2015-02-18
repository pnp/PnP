using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Hybrid.Simple.Common
{
    public class Consts
    {
        /// <summary>
        /// Storage queue name for the process
        /// </summary>
        public const string StorageQueueName = "sitecollectionrequest";

        /// <summary>
        /// App.config or web.config app settings key for service bus namespace
        /// </summary>
        public const string ServiceBusNamespaceKey = "ServiceBus.Namespace";

        /// <summary>
        /// App.config or web.config app settings key for service bus namespace
        /// </summary>
        public const string ServiceBusSecretKey = "ServiceBus.Secret";

        /// <summary>
        /// Deployment type switch
        /// </summary>
        public const string DeploymentTypeCloud = "cloud";

        /// <summary>
        /// Deployment type switch
        /// </summary>
        public const string DeploymentTypeOnPremises = "onprem";

        /// <summary>
        /// Provisioning account used in on-premises
        /// </summary>
        public const string ProvisioningAccount = "ProvisioningAccount";

        /// <summary>
        /// Provisioning account used in on-premises
        /// </summary>
        public const string ProvisioningDomain = "ProvisioningDomain";

        /// <summary>
        /// Provisioning account password used in on-premises
        /// </summary>
        public const string ProvisioningPassword = "ProvisioningPassword";

        /// <summary>
        /// Admin site collection in on-premises
        /// </summary>
        public const string AdminSiteCollectionUrl = "AdminSiteCollectionUrl";

        /// <summary>
        /// Prefix domain for the site collections in on-premises... http://dev.contoso.com... we extend then sub site collection URLs to that
        /// </summary>
        public const string LeadingURLForSiteCollections = "LeadingURLForSiteCollections";
    }
}
