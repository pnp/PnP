using Contoso.Core.UserProfilePropertyUpdater.UserProfileASMX;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Core.UserProfilePropertyUpdater
{
    public class UserProfileManager
    {
        private const string upsServiceName = "/_vti_bin/UserProfileService.asmx";
        private UserProfileASMX.UserProfileService ups = null;


        #region properties

        /// <summary>
        /// URL of the tenant SharePoint admin center (https://tenantname-admin.sharepoint.com)
        /// </summary>
        public string TenantAdminUrl
        {
            get;
            set;
        }

        /// <summary>
        /// User that will be used to make the web service call
        /// </summary>
        public String User
        {
            get;
            set;
        }

        /// <summary>
        /// Password of the user used to make the service call
        /// </summary>
        public String Password
        {
            get;
            set;
        }

        /// <summary>
        /// Domain of the user used to make the service call
        /// </summary>
        public string Domain
        {
            get;
            set;
        }

        /// <summary>
        /// My site host url, needed for SharePoint 2013 on-premises or Office 365 Dedicated
        /// </summary>
        public string MySiteHost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an instance of the user profile ASMX service proxy
        /// </summary>
        private UserProfileASMX.UserProfileService UPS
        {
            get
            {
                if (ups == null)
                {
                    if (!String.IsNullOrEmpty(TenantAdminUrl))
                    {
                        this.ups = new UserProfileASMX.UserProfileService();
                        ups.Url = TenantAdminUrl + upsServiceName;
                        ups.UseDefaultCredentials = false;
                        ups.CookieContainer = new System.Net.CookieContainer();
                        ups.CookieContainer.Add(GetFedAuthCookie(CreateSharePointOnlineCredentials()));
                        return ups;
                    }
                    else if (this.User.Length > 0 && this.Password.Length > 0 && this.Domain.Length > 0 && this.MySiteHost.Length > 0)
                    {
                        this.ups = new UserProfileASMX.UserProfileService();
                        ups.Url = this.MySiteHost + upsServiceName;
                        NetworkCredential credential = new NetworkCredential(this.User, this.Password, this.Domain);
                        CredentialCache credentialCache = new CredentialCache();
                        credentialCache.Add(new Uri(this.MySiteHost), "NTLM", credential);
                        ups.Credentials = credentialCache;
                        return ups;
                    }
                    else
                    {
                        throw new Exception("Please specify an authentication provider or specify domain credentials");
                    }
                }
                else
                {
                    return this.ups;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the value (simple type) of a given property for a given user
        /// </summary>
        /// <param name="property">Name of the property</param>
        /// <param name="user">Login name of the user profile to read</param>
        /// <returns>The value of the returned property</returns>
        public T GetPropertyForUser<T>(string property, string user)
        {
            UserProfileASMX.PropertyData p = UPS.GetUserPropertyByAccountName(user, property);

            if (p.Values.Length > 0)
            {
                return (T)p.Values[0].Value;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gets the value (PropertyData type) of a given property for a given user
        /// </summary>
        /// <param name="property">Name of the property</param>
        /// <param name="user">Login name of the user profile to read</param>
        /// <returns>The value (PropertyData type) of the returned property</returns>
        public UserProfileASMX.PropertyData GetPropertyForUser(string property, string user)
        {
            return UPS.GetUserPropertyByAccountName(user, property);
        }

        /// <summary>
        /// Set a given property for a given user to the passed value (simple type)
        /// </summary>
        /// <typeparam name="T">Type of the passed property value</typeparam>
        /// <param name="property">Name of the property to update</param>
        /// <param name="propertyValue">Property value of type T</param>
        /// <param name="user">Login name of the user profile to update</param>
        public void SetPropertyForUser<T>(string property, T propertyValue, string user)
        {
            UserProfileASMX.PropertyData[] newdata = new UserProfileASMX.PropertyData[1];
            newdata[0] = new UserProfileASMX.PropertyData();
            newdata[0].Name = property;
            newdata[0].Values = new ValueData[1];
            newdata[0].Values[0] = new ValueData();
            newdata[0].Values[0].Value = propertyValue;
            newdata[0].IsValueChanged = true;
            ups.ModifyUserPropertyByAccountName(user, newdata);
        }

        /// <summary>
        /// Set a given property for a given user to the passed value (PropertyData type)
        /// </summary>
        /// <param name="property">Name of the property to update</param>
        /// <param name="propertyValue">Property value of type PropertyData</param>
        /// <param name="user">Login name of the user profile to update</param>
        public void SetPropertyForUser(string property, UserProfileASMX.PropertyData[] newData, string user)
        {
            ups.ModifyUserPropertyByAccountName(user, newData);
        }

        private SharePointOnlineCredentials CreateSharePointOnlineCredentials()
        {
            var spoPassword = new SecureString();
            foreach (char c in Password)
            {
                spoPassword.AppendChar(c);
            }
            return new SharePointOnlineCredentials(User, spoPassword);
        }

        private Cookie GetFedAuthCookie(SharePointOnlineCredentials credentials)
        {
            string authCookie = credentials.GetAuthenticationCookie(new Uri(this.TenantAdminUrl));
            if (authCookie.Length > 0)
            {
                return new Cookie("FedAuth", authCookie.TrimStart("SPOIDCRL=".ToCharArray()), String.Empty, new Uri(this.TenantAdminUrl).Authority);
            }
            else
            {
                return null;
            }
        }

        #endregion


    }
}
