using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;


namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectSiteSecurity : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
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

        private static void AddUserToGroup(Web web, Group group, List<Model.User> members)
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


        public override Model.ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template)
        {
            // No need as we don't (for now) can figure out what _additional_ members we have.

            return template;
        }
    }
}
