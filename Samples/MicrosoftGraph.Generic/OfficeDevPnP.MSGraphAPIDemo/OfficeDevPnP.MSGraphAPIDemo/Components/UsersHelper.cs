using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public class UsersHelper
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
                String.Format("{0}users/{1}/directReports",
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
        /// This method retrieves the list of members of a group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>The full list members of the group</returns>
        public static List<User> ListGroupMembers(String groupId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/members",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId));

            var usersList = JsonConvert.DeserializeObject<UsersList>(jsonResponse);
            return (usersList.Users);
        }
    }
}