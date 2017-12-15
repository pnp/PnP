using Microsoft.Identity.Client;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RestrictSiteToGeo
{

    /// <summary>
    /// Multi-Geo helper class
    /// </summary>
    public class MultiGeoManager
    {
        private ClientContext clientContext = null;
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
        /// Get the currently set site move restriction
        /// </summary>
        /// <param name="siteUrl">Site to check</param>
        /// <returns>Current site move restriction</returns>
        public RestrictedToRegion GetSiteGeoRestriction(string siteUrl)
        {
            using (var ctx = Clone(this.clientContext, new Uri(GetTenantAdminSiteForSite(siteUrl))))
            {
                var tenant = new Tenant(ctx);
                var siteProperties = tenant.GetSitePropertiesByUrl(siteUrl, true);
                tenant.Context.Load(siteProperties);
                tenant.Context.ExecuteQuery();

                if (siteProperties != null)
                {
                    return siteProperties.RestrictedToRegion;
                }
                else
                {
                    throw new Exception($"Site {siteUrl} was not found.");
                }                
            }
        }

        /// <summary>
        /// Set's the site move restriction for a site
        /// </summary>
        /// <param name="siteUrl">Site to set</param>
        /// <param name="restriction">Site move restriction value to apply</param>
        /// <param name="wait">Wait for completion of this operation</param>
        public void SetSiteGeoRestriction(string siteUrl, RestrictedToRegion restriction, bool wait = true)
        {
            using (var ctx = Clone(this.clientContext, new Uri(GetTenantAdminSiteForSite(siteUrl))))
            {
                var tenant = new Tenant(ctx);
                var siteProperties = tenant.GetSitePropertiesByUrl(siteUrl, true);
                tenant.Context.Load(siteProperties);
                tenant.Context.ExecuteQuery();

                if (siteProperties != null)
                {
                    siteProperties.RestrictedToRegion = restriction;
                    var op = siteProperties.Update();
                    tenant.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
                    tenant.Context.ExecuteQuery();
                    if (wait)
                    {
                        WaitForIsComplete(tenant, op);
                    }
                }
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

        /// <summary>
        /// Generic waiting method for async Tenant API methods
        /// </summary>
        /// <param name="tenant">Tenant instance</param>
        /// <param name="op">Wait object</param>
        /// <returns>Status of the waited operation</returns>
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

    }
}
