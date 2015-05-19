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
            this.HandleCustomActions(provisioningTemplate, siteRequest);
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

        /// <summary>
        /// Member to handle the Url of custom actions
        /// </summary>
        /// <param name="provisioningTemplate"></param>
        /// <param name="siteRequest"></param>
        private void HandleCustomActions(ProvisioningTemplate provisioningTemplate, SiteRequestInformation siteRequest)
        {
            if (provisioningTemplate.CustomActions != null)
            {
                //handle site custom actions
                foreach (var _siteCustomActions in provisioningTemplate.CustomActions.SiteCustomActions)
                {
                    //IF ITS A SCRIPT SRC WE DO NOT WANT TO MODIFY
                    if (!string.IsNullOrEmpty(_siteCustomActions.Url))
                    {
                        var _escapedURI = Uri.EscapeUriString(siteRequest.Url);
                        _siteCustomActions.Url = string.Format(_siteCustomActions.Url, _escapedURI);
                    }
                }
                //handle web custom actions
                foreach( var _webActions in provisioningTemplate.CustomActions.WebCustomActions)
                {
                    //IF ITS A SCRIPT SRC WE DO NOT WANT TO MODIFY
                    if (!string.IsNullOrEmpty(_webActions.Url))
                    {
                         var _escapedURI = Uri.EscapeUriString(siteRequest.Url);
                        _webActions.Url = string.Format(_webActions.Url, _escapedURI);
                    }
                }
            }
        }
    }
}
