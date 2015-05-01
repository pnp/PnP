using Newtonsoft.Json;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Templates
{
    internal class TemplateConversion
    {
        internal ProvisioningTemplate HandleProvisioningTemplate(ProvisioningTemplate provisioningTemplate, SiteRequestInformation siteRequest)
        {
            this.HandleSitePolicy(provisioningTemplate, siteRequest);
            this.HandleSecurity(provisioningTemplate, siteRequest);
            this.HandlePropertyBagEntries(provisioningTemplate, siteRequest);
            return provisioningTemplate;
        }

        private void HandleSitePolicy(ProvisioningTemplate provisioningTemplate, SiteRequestInformation siteRequest)
        {
            if(!string.IsNullOrWhiteSpace(siteRequest.SitePolicy))
            {
                provisioningTemplate.SitePolicy = siteRequest.SitePolicy;
            }
        }

        private void HandleSecurity(ProvisioningTemplate provisioningTemplate, SiteRequestInformation siteRequest)
        {
           foreach(var _Admin in siteRequest.AdditionalAdministrators)
           {
               User _user = new User();
               _user.Name = _Admin.LoginName;
               provisioningTemplate.Security.AdditionalAdministrators.Add(_user);
           }
        }

        private void HandlePropertyBagEntries(ProvisioningTemplate provisioningTemplate, SiteRequestInformation siteRequest)
        {
            Dictionary<string, string> _props = JsonConvert.DeserializeObject<Dictionary<string, string>>(siteRequest.PropertiesJSON);

            foreach(var prop in _props)
            {
                PropertyBagEntry _pb = new PropertyBagEntry();
                _pb.Key = prop.Key;
                _pb.Value = prop.Value;
                provisioningTemplate.PropertyBagEntries.Add(_pb);
            }

   
        }
    }
}
