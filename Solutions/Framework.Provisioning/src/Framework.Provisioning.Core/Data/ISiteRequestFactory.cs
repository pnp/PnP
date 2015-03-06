using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Data
{

    public interface ISiteRequestFactory
    {
        /// <summary>
        /// Returns an interface for working with the Site Request Repository
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Framework.Provisioning.Core.Data.DataStoreException">Exception that occurs when interacting with the Site Request Repository</exception>
        ISiteRequestManager GetSiteRequestManager();
    }
      

}
