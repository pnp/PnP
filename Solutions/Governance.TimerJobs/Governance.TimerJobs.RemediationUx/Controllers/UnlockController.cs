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
using Governance.TimerJobs.RemediationUx.Model;

namespace Governance.TimerJobs.RemediationUx.Controllers
{
    public class UnlockController : RemediationController
    {
        public OperationResult Get()
        {
            var siteUrl = Request.GetQueryNameValuePairs().FirstOrDefault(p => p.Key == "siteUrl").Value;
            var ret = new OperationResult()
            {
                IsSuccess = true
            };
            UsingTenantContext(context => {                
                try
                {
                    var tenant = new Tenant(context);
                    tenant.SetSiteLockState(siteUrl, SiteLockState.Unlock, wait: true);
                }
                catch (Exception e)
                {
                    ret.IsSuccess = false;
                    ret.Message = e.Message;
                    return;
                }
            });
            if (ret.IsSuccess)
            {
                DbRepository.UsingContext(dbContext => {
                    var site = dbContext.GetSite(siteUrl);
                    if (site != null)
                        site.ComplianceState.IsLocked = false;
                    dbContext.SaveChanges();
                });
            }
            return ret;
        }

        public static void SetNoAccessRedirectUrl(string host)
        {
             UsingTenantContext(context => {                
                var tenant = new Tenant(context);
                tenant.NoAccessRedirectUrl = string.Format("{0}/{1}", host, "App/index.html#/unlock/");
                context.ExecuteQuery();
            });            
        }
    }
}
