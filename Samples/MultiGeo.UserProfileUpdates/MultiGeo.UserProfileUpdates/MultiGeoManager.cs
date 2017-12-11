using Microsoft.Identity.Client;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeoUserDiscovery
{

    /// <summary>
    /// Multi-Geo helper class
    /// </summary>
    public class MultiGeoManager
    {
        private ClientContext clientContext = null;
        private string accessToken = null;
        private bool securityInitialized = false;
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
        /// <param name="clientContextForDefaultGeo">Client context object to clone for using CSOM against the geo locations</param>
        /// <param name="appId">ID of the Azure AD application</param>
        /// <param name="appPassword">Password defined for this Azure AD application</param>
        /// <param name="aadDomain">Domain of the Multi-Geo tenant</param>
        public MultiGeoManager(ClientContext clientContextForDefaultGeo, string appId, string appPassword, string aadDomain)
        {
            this.clientContext = clientContextForDefaultGeo;
            this.appPassword = appPassword;
            this.appId = appId;
            this.aadDomain = aadDomain;

            // Telemetry, we would like to understand how popular this sample is so we can target future investment. Obviously you're free to drop this section from the code 
            clientContextForDefaultGeo.ClientTag = "SPDev:MultiGeo";
            clientContextForDefaultGeo.Load(clientContextForDefaultGeo.Web, p => p.Description, p => p.Id);
            clientContextForDefaultGeo.ExecuteQuery();
        }

        /// <summary>
        /// Get personal site host value for a given user using CSOM
        /// </summary>
        /// <param name="userPrincipalName">User to retrieve personal site host for</param>
        /// <returns>Personal site host value for the given user</returns>
        public string GetUserPersonalSiteHostUrlCSOM(string userPrincipalName)
        {
            string result = null;

            PeopleManager peopleManager = new PeopleManager(this.clientContext);
            var userProperties = peopleManager.GetPropertiesFor($"i:0#.f|membership|{userPrincipalName}");
            this.clientContext.Load(userProperties);
            this.clientContext.ExecuteQuery();
            result = userProperties.PersonalSiteHostUrl;

            return result;
        }

        /// <summary>
        /// Get personal site host value for a given user using REST
        /// </summary>
        /// <param name="userPrincipalName">User to retrieve personal site host for</param>
        /// <returns>Personal site host value for the given user</returns>
        public string GetUserPersonalSiteHostUrlREST(string userPrincipalName)
        {
            if (!this.securityInitialized)
            {
                this.InitializeSecurity();
            }

            try
            {
                // Make REST call
                Task<String> personalSiteHostUrl = Task.WhenAny(GetUserPersonalSiteHostUrlAsync(this.accessToken, this.clientContext, userPrincipalName)).Result;

                // Parse JSON
                var json = JObject.Parse(personalSiteHostUrl.Result);
                return json["value"].ToString();
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get personal site location using Microsoft Graph
        /// </summary>
        /// <param name="userPrincipalName">User to get the personal site location for</param>
        /// <returns>Url of the user's personal site</returns>
        public string GetUserPersonalSiteLocation(string userPrincipalName)
        {
            // Obtain an access token
            var authenticationResult = Authenticate();

            string jsonString = Task.Run(async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    // GET https://graph.microsoft.com/v1.0/users/bert@a830edad9050849524e17052212.onmicrosoft.com?$select=mySite

                    string requestUrl = String.Format("https://graph.microsoft.com/v1.0/users/{0}?$select=mySite", userPrincipalName);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    request.Headers.Add("accept", "application/json;odata.metadata=minimal");
                    request.Headers.Add("odata-version", "4.0");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

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
            return json["mySite"].Value<string>();
        }

        /// <summary>
        /// Gets the department of a given user
        /// </summary>
        /// <param name="userPrincipalName">User to get the department for</param>
        /// <returns>Department of the user</returns>
        public string GetDepartmentForUser(string userPrincipalName)
        {
            // Obtain an access token
            var authenticationResult = Authenticate();

            string jsonString = Task.Run(async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    // GET https://graph.microsoft.com/v1.0/users/bert@a830edad9050849524e17052212.onmicrosoft.com?$select=department

                    string requestUrl = String.Format("https://graph.microsoft.com/v1.0/users/{0}?$select=department", userPrincipalName);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    request.Headers.Add("accept", "application/json;odata.metadata=minimal");
                    request.Headers.Add("odata-version", "4.0");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

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
            return json["department"].Value<string>();
        }

        /// <summary>
        /// Updates the department value for a given user
        /// </summary>
        /// <param name="userPrincipalName">User to update</param>
        /// <param name="department">Updated department value</param>
        /// <returns>Update response</returns>
        public string UpdateDepartmentForUser(string userPrincipalName, string department)
        {
            // Obtain an access token
            var authenticationResult = Authenticate();

            string jsonString = Task.Run(async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    // PATCH https://graph.microsoft.com/v1.0/users/bert@a830edad9050849524e17052212.onmicrosoft.com

                    string requestUrl = String.Format("https://graph.microsoft.com/v1.0/users/{0}", userPrincipalName);
                    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl);
                    request.Headers.Add("accept", "application/json;odata.metadata=minimal");
                    request.Headers.Add("odata-version", "4.0");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                    request.Content = new StringContent($"{{\"department\":\"{department}\"}}", Encoding.UTF8, "application/json");

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

            return jsonString;
        }

        /// <summary>
        /// Get's the tenant admin site for a given data location
        /// </summary>
        /// <param name="preferredDataLocation">Data location to get tenant admin url for</param>
        /// <returns>Tenant admin url</returns>
        public string GetTenantAdminSiteForPreferredDataLocation(string preferredDataLocation)
        {
            // Get all geo's as we want to gather sites across geo's ==> this is an expensive call and typically the results can be cached
            var geo = this.GetTenantGeoLocations().Where(p => p.GeoLocation == preferredDataLocation).FirstOrDefault();

            if (geo != null)
            {
                return geo.TenantAdminUrl;
            }
            else
            {
                throw new Exception($"Could not retrieve tenant admin url for data location {preferredDataLocation}");
            }
        }

        /// <summary>
        /// Get's the tenant admin site for a given site collection 
        /// </summary>
        /// <param name="siteUrl">Site collection to get the tenant admin url for</param>
        /// <returns>Tenant admin url</returns>
        public string GetTenantAdminSiteForSite(string siteUrl)
        {
            var siteHost = new Uri(siteUrl).DnsSafeHost;
            
            // Get all geo's as we want to gather sites across geo's ==> this is an expensive call and typically the results can be cached
            var geos = this.GetTenantGeoLocations();

            foreach(var geo in geos)
            {
                var geoRootSiteDomain = new Uri(geo.RootSiteUrl).DnsSafeHost;
                var geoMySiteHost = new Uri(geo.MySiteHostUrl).DnsSafeHost;

                if (geoRootSiteDomain.Equals(siteHost, StringComparison.InvariantCultureIgnoreCase) ||
                    geoMySiteHost.Equals(siteHost, StringComparison.InvariantCultureIgnoreCase) )
                {
                    return geo.TenantAdminUrl;
                }
            }

            throw new Exception($"No tenant admin url found for {siteUrl}");
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

        #region private methods
        /// <summary>
        /// Issues an async REST call to obtain a user's personal site host
        /// </summary>
        /// <param name="accessToken">Access token used to query SharePoint</param>
        /// <param name="context">Context used to query SharePoint</param>
        /// <param name="userPrincipalName">User to retrieve the personal site host for</param>
        /// <returns>Personal site host for user</returns>
        private async Task<string> GetUserPersonalSiteHostUrlAsync(string accessToken, ClientContext context, string userPrincipalName)
        {
            string responseString = null;

            using (var handler = new HttpClientHandler())
            {
                // we're not in app-only or user + app context, so let's fall back to cookie based auth
                if (String.IsNullOrEmpty(accessToken))
                {
                    handler.Credentials = context.Credentials;
                    handler.CookieContainer.SetCookies(new Uri(context.Web.Url), (context.Credentials as SharePointOnlineCredentials).GetAuthenticationCookie(new Uri(context.Web.Url)));
                }

                using (var httpClient = new HttpClient(handler))
                {
                    //GET https://contoso.sharepoint.com/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(accountName=@v)/personalsitehosturl?%40v=%27i%3A0%23.f%7Cmembership%7Cbert%40contoso.onmicrosoft.com%27 HTTP/1.1

                    userPrincipalName = WebUtility.UrlEncode(userPrincipalName);
                    string requestUrl = $"{context.Web.Url}/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(accountName=@v)/personalsitehosturl?%40v=%27i%3A0%23.f%7Cmembership%7C{userPrincipalName}%27";
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    request.Headers.Add("accept", "application/json;odata.metadata=minimal");
                    request.Headers.Add("odata-version", "4.0");

                    // We've an access token, so we're in app-only or user + app context
                    if (!String.IsNullOrEmpty(accessToken))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }

                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        string errorString = await response.Content.ReadAsStringAsync();
                        throw new Exception(errorString);
                    }
                }
                return await Task.Run(() => responseString);
            }
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

        private void InitializeSecurity()
        {
            // Let's try to grab an access token, will work when we're in app-only or user+app model
            this.clientContext.ExecutingWebRequest += Context_ExecutingWebRequest;
            this.clientContext.Load(this.clientContext.Web, w => w.Url);
            this.clientContext.ExecuteQuery();
            this.clientContext.ExecutingWebRequest -= Context_ExecutingWebRequest;
        }

        private void Context_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.WebRequestExecutor.RequestHeaders.Get("Authorization")))
            {
                this.accessToken = e.WebRequestExecutor.RequestHeaders.Get("Authorization").Replace("Bearer ", "");
            }
        }

        /// <summary>
        /// Clones a ClientContext object while "taking over" the security context of the existing ClientContext instance
        /// </summary>
        /// <param name="clientContext">ClientContext to be cloned</param>
        /// <param name="siteUrl">Site url to be used for cloned ClientContext</param>
        /// <returns>A ClientContext object created for the passed site url</returns>
        private ClientContext Clone(ClientRuntimeContext clientContext, Uri siteUrl)
        {
            if (siteUrl == null)
            {
                throw new ArgumentException($"Please provide a site url");
            }

            ClientContext clonedClientContext = new ClientContext(siteUrl);
            clonedClientContext.AuthenticationMode = clientContext.AuthenticationMode;
            clonedClientContext.ClientTag = clientContext.ClientTag;
            clonedClientContext.DisableReturnValueCache = clientContext.DisableReturnValueCache;

            // In case of using networkcredentials in on premises or SharePointOnlineCredentials in Office 365
            if (clientContext.Credentials != null)
            {
                clonedClientContext.Credentials = clientContext.Credentials;
            }
            else
            {
                //Take over the form digest handling setting
                clonedClientContext.FormDigestHandlingEnabled = (clientContext as ClientContext).FormDigestHandlingEnabled;

                // In case of app only or SAML
                clonedClientContext.ExecutingWebRequest += delegate (object oSender, WebRequestEventArgs webRequestEventArgs)
                {
                    // Call the ExecutingWebRequest delegate method from the original ClientContext object, but pass along the webRequestEventArgs of 
                    // the new delegate method
                    MethodInfo methodInfo = clientContext.GetType().GetMethod("OnExecutingWebRequest", BindingFlags.Instance | BindingFlags.NonPublic);
                    object[] parametersArray = new object[] { webRequestEventArgs };
                    methodInfo.Invoke(clientContext, parametersArray);
                };
            }

            return clonedClientContext;
        }

        private bool WaitForIsComplete(Tenant tenant, SpoOperation op)
        {
            bool succeeded = true;
            while (!op.IsComplete)
            {
                Thread.Sleep(op.PollingInterval);

                op.RefreshLoad();
                if (!op.IsComplete)
                {
                    try
                    {
                        tenant.Context.ExecuteQuery();
                    }
                    catch (WebException)
                    {
                        // Context connection gets closed after action completed.
                        // Calling ExecuteQuery again returns an error which can be ignored                        
                    }
                }
            }
            return succeeded;
        }
        #endregion
    }
}
