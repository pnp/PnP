using Newtonsoft.Json;
using Provisioning.Common;
using Provisioning.UX.AppWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Provisioning.UX.AppWeb
{
    public class ObjectMapper
    {
        public static SiteInformation ToSiteRequestInformation(SiteRequest request)
        {
            var _owner = new SiteUser()
            {
                Name = request.PrimaryOwner
            };

            List<SiteUser> _additionalAdmins = new List<SiteUser>();
            foreach (var secondaryOwner in request.AdditionalAdministrators)
            {
                if(!string.IsNullOrEmpty(secondaryOwner))
                {
                    var _sharePointUser = new SiteUser();
                    _sharePointUser.Name = secondaryOwner;
                    _additionalAdmins.Add(_sharePointUser);
                }
            }

            var _newRequest = new SiteInformation();
            _newRequest.Title = request.Title;
            _newRequest.Description = request.Description;
            _newRequest.Url = request.Url;
            _newRequest.TimeZoneId = request.TimeZoneID;
            _newRequest.Lcid = request.lcid;
            _newRequest.Template = request.Template;
            _newRequest.SitePolicy = request.SitePolicy;
            _newRequest.SiteOwner = _owner;
            _newRequest.AdditionalAdministrators = _additionalAdmins;
            _newRequest.SharePointOnPremises = request.SharePointOnPremises;
            _newRequest.BusinessCase = request.BusinessCase;
            _newRequest.EnableExternalSharing = request.EnableExternalSharing;


            if(request.Properties != null)
            {
                //Serialize Property Bag Entries
                _newRequest.SiteMetadataJson = JsonConvert.SerializeObject(request.Properties);
            }
            return _newRequest;
        }

        public static SiteEditMetadata ToSiteEditMetadata(SiteMetadata request)
        {
            var _newRequest = new SiteEditMetadata();            

            var _owner = new SiteUser()
            {
                Name = request.PrimaryOwnerName, 
                Email = request.PrimaryOwnerEmail               
            };            
            
            _newRequest.Url = request.Url;
            _newRequest.TenantAdminUrl = request.TenantAdminUrl;
            _newRequest.Title = request.Title;
            _newRequest.Description = request.Description;
            _newRequest.TimeZoneId = request.TimeZoneID;
            _newRequest.Lcid = request.lcid;
            _newRequest.AppliedSitePolicyName = request.SitePolicyName;
            _newRequest.SiteOwner = _owner;
            _newRequest.SitePolicy = request.SitePolicy; ;
            _newRequest.SharePointOnPremises = request.SharePointOnPremises;            
            _newRequest.EnableExternalSharing = request.EnableExternalSharing;
            _newRequest.BusinessUnit = request.BusinessUnit;
            _newRequest.Division = request.Division;
            _newRequest.Function = request.Function;
            _newRequest.Region = request.Region;

           
            return _newRequest;
        }

        public static SiteMetadata ToSiteMetadata(SiteEditMetadata request)
        {
            var _newRequest = new SiteMetadata();

            _newRequest.PrimaryOwnerEmail = request.SiteOwner.Email;
            _newRequest.PrimaryOwnerName = request.SiteOwner.Name;            

            _newRequest.Url = request.Url;
            _newRequest.TenantAdminUrl = request.TenantAdminUrl;
            _newRequest.Title = request.Title;
            _newRequest.Description = request.Description;
            _newRequest.TimeZoneID = request.TimeZoneId;
            _newRequest.lcid = request.Lcid;
            _newRequest.SitePolicy = request.SitePolicy;
            _newRequest.SitePolicyName = request.AppliedSitePolicyName;
            _newRequest.SitePolicyExpirationDate = request.AppliedSitePolicyExpirationDate;           
            _newRequest.SharePointOnPremises = request.SharePointOnPremises;
            _newRequest.EnableExternalSharing = request.EnableExternalSharing;
            _newRequest.BusinessUnit = request.BusinessUnit;
            _newRequest.Division = request.Division;
            _newRequest.Function = request.Function;
            _newRequest.Region = request.Region;
            _newRequest.Success = true;
            
            return _newRequest;
        }

    }
}