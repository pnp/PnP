using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Security.Claims;
using System.Web.Helpers;


namespace AzureAD.RedisCacheUserProfile.Utils
{
    public static class GraphUtil
    {
        /// <summary>
        /// For access check user's group membership must be determined. 
        /// This method retrieves user's group membership from Azure AD Graph API if not present in the token.
        /// </summary>
        /// <param name="claimsIdentity">The <see cref="ClaimsIdenity" /> object that represents the 
        /// claims-based identity of the currently signed in user and contains thier claims.</param>
        /// <returns>A list of ObjectIDs representing the groups that the user is member of.</returns>
        public static async Task<List<string>> GetMemberGroups(ClaimsIdentity claimsIdentity)
        {
            //check for groups overage claim. If present query graph API for group membership
            if (claimsIdentity.FindFirst("_claim_names") != null
                && (Json.Decode(claimsIdentity.FindFirst("_claim_names").Value)).groups != null)
                return await GetGroupsFromGraphAPI(claimsIdentity);

            return claimsIdentity.FindAll("groups").Select(c => c.Value).ToList();
        }

        /// <summary>
        /// In the case of Groups claim overage, we must query the GraphAPI to obtain the group membership.
        /// Here we use the GraphAPI Client Library to do so.
        /// </summary>
        /// <param name="claimsIdentity">The <see cref="ClaimsIdenity" /> object that represents the 
        /// claims-based identity of the currently signed in user and contains thier claims.</param>
        /// <returns>A list of ObjectIDs representing the groups that the user is member of.</returns>
        private static async Task<List<string>> GetGroupsFromGraphAPI(ClaimsIdentity claimsIdentity)
        {
            List<string> groupObjectIds = new List<string>();

            string tenantId = claimsIdentity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string signedInUserID = claimsIdentity.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
            string userObjectID = claimsIdentity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority, new EfAdalTokenCache(signedInUserID));
            AuthenticationResult result = authContext.AcquireTokenSilent(
                SettingsHelper.AzureAdGraphResourceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));

            // Get the GraphAPI Group Endpoint for the specific user from the _claim_sources claim in token
            string groupsClaimSourceIndex = (Json.Decode(claimsIdentity.FindFirst("_claim_names").Value)).groups;
            var groupClaimsSource = (Json.Decode(claimsIdentity.FindFirst("_claim_sources").Value))[groupsClaimSourceIndex];
            string requestUrl = groupClaimsSource.endpoint + "?api-version=" + SettingsHelper.GraphAPIVersion;

            // Prepare and Make the POST request
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            StringContent content = new StringContent("{\"securityEnabledOnly\": \"false\"}");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            // Endpoint returns JSON with an array of Group ObjectIDs
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var groupsResult = (Json.Decode(responseContent)).value;

