using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Contoso.Provisioning.Hybrid.Contract;


namespace Contoso.Provisioning.Hybrid.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISharePointProvisioningService" in both code and config file together.
    [ServiceContract]
    public interface ISharePointProvisioningService
    {
        /// <summary>
        /// Requests the provisioning of a SharePoint site collection
        /// </summary>
        /// <param name="sharePointProvisioningData">Information needed to provision the SharePoint site collection</param>
        /// <returns>true if queued, false otherwise</returns>
        [OperationContract]
        bool ProvisionSiteCollection(SharePointProvisioningData sharePointProvisioningData);
    }
}
