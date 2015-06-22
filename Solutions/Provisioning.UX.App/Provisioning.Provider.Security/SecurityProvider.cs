using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;

namespace Provisioning.Provider.Security
{
    public class SecurityProvider : IProvisioningExtensibilityProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="template"></param>
        /// <param name="configurationData"></param>
        public void ProcessRequest(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            var _sitePolicy = template.SitePolicy;
            if(string.IsNullOrEmpty(_sitePolicy))
            {
                this.AddAllUsersSecurity(ctx);
            }
        }

        private void AddAllUsersSecurity(ClientContext ctx)
        {
      
        }
    }
}
