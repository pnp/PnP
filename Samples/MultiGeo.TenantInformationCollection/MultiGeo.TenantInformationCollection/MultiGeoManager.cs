using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GeoTenantInformationCollection
{
    /// <summary>
    /// Multi-Geo helper class
    /// </summary>
    public class MultiGeoManager
    {
        private List<GeoProperties> geosCache = null;
        private string appPassword;
        private string appId;
        private string aadDomain;
        private readonly Uri AADLogin = new Uri("https://login.microsoftonline.com/");
        private readonly string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
        private readonly string[] DefaultScope = { "https://graph.microsoft.com/.default" };

        /// <summary>
        /// Constructs the multi geo manager
        /// </summary>
        /// <param name="appId">ID of the Azure AD application</param>
        /// <param name="appPassword">Password defined for this Azure AD application</param>
        /// <param name="aadDomain">Domain of the Multi-Geo tenant</param>
        public MultiGeoManager(string appId, string appPassword, string aadDomain)
        {
            this.appPassword = appPassword;
            this.appId = appId;
            this.aadDomain = aadDomain;
        }

        /// <summary>
        /// Return the geo locations from the tenant linked to the Azure AD hosting the defined Azure AD application
        /// </summary>
        /// <returns>List of geo locations in this tenant</returns>
        public List<GeoProperties> GetTenantGeoLocations()
        {
            // Return data from cache...geos are fairly stable :-)
            if (this.geosCache != null)
            {
                return this.geosCache;
            }

            // Obtain an access token
            string accessTokenToUse = Authenticate().AccessToken;

            string jsonString = Task.Run(async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    // GET https://graph.microsoft.com/v1.0/sites?filter=siteCollection/root%20ne%20null&select=webUrl,siteCollection

                    string requestUrl = "https://graph.microsoft.com/v1.0/sites?filter=siteCollection/root%20ne%20null&select=webUrl,siteCollection";
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    request.Headers.Add("accept", "application/json;odata.metadata=minimal");
                    request.Headers.Add("odata-version", "4.0");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenToUse);

                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    string responseString = null;
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        string errorString = await response.Content.ReadAsStringAsync();
                        throw new Exception(errorString);
                    }

                    return responseString;
                }
            }).GetAwaiter().GetResult();

            var json = JObject.Parse(jsonString);
            List<GeoProperties> geoList = new List<GeoProperties>(json["value"].Count());
            foreach (var geo in json["value"])
            {
                string rootSiteUrl = geo["webUrl"].Value<string>();
                string rootSiteHost = geo["siteCollection"]["hostname"].Value<string>();

                // dataLocationCode was not yet returned in earlier versions
                string dataLocationCode = GetJTokenValue<string>(geo["siteCollection"], "dataLocationCode", "");

                geoList.Add(new GeoProperties()
                {
                    GeoLocation = dataLocationCode, 
                    RootSiteUrl = rootSiteUrl,
                    MySiteHostUrl = GetSPOUrl(rootSiteHost, false, true),
                    TenantAdminUrl = GetSPOUrl(rootSiteHost, true, false),
                });
            }

            // cache data as geos are fairly stable
            this.geosCache = geoList;

            return geoList;
        }

        #region Helper methods
        /// <summary>
        /// Return the requested value from a JToken
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="jToken">JToken to obtain value from</param>
        /// <param name="key">Key to get from JToken</param>
        /// <param name="defaultValue">Return value if key was not found</param>
        /// <returns>Requested key value from JToken</returns>
        private static T GetJTokenValue<T>(JToken jToken, string key, T defaultValue = default(T))
        {
            dynamic ret = jToken[key];
            if (ret == null) return defaultValue;
            if (ret is JObject) return JsonConvert.DeserializeObject<T>(ret.ToString());
            return (T)ret;
        }

        /// <summary>
        /// Deducts the tenant admin and personal site root site urls from the received root site domain
        /// </summary>
        /// <param name="rootSiteHost">Root site domain to investigate</param>
        /// <param name="isAdmin">Return the tenant admin url</param>
        /// <param name="isMy">Return the personal site host url</param>
        /// <returns>The requested url</returns>
        private string GetSPOUrl(string rootSiteHost, bool isAdmin, bool isMy)
        {
            string url = "";
            string[] hostParts = rootSiteHost.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            if (isAdmin)
            {
                url = $"{hostParts[0]}-admin";
            }
            else if (isMy)
            {
                url = $"{hostParts[0]}-my";
            }

            for (int i = 1; i < hostParts.Length; i++)
            {
                url = url + "." + hostParts[i];
            }

            return string.Format("https://{0}/", url);
        }

        /// <summary>
        /// Authentication is done using the preview version of the Microsoft Identity Client (Microsoft Authentication Library or MSAL). 
        /// See https://developer.microsoft.com/en-us/graph/docs/concepts/auth_overview and https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-v2-libraries
        /// </summary>
        /// <returns>Object holding information about the authentication flow</returns>
        private AuthenticationResult Authenticate()
        {
            var appCredentials = new ClientCredential(this.appPassword);
            var authority = new Uri(this.AADLogin, this.aadDomain).AbsoluteUri;
            var clientApplication = new ConfidentialClientApplication(this.appId, authority, this.RedirectUri, appCredentials, new TokenCache(), new TokenCache());
            AuthenticationResult authenticationResult = clientApplication.AcquireTokenForClientAsync(DefaultScope).GetAwaiter().GetResult();
            return authenticationResult;
        }
        #endregion
    }
}
