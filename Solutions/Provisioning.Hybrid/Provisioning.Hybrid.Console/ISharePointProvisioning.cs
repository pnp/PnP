using Contoso.Provisioning.Hybrid.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Contoso.Provisioning.Hybrid
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISharePointProvisioning" in both code and config file together.
    [ServiceContract]
    public interface ISharePointProvisioning
    {
        [OperationContract]
        bool ProvisionSiteCollection(SharePointProvisioningData sharePointProvisioningData);

    }

    public interface ISharePointProvisioningChannel : ISharePointProvisioning, IClientChannel { }
}
