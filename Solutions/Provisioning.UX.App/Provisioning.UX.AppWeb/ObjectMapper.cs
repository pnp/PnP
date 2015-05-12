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
        public static SiteRequestInformation ToSiteRequestInformation(SiteRequest request)
        {
            var _owner = new SharePointUser()
            {
                Email = request.PrimaryOwner
            };

            List<SharePointUser> _additionalAdmins = new List<SharePointUser>();

            foreach (var secondaryOwner in request.SecondaryOwners)
            {
                var _sharePointUser = new SharePointUser();
                _sharePointUser.Email = secondaryOwner;
                _additionalAdmins.Add(_sharePointUser);
            }

            var _newRequest = new SiteRequestInformation();
            _newRequest.Title = request.Title;
            _newRequest.Description = request.Description;
            _newRequest.Url = string.Format("{0}{1}", request.HostPath, request.Url);
            _newRequest.TimeZoneId = request.TimeZoneID;
            _newRequest.Lcid = request.LanguageID;
            _newRequest.Template = request.Template;
            _newRequest.SitePolicy = request.SitePolicy;
            _newRequest.SiteOwner = _owner;
            _newRequest.AdditionalAdministrators = _additionalAdmins;
            _newRequest.SharePointOnPremises = request.SharePointOnPremises;
            _newRequest.BusinessCase = request.BusinessCase;

            if(request.Properties != null)
            {
                //Serialize Property Bag Entries
                _newRequest.PropertiesJSON = JsonConvert.SerializeObject(request.Properties);
            }
    


            return _newRequest;
        }

    }
}