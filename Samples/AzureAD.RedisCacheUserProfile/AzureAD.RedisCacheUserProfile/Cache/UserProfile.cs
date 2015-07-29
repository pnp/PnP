
using AzureAD.RedisCacheUserProfile.Utils;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;

namespace AzureAD.RedisCacheUserProfile.Cache
{
    public class UserProfile
    {
        public static Models.UserProfile GetUserProfile(string name)
        {

            IDatabase cache = CacheConnectionHelper.Connection.GetDatabase();
            Models.UserProfile userProfile = (Models.UserProfile)cache.Get(name);
            if (userProfile == null)
            {
                #region Get User Profile from AD
                Uri serviceRoot = new Uri(SettingsHelper.AzureAdGraphApiEndPoint);
                var token = AppToken.GetAppToken();

                ActiveDirectoryClient adClient = new ActiveDirectoryClient(
                 serviceRoot,
                 async () => await AppToken.GetAppTokenAsync());

                string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

                Microsoft.Azure.ActiveDirectory.GraphClient.Application app = (Microsoft.Azure.ActiveDirectory.GraphClient.Application)adClient.Applications.Where(
                    a => a.AppId == SettingsHelper.ClientId).ExecuteSingleAsync().Result;
                if (app == null)
                {
                    throw new ApplicationException("Unable to get a reference to application in Azure AD.");
                }

                string requestUrl = string.Format("https://graph.windows.net/{0}/users/{1}?api-version=1.5", SettingsHelper.Tenant, name);
                HttpClient hc = new HttpClient();
                hc.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage hrm = hc.GetAsync(new Uri(requestUrl)).Result;

                if (hrm.IsSuccessStatusCode)
                {
                    Models.UserProfile currentUserProfile = JsonConvert.DeserializeObject<Models.UserProfile>(hrm.Content.ReadAsStringAsync().Result);
                    cache.Set(ClaimsPrincipal.Current.Identities.First().Name, currentUserProfile, TimeSpan.FromMinutes(SettingsHelper.CacheUserProfileMinutes));
               
                    return currentUserProfile;
                }
                else
                {
                    return null;
                }
                #endregion
            }
            else
            {
                return userProfile;
            }
        }

    }
}