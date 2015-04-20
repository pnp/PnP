using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Provisioning.Common
{
    /// <summary>
    /// Profile Service Implmentation Class
    /// This class is used to work Profiles. You can not call the profile api using api only permissions. In order to work the Profile API, 
    /// it requires the account access the Profile to have a user profile.
    /// This requires user account.
    /// </summary>
    public class ProfileService : ISharePointService
    {
        #region Instance Members
        const string LOGGING_SOURCE = "ProfileService";
        IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
        AppSettings _settings = null;
        #endregion

        #region Constructor
        public ProfileService()
        {
            IAppSettingsManager _appManager = _configFactory.GetAppSetingsManager();
            _settings = _appManager.GetAppSettings();
        }

        #endregion

        #region Properties
        /// <summary>
        /// TODO
        /// </summary>
        public IAuthentication Authentication
        {
            get;
            set;
        }
        #endregion

        public void UsingContext(Action<ClientContext> action)
        {
            this.UsingContext(action, Timeout.Infinite);
        }

        public void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext _ctx = Authentication.GetAuthenticatedContext())
            {
                _ctx.RequestTimeout = csomTimeout;
                action(_ctx);
            }
        }

        public IEnumerable<string> GetPropertiesForUser(string accountName, string[] properties)
        {
           IEnumerable<string> _results = Enumerable.Empty<string>();
           UsingContext(ctx =>
           {
               UserProfilePropertiesForUser _user = new UserProfilePropertiesForUser(ctx, accountName, properties);
               PeopleManager _manager = new PeopleManager(ctx);
               _results = _manager.GetUserProfilePropertiesFor(_user);
               ctx.ExecuteQuery();
           });

           return _results;
        }

        public IDictionary<string, string> GetAllPropertiesForUser(string accountName)
        {
            IDictionary<string,string> userPropResults = new Dictionary<string, string>();
            UsingContext(ctx =>
            {
                PeopleManager peopleManager = new PeopleManager(ctx);
                PersonProperties props = peopleManager.GetPropertiesFor(accountName);
                ctx.Load(props);
                ctx.ExecuteQuery();
                userPropResults = props.UserProfileProperties;
            });

            return userPropResults;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetUserPropertySingleValue(string accountName, string property, string value)
        {
            if (this._settings.SharePointOnPremises) throw new NotSupportedException("Only available for Office 365");
            UsingContext(ctx =>
            {
                PeopleManager peopleManager = new PeopleManager(ctx);
                peopleManager.SetSingleValueProfileProperty(accountName, property, value);
                ctx.ExecuteQuery();
            //    userPropResults = props.UserProfileProperties;

            });
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetUserProfilePropertyMultiValue(string accountName, string property, List<string> value)
        {
            if (this._settings.SharePointOnPremises) throw new NotSupportedException("Only available for Office 365");
            UsingContext(ctx =>
            {
                PeopleManager peopleManager = new PeopleManager(ctx);
                peopleManager.SetMultiValuedProfileProperty(accountName, property, value);
                ctx.ExecuteQuery();
           });
        }
    }
}
