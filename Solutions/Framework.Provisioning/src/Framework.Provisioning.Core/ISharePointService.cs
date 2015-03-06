using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core
{
    /// <summary>
    /// Interface used by to implement Services that use SharePoint
    /// </summary>
    public interface ISharePointService
    {
        /// <summary>
        /// Delegate that is used by the implementation class for working with 
        /// ClientContext Object
        /// </summary>
        /// <param name="action"></param>
        void UsingContext(Action<ClientContext> action);

        /// <summary>
        /// Delegate that is used by the implementation class for working with 
        /// ClientContext Object
        /// <param name="action"></param>
        /// <param name="csomTimeout"></param>
        void UsingContext(Action<ClientContext> action, int csomTimeout);
    }
}
