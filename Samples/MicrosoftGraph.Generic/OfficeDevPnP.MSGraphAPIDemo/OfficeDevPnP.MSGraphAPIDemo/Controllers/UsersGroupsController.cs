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

            var paoloManager = UsersGroupsHelper.GetUserManager("paolo@piasysdev.onmicrosoft.com");
            var paoloMFADirectReports = UsersGroupsHelper.GetUserDirectReports("paoloMFA@piasysdev.onmicrosoft.com");

            return View("Index");
        }

        public ActionResult PlayWithSecurityGroups()
        {
            var groups = UsersGroupsHelper.ListGroups(100);
            var group = UsersGroupsHelper.GetGroup(groups[6].Id);
            var owners = UsersGroupsHelper.ListGroupOwners(group.Id);
            var members = UsersGroupsHelper.ListGroupMembers(group.Id);

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
            var threads = UnifiedGroupsHelper.ListUnifiedGroupThreads(group.Id);
            var conversations = UnifiedGroupsHelper.ListUnifiedGroupConversations(group.Id);

            return View("Index");
        }
    }
}