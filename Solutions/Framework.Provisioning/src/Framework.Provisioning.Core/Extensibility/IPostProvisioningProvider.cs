using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Extensibility
{
    /// <summary>
    /// Interface that is required to implement a custom provider. 
    /// The custom provider is away for the Provisioning Engine to call out to the extensibilty pipeline to customize the site
    /// collection.
    /// </summary>
    public interface IPostProvisioningProvider : ISharePointService
    {
        /// <summary>
        /// Member method that the Post Provisoning Pipeline will invoke.
        /// </summary>
        /// <param name="request"></param>
        void ProcessRequest(SiteRequestInformation request, string configuration);
    }
}
