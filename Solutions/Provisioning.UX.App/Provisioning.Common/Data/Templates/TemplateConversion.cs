using Newtonsoft.Json;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Templates
{
    /// <summary>
    /// Internal class for Handling Tempalte Conversions
    /// </summary>
    internal class TemplateConversion
    {
        internal ProvisioningTemplate HandleProvisioningTemplate(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest, Template template)
        {
            this.HandleExternalSharing(provisioningTemplate, siteRequest);
            this.HandleSitePolicy(provisioningTemplate, siteRequest, template);
            this.HandleAdditionalAdministrators(provisioningTemplate, siteRequest);
            this.HandlePropertyBagEntries(provisioningTemplate, siteRequest);
            this.HandleCustomActions(provisioningTemplate, siteRequest);
            this.HandleParameters(provisioningTemplate, siteRequest);
            return provisioningTemplate;
        }
      
        private void HandleExternalSharing(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest)
        {
            //EXTERNAL SHARING CUSTOM ACTION MUST BE DEFINED IN TEMPLATE. IF THE SITE REQUEST DOES NOT HAVE EXTERNAL SHARING ENABLE WE WILL REMOVE THE CUSTOM ACCTION
            if(!siteRequest.EnableExternalSharing)
            {
                if(provisioningTemplate.CustomActions != null)
                {
                    //FIND THE CUSTOM ACTION CA_SITE_EXTERNALSHARING
                    var _externalSharingCA = provisioningTemplate.CustomActions.SiteCustomActions.FirstOrDefault(t => t.Title == "CA_SITE_EXTERNALSHARING");
                    if(_externalSharingCA != null)
                    {
                        provisioningTemplate.CustomActions.SiteCustomActions.Remove(_externalSharingCA);
                    }
                }
            }
        }
        private void HandleSitePolicy(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest, Template template)
        {
            if (!template.UseTemplateDefinedPolicy)
            {
                if (!string.IsNullOrWhiteSpace(siteRequest.SitePolicy))
                {
                    provisioningTemplate.SitePolicy = siteRequest.SitePolicy;
                }
            }
        }
        private void HandleAdditionalAdministrators(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest)
        {
           foreach(var _Admin in siteRequest.AdditionalAdministrators)
           {
               User _user = new User();
               _user.Name = _Admin.Name;
               provisioningTemplate.Security.AdditionalAdministrators.Add(_user);
           }
        }
        private void HandlePropertyBagEntries(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest)
        {
            if (siteRequest.SiteMetadataJson != null)
            {
                Dictionary<string, string> _props = JsonConvert.DeserializeObject<Dictionary<string, string>>(siteRequest.SiteMetadataJson);
                if (_props != null)
                {
                    foreach (var prop in _props)
                    {
                        PropertyBagEntry _pb = new PropertyBagEntry();
                        _pb.Key = prop.Key;
                        _pb.Value = prop.Value;
                        provisioningTemplate.PropertyBagEntries.Add(_pb);
                    }
                }
            }
        }

        /// <summary>
        /// Member to handle the Url of custom actions
        /// </summary>
        /// <param name="provisioningTemplate"></param>
        /// <param name="siteRequest"></param>
        private void HandleCustomActions(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest)
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
        private void HandleParameters(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest)
        {
            if (siteRequest.SiteMetadataJson != null)
            {
                // Add dynamic properties
                Dictionary<string, string> _props = JsonConvert.DeserializeObject<Dictionary<string, string>>(siteRequest.SiteMetadataJson);
                if (_props != null)
                {
                    foreach (var prop in _props)
                    {
                        provisioningTemplate.Parameters.Add("pnp_" + prop.Key, prop.Value);
                    }
                }
            }

            // Add static properties
            provisioningTemplate.Parameters.Add("pnp_LCID", siteRequest.Lcid.ToString());
            provisioningTemplate.Parameters.Add("pnp_Title", siteRequest.Title);
            provisioningTemplate.Parameters.Add("pnp_SafeTitle", siteRequest.Title.UrlNameFromString());
            provisioningTemplate.Parameters.Add("pnp_Policy", siteRequest.SitePolicy);
            provisioningTemplate.Parameters.Add("pnp_ExternalSharing", siteRequest.EnableExternalSharing.ToString());
            provisioningTemplate.Parameters.Add("pnp_TemplateName", siteRequest.Template);
        }

    }
}
