using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Core
{
    [Obsolete("Use CSOM")]
    public static class SPOUser
    {

        public static User EnsureUser(string logonName, Web web)
        {
            ClientContext context = web.Context as ClientContext;
            User user = web.EnsureUser(logonName);

            context.Load(user,u => u.Email, u => u.Id, u => u.IsSiteAdmin, u => u.Groups, u=>u.PrincipalType, u=>u.Title, u=>u.IsHiddenInUI, u=> u.UserId, u=>u.LoginName);
            context.ExecuteQuery();

            return user;
        }
    }
}
