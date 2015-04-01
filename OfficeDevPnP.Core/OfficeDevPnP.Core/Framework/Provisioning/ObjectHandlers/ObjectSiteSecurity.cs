using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using User = OfficeDevPnP.Core.Framework.Provisioning.Model.User;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectSiteSecurity : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            // if this is a sub site then we're not provisioning security as by default security is inherited from the root site
            if (web.IsSubSite())
            {
                return;
            }

            var siteSecurity = template.Security;

            var ownerGroup = web.AssociatedOwnerGroup;
            var memberGroup = web.AssociatedMemberGroup;
            var visitorGroup = web.AssociatedVisitorGroup;

            web.Context.Load(ownerGroup, o => o.Users);
            web.Context.Load(memberGroup, o => o.Users);
            web.Context.Load(visitorGroup, o => o.Users);

            web.Context.ExecuteQueryRetry();

            AddUserToGroup(web, ownerGroup, siteSecurity.AdditionalOwners);
            AddUserToGroup(web, memberGroup, siteSecurity.AdditionalMembers);
            AddUserToGroup(web, visitorGroup, siteSecurity.AdditionalVisitors);

            foreach (var admin in siteSecurity.AdditionalAdministrators)
            {
                var user = web.EnsureUser(admin.Name);
                user.IsSiteAdmin = true;
                user.Update();
                web.Context.ExecuteQueryRetry();
            }

        }

        private static void AddUserToGroup(Web web, Group group, List<User> members)
        {
            web.Context.Load(group, o => o.Users);

            if (group.Users.Any())
            {
                foreach (var user in members)
                {
                    var existingUser = web.EnsureUser(user.Name);
                    group.Users.AddUser(existingUser);
                }
                web.Context.ExecuteQueryRetry();
            }
        }


        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {

            // if this is a sub site then we're not creating security entities as by default security is inherited from the root site
            if (web.IsSubSite())
            {
                return template;
            }

            var ownerGroup = web.AssociatedOwnerGroup;
            var memberGroup = web.AssociatedMemberGroup;
            var visitorGroup = web.AssociatedVisitorGroup;

            web.Context.Load(ownerGroup, o => o.Users);
            web.Context.Load(memberGroup, o => o.Users);
            web.Context.Load(visitorGroup, o => o.Users);

            web.Context.ExecuteQueryRetry();

            var owners = new List<User>();
            var members = new List<User>();
            var visitors = new List<User>();

            foreach (var member in ownerGroup.Users)
            {
                owners.Add(new User() {Name = member.LoginName});
            }
            foreach (var member in memberGroup.Users)
            {
                members.Add(new User() { Name = member.LoginName });
            }
            foreach (var member in visitorGroup.Users)
            {
                visitors.Add(new User() { Name = member.LoginName });
            }
            var siteSecurity = new SiteSecurity();
            siteSecurity.AdditionalOwners.AddRange(owners);
            siteSecurity.AdditionalMembers.AddRange(members);
            siteSecurity.AdditionalVisitors.AddRange(visitors);

            var allUsers = web.SiteUsers;
            web.Context.Load(allUsers, users => users.Include(u => u.LoginName, u => u.IsSiteAdmin));
            web.Context.ExecuteQueryRetry();

            var admins = new List<User>();
            foreach (var member in allUsers)
            {
                if (member.IsSiteAdmin)
                {
                    admins.Add(new User() {Name = member.LoginName});
                }
            }
            siteSecurity.AdditionalAdministrators.AddRange(admins);

            template.Security = siteSecurity;

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (baseTemplate != null)
            {
                template = CleanupEntities(template, baseTemplate);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            foreach (var user in baseTemplate.Security.AdditionalAdministrators)
            {
                int index = template.Security.AdditionalAdministrators.FindIndex(f => f.Name.Equals(user.Name));

                if (index > -1)
                {
                    template.Security.AdditionalAdministrators.RemoveAt(index);
                }
            }

            foreach (var user in baseTemplate.Security.AdditionalMembers)
            {
                int index = template.Security.AdditionalMembers.FindIndex(f => f.Name.Equals(user.Name));

                if (index > -1)
                {
                    template.Security.AdditionalMembers.RemoveAt(index);
                }
            }

            foreach (var user in baseTemplate.Security.AdditionalOwners)
            {
                int index = template.Security.AdditionalOwners.FindIndex(f => f.Name.Equals(user.Name));

                if (index > -1)
                {
                    template.Security.AdditionalOwners.RemoveAt(index);
                }
            }

            foreach (var user in baseTemplate.Security.AdditionalVisitors)
            {
                int index = template.Security.AdditionalVisitors.FindIndex(f => f.Name.Equals(user.Name));

                if (index > -1)
                {
                    template.Security.AdditionalVisitors.RemoveAt(index);
                }
            }

            return template;
        }
    }
}
