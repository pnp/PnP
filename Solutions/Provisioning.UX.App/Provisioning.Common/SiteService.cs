using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using OfficeDevPnP.Core.Enums;

namespace Provisioning.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="site"></param>
        /// <param name="name"></param>
        public void AddUserVisitorGroup(ClientContext ctx, Site site, string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                //TODO LOG
            }
        }

        public void Add(ClientContext ctx, Site site, BuiltInIdentity identity)
        {

        }
    }
}
