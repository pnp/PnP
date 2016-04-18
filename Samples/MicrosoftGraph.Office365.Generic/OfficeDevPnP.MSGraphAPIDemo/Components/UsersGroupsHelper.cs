using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class UsersGroupsHelper
    {
        /// <summary>
        /// This method retrieves the list of users registered in Azure AD
        /// </summary>
        /// <param name="numberOfItems">Defines the TOP number of items to retrieve</param>
        /// <returns>The list of users in Azure AD</returns>
        public static List<User> ListUsers(Int32 numberOfItems = 100)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}users?$top={1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    numberOfItems));

            var usersList = JsonConvert.DeserializeObject<UsersList>(jsonResponse);
            return (usersList.Users);
        }

        /// <summary>
        /// This method retrieves the list of all the external users for a tenant
        /// </summary>
        /// <param name="numberOfItems">Defines the TOP number of items to retrieve</param>
        /// <returns>The list of externa users in Azure AD</returns>
        public static List<User> ListExternalUsers(Int32 numberOfItems = 100)
        {

            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}users?$filter=userType%20eq%20'Guest'&$top={1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    numberOfItems));

            var usersList = JsonConvert.DeserializeObject<UsersList>(jsonResponse);
            return (usersList.Users);
        }

        /// <summary>
        /// This method retrieves the list of users registered in Azure AD with custom fields
        /// </summary>
        /// <param name="fields">The list of fields to retrieve</param>
        /// <param name="numberOfItems">Defines the TOP number of items to retrieve</param>
        /// <returns>The list of users in Azure AD</returns>
        public static List<User> ListUsers(String[] fields = null, Int32 numberOfItems = 100)
        {
            String selectFilter = String.Empty;

            if (fields != null)
            {
                selectFilter = "&$select=";
                foreach (var field in fields)
                {
                    selectFilter += HttpUtility.UrlEncode(field) + ",";
                }
            }

            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}users?$top={1}{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    numberOfItems,
                    selectFilter));

            var usersList = JsonConvert.DeserializeObject<UsersList>(jsonResponse);
            return (usersList.Users);
        }

        /// <summary>
        /// This method retrieves the list of users working in a specific department
        /// </summary>
        /// <param name="department">The department to filter the users on</param>
        /// <param name="numberOfItems">Defines the TOP number of items to retrieve</param>
        /// <returns>The list of users in Azure AD</returns>
        public static List<User> ListUsersByDepartment(String department,
            Int32 numberOfItems = 100)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}users?$filter=department%20eq%20'{1}'&$top={2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    department,
                    numberOfItems));

            var usersList = JsonConvert.DeserializeObject<UsersList>(jsonResponse);
            return (usersList.Users);
        }

        /// <summary>
        /// This method retrieves a single user from Azure AD
        /// </summary>
        /// <param name="upn">The UPN of the user to retrieve</param>
        /// <returns>The user retrieved from Azure AD</returns>
        public static User GetUser(String upn)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}users/{1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    upn));

            var user = JsonConvert.DeserializeObject<User>(jsonResponse);
            return (user);
        }

        /// <summary>
        /// This method retrieves the photo of a single user from Azure AD
        /// </summary>
        /// <param name="upn">The UPN of the user</param>
        /// <returns>The user's photo retrieved from Azure AD</returns>
        public static Stream GetUserPhoto(String upn)
        {
            String contentType = "image/png";

            var result = MicrosoftGraphHelper.MakeGetRequestForStream(
                String.Format("{0}users/{1}/photo/$value",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, upn),
                contentType);

            return (result);
        }

        /// <summary>
        /// This method adds a new user to Azure AD
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <returns>The just added user</returns>
        public static User AddUser(User user)
        {
            String jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}users",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri),
                user, "application/json");

            var addedUser = JsonConvert.DeserializeObject<User>(jsonResponse);
            return (addedUser);
        }

        /// <summary>
        /// This method updated an existing user in Azure AD
        /// </summary>
        /// <param name="user">The user's fields to update</param>
        public static void UpdateUser(User user)
        {
            MicrosoftGraphHelper.MakePatchRequestForString(
                String.Format("{0}users/{1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, 
                    user.Id),
                user, "application/json");
        }

        /// <summary>
        /// This method returns the manager of a user
        /// </summary>
        /// <param name="upn">The UPN of the user</param>
        /// <returns>The user's manager</returns>
        public static User GetUserManager(String upn)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}users/{1}/manager",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    upn));

            var user = JsonConvert.DeserializeObject<User>(jsonResponse);
            return (user);
        }

        /// <summary>
        /// This method returns the direct reports of a user
        /// </summary>
        /// <param name="upn">The UPN of the user</param>
        /// <returns>The user's direct reports</returns>
        public static List<User> GetUserDirectReports(String upn)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}users/{1}/directReports",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    upn));

            var directReports = JsonConvert.DeserializeObject<UsersList>(jsonResponse);
            return (directReports.Users);
        }

        /// <summary>
        /// This method returns the groups of a user
        /// </summary>
        /// <param name="upn">The UPN of the user</param>
        /// <returns>The user's groups</returns>
        public static List<Group> GetUserGroups(String upn)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}users/{1}/memberOf",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    upn));

            var userGroups = JsonConvert.DeserializeObject<GroupsList>(jsonResponse);
            return (userGroups.Groups);
        }

        /// <summary>
        /// This method retrieves the list of groups registered in Azure AD
        /// </summary>
        /// <param name="numberOfItems">Defines the TOP number of items to retrieve</param>
        /// <returns>The list of groups in Azure AD</returns>
        public static List<Group> ListGroups(Int32 numberOfItems = 100)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups?$top={1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    numberOfItems));

            var groupsList = JsonConvert.DeserializeObject<GroupsList>(jsonResponse);
            return (groupsList.Groups);
        }

        /// <summary>
        /// This method retrieves the list of Security Groups
        /// </summary>
        /// <param name="numberOfItems">Defines the TOP number of items to retrieve</param>
        /// <returns>The list of Security Groups</returns>
        public static List<Group> ListSecurityGroups(Int32 numberOfItems = 100)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups?$filter=SecurityEnabled%20eq%20true" +
                    "&$top={1}", MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    numberOfItems));

            var groupsList = JsonConvert.DeserializeObject<GroupsList>(jsonResponse);
            return (groupsList.Groups);
        }

        /// <summary>
        /// This method retrieves the list of Office 365 Groups
        /// </summary>
        /// <param name="numberOfItems">Defines the TOP number of items to retrieve</param>
        /// <returns>The list of Office 365 Groups</returns>
        public static List<Group> ListUnifiedGroups(Int32 numberOfItems = 100)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups?$filter=groupTypes/any(gt:%20gt%20eq%20'Unified')" +
                    "&$top={1}", MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    numberOfItems));

            var groupsList = JsonConvert.DeserializeObject<GroupsList>(jsonResponse);
            return (groupsList.Groups);
        }

        /// <summary>
        /// This method retrieves a specific group registered in Azure AD
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>The group instance</returns>
        public static Group GetGroup(String groupId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId));

            var group = JsonConvert.DeserializeObject<Group>(jsonResponse);
            return (group);
        }

        /// <summary>
        /// This method retrieves the photo of a group from Azure AD
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>The group's photo retrieved from Azure AD</returns>
        public static Stream GetGroupPhoto(String groupId)
        {
            String contentType = "image/png";

            var result = MicrosoftGraphHelper.MakeGetRequestForStream(
                String.Format("{0}groups/{1}/photo/$value",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, groupId),
                contentType);

            return (result);
        }

        /// <summary>
        /// This method retrieves the list of members of a group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>The members of the group</returns>
        public static List<User> ListGroupMembers(String groupId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/members",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId));

            var usersList = JsonConvert.DeserializeObject<UsersList>(jsonResponse);
            return (usersList.Users);
        }

        /// <summary>
        /// This method retrieves the list of owners of a group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>The owners of the group</returns>
        public static List<User> ListGroupOwners(String groupId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/owners",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId));

            var usersList = JsonConvert.DeserializeObject<UsersList>(jsonResponse);
            return (usersList.Users);
        }

        /// <summary>
        /// This method adds a new member to a group
        /// </summary>
        /// <param name="user">The user to add as a new group's member</param>
        /// <param name="groupId">The ID of the target group</param>
        public static void AddMemberToGroup(User user, String groupId)
        {
            MicrosoftGraphHelper.MakePostRequest(
                String.Format("{0}groups/{1}/members/$ref",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId),
                new GroupMemberToAdd
                {
                    ObjectId = String.Format("{0}users/{1}/id",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, user.UserPrincipalName)
                },
                "application/json");
        }

        /// <summary>
        /// This method removes a member from a group
        /// </summary>
        /// <param name="user">The user to remove from the group</param>
        /// <param name="groupId">The ID of the target group</param>
        public static void RemoveMemberFromGroup(User user, String groupId)
        {
            MicrosoftGraphHelper.MakeDeleteRequest(
                String.Format("{0}groups/{1}/members/{2}/$ref",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId, user.Id));
        }
    }
}