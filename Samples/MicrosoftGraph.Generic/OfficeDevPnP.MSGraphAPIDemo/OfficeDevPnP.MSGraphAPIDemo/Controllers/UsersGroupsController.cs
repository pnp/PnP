using OfficeDevPnP.MSGraphAPIDemo.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIDemo.Controllers
{
    public class UsersGroupsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PlayWithUsers()
        {
            var users = UsersGroupsHelper.ListUsers(600);
            var externalUsers = UsersGroupsHelper.ListExternalUsers(600);
            var usersWithCustomAttributes = UsersGroupsHelper.ListUsers(
                new String[] { "id", "userPrincipalName", "mail",
                    "department", "country", "preferredLanguage",
                    "onPremisesImmutableId", "onPremisesSecurityIdentifier",
                    "onPremisesSyncEnabled", "userType" },
                600);
            var usersWorkingInIT = UsersGroupsHelper.ListUsersByDepartment("IT", 100);
            var paolo = UsersGroupsHelper.GetUser("paolo@piasysdev.onmicrosoft.com");
            var paoloMFA = UsersGroupsHelper.GetUser("paoloMFA@piasysdev.onmicrosoft.com");
            var paoloADFS = UsersGroupsHelper.GetUser("paolo.pialorsi@sharepoint-camp.com");

            var newUser = UsersGroupsHelper.AddUser(
                new Models.User
                {
                    AccountEnabled = true,
                    DisplayName = "API Created",
                    PasswordProfile = new Models.PasswordProfile
                    {
                        ForceChangePasswordNextSignIn = true,
                        Password = "Pass@w0rd!",
                    },
                    UserPrincipalName = "api-created@piasysdev.onmicrosoft.com",
                }
                );

            paoloMFA.City = "Brescia";
            UsersGroupsHelper.UpdateUser(paoloMFA);

            var paoloManager = UsersGroupsHelper.GetUserManager("paolo@piasysdev.onmicrosoft.com");
            var paoloMFADirectReports = UsersGroupsHelper.GetUserDirectReports("paoloMFA@piasysdev.onmicrosoft.com");

            return View("Index");
        }

        public ActionResult PlayWithSecurityGroups()
        {
            var groups = UsersGroupsHelper.ListGroups(100);
            var securityGroups = UsersGroupsHelper.ListSecurityGroups(100);
            var group = UsersGroupsHelper.GetGroup(groups[6].Id);
            var owners = UsersGroupsHelper.ListGroupOwners(group.Id);
            var members = UsersGroupsHelper.ListGroupMembers(group.Id);

            var cristian = UsersGroupsHelper.GetUser("cristian.civera@sharepoint-camp.com");
            UsersGroupsHelper.AddMemberToGroup(cristian, group.Id);
            members = UsersGroupsHelper.ListGroupMembers(group.Id);
            UsersGroupsHelper.RemoveMemberFromGroup(cristian, group.Id);

            return View("Index");
        }

        public ActionResult PlayWithUnifiedGroups()
        {
            var groups = UsersGroupsHelper.ListUnifiedGroups(100);
            var group = UsersGroupsHelper.GetGroup(groups[0].Id);
            var owners = UsersGroupsHelper.ListGroupOwners(group.Id);
            var members = UsersGroupsHelper.ListGroupMembers(group.Id);
            var photo = UsersGroupsHelper.GetGroupPhoto(group.Id);
            var calendar = UnifiedGroupsHelper.GetUnifiedGroupCalendar(group.Id);
            var calendarEvents = UnifiedGroupsHelper.ListUnifiedGroupEvents(group.Id);
            var events = UnifiedGroupsHelper.ListUnifiedGroupEvents(group.Id, DateTime.Now,
                DateTime.Now.AddMonths(1), 0);

            var conversations = UnifiedGroupsHelper.ListUnifiedGroupConversations(group.Id);
            var threads = UnifiedGroupsHelper.ListUnifiedGroupThreads(group.Id);
            var postsOfThread = UnifiedGroupsHelper.ListUnifiedGroupThreadPosts(group.Id, threads[0].Id);
            var singlePostOfThread = UnifiedGroupsHelper.GetUnifiedGroupThreadPost(group.Id, threads[0].Id, postsOfThread[0].Id);

            UnifiedGroupsHelper.ReplyToUnifiedGroupThread(group.Id, threads[0].Id,
                new Models.ConversationThreadPost
                {
                    Body = new Models.ItemBody
                    {
                        Type = Models.BodyType.Html,
                        Content = "<html><body><div>This is the body of a post created via the Microsoft Graph API!</div></body></html>",
                    },
                    NewParticipants = new List<Models.UserInfoContainer>(
                        new Models.UserInfoContainer[] {
                            new Models.UserInfoContainer {
                                Recipient = new Models.UserInfo {
                                    Name = "Paolo Pialorsi",
                                    Address = "paolo@pialorsi.com",
                                }
                            }
                        }),                        
                });

            return View("Index");
        }
    }
}