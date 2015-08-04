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
using Governance.TimerJobs.Policy;

namespace Governance.TimerJobs.RemediationUx.Controllers
{
    public class SiteController : RemediationController
    {
        public SiteInformation Get()
        {
            var result = new SiteInformation();
            var siteUrl = Request.GetQueryNameValuePairs().FirstOrDefault(p => p.Key == "siteUrl").Value;
            DbRepository.UsingContext(dbContext =>
            {
                var site = dbContext.GetSite(siteUrl);
                if (site == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format(UxResources.SiteRecordNotFound, siteUrl);
                }
                else
                {
                    ILifeCycleHelper helper = new LifeCyclePolicy();                    
                    result.AudienceScope = site.AudienceScope;
                    result.ExpireDate = site.ComplianceState.ExpireDate;
                    result.NeedExtend = GovernanceWorkflowHelper.NeedExtend(
                        helper.GetExpiredDate(site));
                    result.CanDecommission =
                        site.ComplianceState.IsCompliant &&
                        !result.NeedExtend;
                    
                    DateTime tmp = site.ComplianceState.ExpireDate;
                    site.ComplianceState.ExpireDate = DateTime.MinValue;
                    var extendDate = helper.GetExpiredDate(site);
                    site.ComplianceState.ExpireDate = tmp;

                    result.IsSuccess = true;
                }
            });
            return result;
        }

        public OperationResult Post(SiteInformation data)
        {
            var result = new OperationResult();
            try
            {
                var siteUrl = Request.GetQueryNameValuePairs().FirstOrDefault(p => p.Key == "siteUrl").Value;
                DbRepository.UsingContext(dbContext =>
                {
                    var site = dbContext.GetSite(siteUrl);
                    if (site == null)
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format(UxResources.SiteRecordNotFound, siteUrl);
                    }
                    else
                    {
                        site.AudienceScope = data.AudienceScope;
                        var status = site.ComplianceState;     
                        if (data.NeedExtend)
                        {                                                   
                            var now = GovernanceWorkflowHelper.GetCurrentBusinessTime();
                            var lockDate = now.AddDays(GovernanceWorkflowHelper.FirstLockNotificationDays);
                            if (status.ExpireDate > lockDate || 
                                status.ExpireDate == DateTime.MinValue)
                                status.ExpireDate = lockDate;
                            if (status.LockedDate > lockDate || 
                                status.LockedDate == DateTime.MinValue)
                                status.LockedDate = lockDate;
                        }
                        else if (data.IsExtend)
                        {
                            status.ExpireDate = data.ExtendDate;
                        }
                        dbContext.SaveChanges();
                        result.IsSuccess = true;
                    }
                });
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = e.Message;
            }
            return result;
        }
    }
}
