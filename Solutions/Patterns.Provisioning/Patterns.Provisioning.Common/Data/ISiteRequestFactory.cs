using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Provisioning.Common.Data
{
    public interface ISiteRequestFactory
    {
        /// <summary>
        /// Returns an Inteface for working with the Site Request Repository
        /// </summary>
        /// <param name="ctx">An Authenicated SharePoint ClientContext</param>
        /// <param name="listName">The name of the SharePoint List that is used as the repository.</param>
        /// <returns>Patterns.Provisioning.Common.Data.ISiteRequestManager</returns>
        /// <exception cref="Patterns.Provisioning.Common.Data.DataStoreException">Exception that occurs when interacting with the Site Request Repository</exception>
        ISiteRequestManager GetSPSiteRepository(ClientContext ctx, string listName);
        
        /// <summary>
        /// Returns an Inteface for working with the Site Request Repository
        /// </summary>
        /// <param name="connectionString">The Connection string to the DB repository</param>
        /// <returns>Patterns.Provisioning.Common.Data.ISiteRequestManager</returns>
        /// <exception cref="Patterns.Provisioning.Common.Data.DataStoreException">Exception that occurs when interacting with the Site Request Repository</exception>
        ISiteRequestManager GetDbSiteRepository(string connectionString);
    }
}
