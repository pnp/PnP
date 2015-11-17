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

    }
}