using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GeoUserPreferredDataLocation
{
    /// <summary>
    /// Multi-Geo helper class
    /// </summary>
    public class MultiGeoManager
    {
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

        #region REST based
        /// <summary>
        /// Gets the personal site url for a given user
        /// </summary>
        /// <param name="userPrincipalName">User to retrieve personal site url for</param>
        /// <returns></returns>
        public string GetPersonalSiteForUser(string userPrincipalName)
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
        /// Gets the preferred data location for a given user
        /// </summary>
        /// <param name="userPrincipalName">User to get the preferred data location for</param>
        /// <returns>Preferred data location</returns>
        public string GetPreferredDataLocationForUser(string userPrincipalName)
        {
            // Obtain an access token
            var authenticationResult = Authenticate();

            string jsonString = Task.Run(async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    // GET https://graph.microsoft.com/beta/users/bert@a830edad9050849524e17052212.onmicrosoft.com

                    string requestUrl = String.Format("https://graph.microsoft.com/beta/users/{0}?$select=preferredDataLocation", userPrincipalName);
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
            return json["preferredDataLocation"].Value<string>();
        }

        /// <summary>
        /// Update the user's preferred data location
        /// </summary>
        /// <param name="userPrincipalName">User to update the preferred data location for</param>
        /// <param name="preferredDataLocation">Preferred data location to set</param>
        /// <returns>Return value of the update operation</returns>
        public string UpdatePreferredDataLocationForUser(string userPrincipalName, string preferredDataLocation)
        {
            // Obtain an access token
            var authenticationResult = Authenticate();

            string jsonString = Task.Run(async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    // PATCH https://graph.microsoft.com/beta/users/bert@a830edad9050849524e17052212.onmicrosoft.com

                    string requestUrl = String.Format("https://graph.microsoft.com/beta/users/{0}", userPrincipalName);
                    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl);
                    request.Headers.Add("accept", "application/json;odata.metadata=minimal");
                    request.Headers.Add("odata-version", "4.0");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                    request.Content = new StringContent($"{{\"preferredDataLocation\":\"{preferredDataLocation}\"}}", Encoding.UTF8, "application/json");

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
        /// Update the user's department
        /// </summary>
        /// <param name="userPrincipalName">User to update the department for</param>
        /// <param name="department">Department value to set</param>
        /// <returns>Return value of the update operation</returns>
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
        #endregion

        #region SDK based
        /// <summary>
        /// Shows the Microsoft Graph SDK way of getting user information
        /// </summary>
        /// <param name="userPrincipalName">UPN of the user to fetch</param>
        /// <returns>User object</returns>
        public User GetUser(string userPrincipalName, string fieldsToRetrieve)
        {
            var authenticationResult = Authenticate();
            User result = null;
            try
            {
                // Use a synchronous model to invoke the asynchronous process
                result = Task.Run(async () =>
                {
                    var graphClient = CreateGraphClient(authenticationResult.AccessToken);
                    // .Me does not work when not signed in a user: https://stackoverflow.com/questions/42502323/getting-request-resourcenotfound-error-when-retrieving-graphserviceclient-me-dat
                    //var user = await graphClient.Me.Request().GetAsync();

                    var user = await graphClient.Users[userPrincipalName].Request().Select(fieldsToRetrieve).GetAsync();
                    return user;
                }).GetAwaiter().GetResult();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine(ex.Error.Message);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Updates a user 
        /// </summary>
        /// <param name="user">User to update</param>
        /// <returns>Updated user object</returns>
        public User UpdateUser(User user)
        {
            var authenticationResult = Authenticate();
            User result = null;
            try
            {
                // Use a synchronous model to invoke the asynchronous process
                result = Task.Run(async () =>
                {
                    var graphClient = CreateGraphClient(authenticationResult.AccessToken);
                    var updatedUser = await graphClient.Users[user.Id].Request().UpdateAsync(new User
                    {
                        Department = user.Department            
                    });
                    return updatedUser;
                }).GetAwaiter().GetResult();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine(ex.Error.Message);
                throw;
            }
            return result;
        }
        private GraphServiceClient CreateGraphClient(String accessToken)
        {
            var result = new GraphServiceClient(new DelegateAuthenticationProvider(
                        async (requestMessage) =>
                        {
                            if (!String.IsNullOrEmpty(accessToken))
                            {
                                // Configure the HTTP bearer Authorization Header
                                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                            }
                        }));

            return (result);
        }
        #endregion

        #region Helper methods
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
