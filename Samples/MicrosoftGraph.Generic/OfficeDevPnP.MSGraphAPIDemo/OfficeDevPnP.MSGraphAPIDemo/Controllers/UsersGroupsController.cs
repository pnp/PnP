using OfficeDevPnP.MSGraphAPIDemo.Components;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIDemo.Controllers
{
    public class UsersGroupsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PlayWithUsers(PlayWithUsersViewModel model)
        {
            AntiForgery.Validate();

            var users = UsersGroupsHelper.ListUsers(600);
            var externalUsers = UsersGroupsHelper.ListExternalUsers(600);
            var usersWithCustomAttributes = UsersGroupsHelper.ListUsers(
                new String[] { "id", "userPrincipalName", "mail",
                    "department", "country", "preferredLanguage",
                    "onPremisesImmutableId", "onPremisesSecurityIdentifier",
                    "onPremisesSyncEnabled", "userType" },
                600);

            try
            {
                var usersWorkingInIT = UsersGroupsHelper.ListUsersByDepartment("IT", 100);
                var oneUser = UsersGroupsHelper.GetUser(model.UserPrincipalName);

                oneUser.City = "Brescia";
                UsersGroupsHelper.UpdateUser(oneUser);
            }
            catch (Exception)
            {
                // Something wrong while getting the thumbnail,
                // We will have to handle it properly ...
            }

            try
            {
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
                    UserPrincipalName = $"api-created@{model.UserPrincipalName.Substring(model.UserPrincipalName.IndexOf("@") + 1)}",
                }
                );
            }
            catch (Exception)
            {
                // Something wrong while getting the thumbnail,
                // We will have to handle it properly ...
            }

            try
            {
                var oneUserManager = UsersGroupsHelper.GetUserManager(model.UserPrincipalName);
                var oneUserManagerDirectReports = UsersGroupsHelper.GetUserDirectReports(oneUserManager.UserPrincipalName);
            }
            catch (Exception)
            {
                // Something wrong while getting the thumbnail,
                // We will have to handle it properly ...
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult PlayWithSecurityGroups(PlayWithUsersViewModel model)
        {
            AntiForgery.Validate();

            var groups = UsersGroupsHelper.ListGroups(100);
            var securityGroups = UsersGroupsHelper.ListSecurityGroups(100);
            var group = UsersGroupsHelper.GetGroup(groups[0].Id);
            var owners = UsersGroupsHelper.ListGroupOwners(group.Id);
            var members = UsersGroupsHelper.ListGroupMembers(group.Id);

            var someone = UsersGroupsHelper.GetUser(model.UserPrincipalName);
            UsersGroupsHelper.AddMemberToGroup(someone, group.Id);
            members = UsersGroupsHelper.ListGroupMembers(group.Id);
            UsersGroupsHelper.RemoveMemberFromGroup(someone, group.Id);

            return View("Index");
        }

        [HttpPost]
        public ActionResult PlayWithUnifiedGroups(PlayWithUsersViewModel model)
        {
            AntiForgery.Validate();

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
                                    Name = model.MailSendToDescription,
                                    Address = model.MailSendTo,
                                }
                            }
                        }),
                });

            var drive = UnifiedGroupsHelper.GetUnifiedGroupDrive(group.Id);

            var newUnifiedGroup = UnifiedGroupsHelper.AddUnifiedGroup(
                new Models.Group
                {
                    DisplayName = "Created via API",
                    MailEnabled = true,
                    SecurityEnabled = false,
                    GroupTypes = new List<String>(new String[] { "Unified" }),
                    MailNickname = "APICreated",
                });

            // Wait for a while to complete Office 365 Group creation
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));

            MemoryStream memPhoto = new MemoryStream();
            using (FileStream fs = new FileStream(Server.MapPath("~/AppIcon.png"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Byte[] newPhoto = new Byte[fs.Length];
                fs.Read(newPhoto, 0, (Int32)(fs.Length - 1));
                memPhoto.Write(newPhoto, 0, newPhoto.Length);
                memPhoto.Position = 0;
            }

            try
            {
                if (memPhoto.Length > 0)
                {
                    UnifiedGroupsHelper.UpdateUnifiedGroupPhoto(newUnifiedGroup.Id, memPhoto);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
            }

            UnifiedGroupsHelper.DeleteUnifiedGroup(newUnifiedGroup.Id);

            return View("Index");
        }
    }
}