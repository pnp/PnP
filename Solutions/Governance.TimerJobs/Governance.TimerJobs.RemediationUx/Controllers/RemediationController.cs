using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Microsoft.Online.SharePoint.TenantAdministration;
using Provisioning.Common.Authentication;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using Governance.TimerJobs.Data;
using System.Configuration;

namespace Governance.TimerJobs.RemediationUx.Controllers
{
    public abstract class RemediationController : ApiController
    {
        protected static GovernanceDbRepository DbRepository
        {
            get;
            set;
        }

        static RemediationController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            DbRepository = new GovernanceDbRepository(connectionString);
        }

        protected static void UsingTenantContext(Action<ClientContext> action)
        {
            var auth = new AppOnlyAuthenticationTenant();
            using(var context = auth.GetAuthenticatedContext())
            {
                action(context);
            }
        }
    }
}