                foreach (string groupObjectID in groupsResult)
                    groupObjectIds.Add(groupObjectID);
            }
            else
            {
                throw new WebException();
            }

            return groupObjectIds;
        }

        /// <summary>
        /// During access management, the user searches for users and groups in the directory and grants them access.
        /// If the given search string matches exactly one user or group in the directory, this method returns its objectId.
        /// </summary>
        /// <param name="searchString">The search string entered by the user to lookup a user or group in the directory.</param>
        /// <returns>The objectID of the matching user or group.</returns>
        public static string LookupObjectIdOfAADUserOrGroup(string searchString, ClaimsIdentity Identity)
        {
            string userOrGroupObjectId = null;
            string tenantId = Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string signedInUserID = Identity.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
            string userObjectID = Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority, new EfAdalTokenCache(signedInUserID));
            AuthenticationResult result = authContext.AcquireTokenSilent(
                SettingsHelper.AzureAdGraphResourceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));

            HttpClient client = new HttpClient();

            string userQueryUrl = string.Format("{0}/{1}/users?api-version={2}&$filter=startswith(displayName,'{3}') or startswith(userPrincipalName,'{3}')",
                SettingsHelper.AzureAdGraphResourceId, tenantId,
                SettingsHelper.GraphAPIVersion, searchString);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, userQueryUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                var users = (System.Web.Helpers.Json.Decode(responseString)).value;
                if (users.Length == 1) userOrGroupObjectId = users[0].objectId;
            }

            if (userOrGroupObjectId == null)
            {
                string groupQueryUrl = string.Format("{0}/{1}/groups?api-version={2}&$filter=startswith(displayName,'{3}')",
                    SettingsHelper.AzureAdGraphResourceId, tenantId,
                    SettingsHelper.GraphAPIVersion, searchString);

                request = new HttpRequestMessage(HttpMethod.Get, groupQueryUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    var groups = (System.Web.Helpers.Json.Decode(responseString)).value;
                    if (groups.Length == 1) userOrGroupObjectId = groups[0].objectId;
                }
            }

            return userOrGroupObjectId;
        }

        /// <summary>
        /// During access management, the user is shown a list of users and groups that currently have access.
        /// Given an objectId of a user or a group, this method returns a display string in the following format "displayName (objectType)".
        /// </summary>
        /// <param name="Identity">Identity of the user to which search for the groups.</param>
        /// <returns>String containing the display name for the user or group.</returns>
        public static string LookupDisplayNameOfAADObject(string objectIdToLookUpFor, ClaimsIdentity Identity)
        {
            string objectDisplayName = null;
            string tenantId = Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string signedInUserID = Identity.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
            string userObjectID = Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority, new EfAdalTokenCache(signedInUserID));
            AuthenticationResult result = authContext.AcquireTokenSilent(
                SettingsHelper.AzureAdGraphResourceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));

            HttpClient client = new HttpClient();

            string doQueryUrl = string.Format("{0}/{1}/directoryObjects/{2}?api-version={3}",
                SettingsHelper.AzureAdGraphResourceId, tenantId,
                objectIdToLookUpFor, SettingsHelper.GraphAPIVersion);


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, doQueryUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                var directoryObject = System.Web.Helpers.Json.Decode(responseString);
                if (directoryObject != null) objectDisplayName = string.Format("{0}", directoryObject.displayName);
            }

            return objectDisplayName;
        }

        /// <summary>
        /// The global administrators and user account administrators of the directory are automatically assgined the admin role in the application.
        /// This method determines whether the user is a member of the global administrator or user account administrator directory role.
        /// RoleTemplateId of Global Administrator role = 62e90394-69f5-4237-9190-012177145e10
        /// RoleTemplateId of User Account Administrator role = fe930be7-5e62-47db-91af-98c3a49a38b1
        /// </summary>
        /// <param name="objectId">The objectId of user or group that currently has access.</param>
        /// <returns>String containing the display string for the user or group.</returns>
        public static bool IsUserAADAdmin(ClaimsIdentity Identity)
        {
            string tenantId = Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string signedInUserID = Identity.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
            string userObjectID = Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);

            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's EF DB
            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority, new EfAdalTokenCache(signedInUserID));

            AuthenticationResult result = authContext.AcquireTokenSilent(
                SettingsHelper.AzureAdGraphResourceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));

            HttpClient client = new HttpClient();

            string doQueryUrl = string.Format("{0}/{1}/users/{2}/memberOf?api-version={3}",
                SettingsHelper.AzureAdGraphResourceId, tenantId,
                userObjectID, SettingsHelper.GraphAPIVersion);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, doQueryUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                var memberOfObjects = (System.Web.Helpers.Json.Decode(responseString)).value;

                if (memberOfObjects != null)
                    foreach (var memberOfObject in memberOfObjects)
                        if (memberOfObject.objectType == "Role" && (
                            memberOfObject.roleTemplateId.Equals("62e90394-69f5-4237-9190-012177145e10", StringComparison.InvariantCultureIgnoreCase) ||
                            memberOfObject.roleTemplateId.Equals("fe930be7-5e62-47db-91af-98c3a49a38b1", StringComparison.InvariantCultureIgnoreCase)))
                            return true;
            }

            return false;
        }
    }
}