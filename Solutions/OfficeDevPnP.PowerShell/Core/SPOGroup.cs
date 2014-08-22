using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Core
{
    public static class SPOGroup
    {
        public static Group GetGroup(string name, Web web)
        {
            ClientContext context = web.Context as ClientContext;

            Group group = web.SiteGroups.GetByName(name);
                
                context.Load(group);
                context.Load(group.Users);
                
                context.ExecuteQuery();

                return group;
        }

        public static GroupCollection GetGroups(Web web)
        {
            ClientContext context = web.Context as ClientContext;

            var groups = web.SiteGroups;

            context.Load(groups);

            context.ExecuteQuery();

            return groups;
        }
        
        public static User AddUserToGroup(string loginName, int groupId, Web web)
        {
            User addedUser = null;
            Group group = web.SiteGroups.GetById(groupId);
            User user = SPOUser.EnsureUser(loginName, web);
            if(user != null)
            {
                addedUser = AddUserToGroup(user, group, web);
            }

            return addedUser;
        }

        public static User AddUserToGroup(User user, string groupName, Web web)
        {
            User addedUser = null;
            Group group = GetGroup(groupName, web) as Group;
            if(group != null)
            {
                addedUser = AddUserToGroup(user, group, web);
            }

            return user;
        }

        public static User AddUserToGroup(string loginName, string groupName, Web web)
        {
            User addedUser = null;
            User user = SPOUser.EnsureUser(loginName, web);
            if(user != null)
            {
                addedUser = AddUserToGroup(user, groupName, web);
            }

            return addedUser;
        }
        public static User AddUserToGroup(User user, Group group, Web web)
        {
            ClientContext context = web.Context as ClientContext;
            UserCollection users = group.Users;
            var addeduser = users.AddUser(user);
            context.Load(addeduser);
            context.ExecuteQuery();
            return addeduser;
        }

        public static void RemoveUserFromGroup(User user, string groupName, Web web)
        {
            ClientContext context = web.Context as ClientContext;
            Group group = GetGroup(groupName, web) as Group;
            if (group != null)
            {
                RemoveUserFromGroup(user, group, web);
            }
        }

        public static void RemoveUserFromGroup(int userid, string groupName, Web web)
        {
            ClientContext context = web.Context as ClientContext;

            User user = web.SiteUsers.GetById(userid);
            context.Load(user);

            context.ExecuteQuery();
           
            RemoveUserFromGroup(user, groupName, web);
        }

        public static void RemoveUserFromGroup(string loginName, string groupName, Web web)
        {
            ClientContext context = web.Context as ClientContext;
            
            User user = web.SiteUsers.GetByLoginName(loginName);
            context.Load(user);

            context.ExecuteQuery();

            RemoveUserFromGroup(user, groupName, web);

        }

        public static void RemoveUserFromGroup(User user, Group group, Web web)
        {
            ClientContext context = web.Context as ClientContext;
            UserCollection users = group.Users;

            users.Remove(user);

            context.Load(users);
            context.ExecuteQuery();
        }


        public static User AddUserToGroup(string loginName, Group group)
        {
            ClientContext context = group.Context as ClientContext;
            User user = SPOUser.EnsureUser(loginName, context.Web);
            UserCollection users = group.Users;
            var addeduser = users.AddUser(user);
            context.Load(addeduser);
            context.ExecuteQuery();
            return addeduser;
        }
    }
}
