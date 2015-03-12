using Provisioning.Hybrid.Simple.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Hybrid.Simple.Common
{
    [ServiceContract]
    public interface ISiteRequest
    {
        /// <summary>
        /// Actual operation to request site collections to be created.
        /// </summary>
        /// <param name="request">Complex data type for providing needed information for site collection creation</param>
        /// <returns>URL to the site which was created</returns>
        [OperationContract]
        string ProvisionSiteCollection(SiteCollectionRequest request);

        /// <summary>
        /// Test message for sending messages cross cloud and on-premises. Can be used to verify the connectivity. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract]
        string SendMessage(string message);
    }
    public interface ISharePointProvisioningChannel : ISiteRequest, IClientChannel { }
}
