using Core.SiteClassification.Common.impl;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SiteClassification.Common
{
    public class SiteClassificationFactory : ISiteClassificationFactory
    {
        #region Private instance members
        private static readonly SiteClassificationFactory _instance = new SiteClassificationFactory();
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor to handle beforefieldinit
        /// </summary>
        static SiteClassificationFactory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        SiteClassificationFactory()
        {
        }
        #endregion

        #region Public Memmbers
        public static ISiteClassificationFactory GetInstance()
        {
            return _instance;
        }
        #endregion

        public ISiteClassificationManager GetManager(ClientContext ctx)
        {
            var _manager = new SiteClassificationImpl();
            _manager.Initialize(ctx);
            return _manager;
        }
    }
}
