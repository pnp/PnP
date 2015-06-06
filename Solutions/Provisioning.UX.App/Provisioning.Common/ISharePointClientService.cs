using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common
{
    /// <summary>
    /// Interface used to implement Services that use SharePoint CSOM
    /// </summary>
    public interface ISharePointClientService
    {
        /// <summary>
        /// Delegate that is used by the implementation class for working with ClientContext Object
        /// </summary>
        /// <param name="action"></param>
        void UsingContext(Action<ClientContext> action);

        /// <summary>
        /// Delegate that is used by the implementation class for working with the ClientContext Object
        /// <param name="action"></param>
        /// <param name="csomTimeout"></param>
        void UsingContext(Action<ClientContext> action, int csomTimeout);
    }
}
