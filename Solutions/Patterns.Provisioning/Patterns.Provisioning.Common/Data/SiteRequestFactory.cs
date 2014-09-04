using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Patterns.Provisioning.Common.Data;
using Patterns.Provisioning.Common.Util;
using Patterns.Provisioning.Common.Data.Impl;

namespace Patterns.Provisioning.Common.Data
{
    /// <summary>
    /// Manager Class for working with the Site Request Repository
    /// </summary>
    public sealed class SiteRequestFactory : ISiteRequestFactory
    {
        #region Private Instance Members
        private static readonly SiteRequestFactory _instance = new SiteRequestFactory();
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor to handle beforefieldinit
        /// </summary>
        static SiteRequestFactory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        SiteRequestFactory()
        {
        }
        #endregion

        /// <summary>
        /// Used to return an interface for working with the Site Request Repository
        /// </summary>
        /// <returns>Patterns.Provisioning.Common.Data.ISiteRequestFactory</returns>
        public static ISiteRequestFactory GetInstance()
        {
            return _instance;
        }

        public ISiteRequestManager GetSPSiteRepository(ClientContext ctx, string listName)
        {
            ArgumentHelper.RequireNotNullOrEmpty(listName, "listName");
            ArgumentHelper.RequireObjectNotNull(ctx, "ctx");

            var _iRepository = new SpSiteSource();

            try
            {
                _iRepository.Initialize(ctx, listName);
            }
            catch(DataStoreException)
            {
                throw;
            }
            catch(Exception _ex)
            {
                throw new DataStoreException(_ex.Message, _ex);
            }

            return _iRepository;
        }

        public ISiteRequestManager GetDbSiteRepository(string connectionString)
        {
            ArgumentHelper.RequireNotNullOrEmpty(connectionString, "connectionString");
            throw new NotImplementedException("Not Implmentented in this release.");
        }

    }
}
