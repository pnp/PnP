using System;
using System.Globalization;
using System.Linq;
using Governance.TimerJobs.Data;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;

namespace Governance.TimerJobs.Policy
{
    public class GovernanceWorkflowExecutor
    {
        public GovernanceWorkflowExecutor(ClientContext tenentClientContext)
        {
            TenentClientContext = tenentClientContext;
        }

        protected ClientContext TenentClientContext { get; set; }

        public virtual void Enforce(SiteInformation site, bool supressEmail)
        {
            var state = site.ComplianceState;

            //If site is locked, then will not send any lock notification email, instead of sending delete email.
            if (site.ComplianceState.IsLocked && !site.ComplianceState.DeleteNotificationSent)
                SetNotifyDeleteState(site);
            if (GovernanceWorkflowHelper.NeedNotifyLock(state.LockedDate, state))
            {
                ExtendOutdatedLockedDate(site); //Extend the LockedDate if it is earlier than current date
            }
            else if (GovernanceWorkflowHelper.NeedNotifyDelete(state.LockedDate, state))
            {
                ExtendOutdatedDeleteDate(site); //Extend the DeleteDate if it is earlier than current date
            }
            if (state.IsCompliant)
                ChangeComplianceStateToDefault(site);

            var tenant = new Tenant(TenentClientContext);
            if (GovernanceWorkflowHelper.NeedNotifyLock(state.LockedDate, state))
            {
                Notifiy(site, TenentClientContext, supressEmail);
                Log.Info(GetType().Name, "Notify Lock for site {0}", site.Url);
            }
            else if (GovernanceWorkflowHelper.NeedLock(state.LockedDate, state))
            {
                tenant.SetSiteLockState(site.Url, SiteLockState.NoAccess, true);
                site.ComplianceState.IsLocked = true;
                Log.Info(GetType().Name, "Site {0} was locked", site.Url);
            }
            else if (GovernanceWorkflowHelper.NeedNotifyDelete(state.LockedDate, state))
            {
                Notifiy(site, TenentClientContext, supressEmail);                
                Log.Info(GetType().Name, "Notify Delete for site {0}", site.Url);
            }
            else if (GovernanceWorkflowHelper.NeedDelete(state.LockedDate))
            {
                //set a flag to let us know that the site is deleted by Governance Job
                //site.DeletedBy = AutoSiteDeleteBy.GovernanceJob;
                tenant.DeleteSiteCollection(site.Url, true);
                //Set a value to indicate that the site was just deleted
                site.ComplianceState.DeleteDate = DateTime.MaxValue;
                Log.Info(GetType().Name, "Site {0} was deleted", site.Url);
            }
            TenentClientContext.ExecuteQueryRetry();
        }

        //LockedDate will be displayed in first&second notification email
        //Extend LockedDate to no earlier than the email sent date if needed to avoid confusion
        private void ExtendOutdatedLockedDate(SiteInformation site)
        {
            var now = GovernanceWorkflowHelper.GetCurrentBusinessTime();
            if (site.ComplianceState.LockedDate < now)
            {
                var lockDate = DateTime.MinValue;
                if (!site.ComplianceState.FirstLockNotificationSent)
                {
                    lockDate = now.AddDays(GovernanceWorkflowHelper.FirstLockNotificationDays);
                }
                else if (!site.ComplianceState.SecondLockNotificationSent)
                {
                    lockDate = now.AddDays(GovernanceWorkflowHelper.SecondLockNotificationDays);
                }
                if (lockDate == DateTime.MinValue) return;
                site.ComplianceState.LockedDate = lockDate;
                if (site.ComplianceState.ExpireDate < lockDate)
                    site.ComplianceState.ExpireDate = lockDate;
            }
        }

        //DeleteDate will be displayed in delete notification email
        //Extend DeleteDate to no earlier than the email sent date if needed to avoid confusion
        private void ExtendOutdatedDeleteDate(SiteInformation site)
        {
            var now = GovernanceWorkflowHelper.GetCurrentBusinessTime();
            if (site.ComplianceState.LockedDate.AddDays(GovernanceWorkflowHelper.DeleteDays - 7) < now)
            {
                var lockDate = now.AddDays(-GovernanceWorkflowHelper.DeleteNotificationDays);
                site.ComplianceState.LockedDate = lockDate;

                if (site.ComplianceState.ExpireDate < lockDate)
                    site.ComplianceState.ExpireDate = lockDate;
            }
        }

        public void ChangeComplianceStateToDefault(SiteInformation site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            var status = site.ComplianceState;
            status.DeleteNotificationSentDate = DateTime.MinValue;
            status.DeleteNotificationSent = false;

            status.DeleteDate = DateTime.MinValue;
            status.LockedDate = DateTime.MinValue;

            status.FirstLockNotificationSentDate = DateTime.MinValue;
            status.FirstLockNotificationSent = false;

            status.IsLocked = false;
            status.IsReadonly = false;
            status.SecondLockNotificationSent = false;
            status.SecondLockNotificationSentDate = DateTime.MinValue;

            status.IsCompliant = true;
        }

        private void SetNotifyDeleteState(SiteInformation site)
        {
            var now = GovernanceWorkflowHelper.GetCurrentBusinessTime();

            if (site.ComplianceState.IsLocked && (!site.ComplianceState.FirstLockNotificationSent
                                                  || !site.ComplianceState.SecondLockNotificationSent))
            {
                if (!site.ComplianceState.FirstLockNotificationSent)
                {
                    site.ComplianceState.FirstLockNotificationSent = true;
                    site.ComplianceState.FirstLockNotificationSentDate = now;
                    site.ComplianceState.SecondLockNotificationSent = true;
                    site.ComplianceState.SecondLockNotificationSentDate = now;
                }
                else if (!site.ComplianceState.SecondLockNotificationSent)
                {
                    site.ComplianceState.SecondLockNotificationSent = true;
                    site.ComplianceState.SecondLockNotificationSentDate = now;
                }
            }

            ExtendOutdatedDeleteDate(site);
        }

        protected void Notifiy(SiteInformation site, ClientContext clientContext, bool supressEmail)
        {
            if (site == null)
                throw new ArgumentNullException("site");
            if (site.Administrators == null || site.Administrators.Count == 0)
                return;
            var mailSendTo = site.Administrators.Select(admin => admin.Email).ToList();
            //Get Email Subject
            var subject = GetEmailSubject(site);
            if (string.IsNullOrEmpty(subject))
                return;
            var body = GetEmailBody(site);
            //Sent Email
            if (mailSendTo.Count > 0 && !supressEmail)
            {
                MailUtility.SendEmail(clientContext, mailSendTo, null, subject, body);
            }
            SetNotifyFlag(site);
        }

        public string GetEmailSubject(SiteInformation site)
        {
            if (site == null)
                throw new ArgumentNullException("site");
            var subject = string.Empty;

            //If already sent delete email, then will not send any email.
            if (site.ComplianceState.DeleteNotificationSent)
                return subject;

            if (!site.ComplianceState.FirstLockNotificationSent)
            {
                subject = string.Format(SitePolicyResources.MailSubjectFirstLock, site.Url);
            }
            else if (!site.ComplianceState.SecondLockNotificationSent)
            {
                subject = string.Format(SitePolicyResources.MailSubjectSecondLock, site.Url);
            }
            else if (!site.ComplianceState.DeleteNotificationSent)
            {
                subject = string.Format(SitePolicyResources.MailSubjectDelete, site.Url);
            }
            return subject;
        }

        public string GetEmailBody(SiteInformation site)
        {
            if (site == null)
                throw new ArgumentNullException("site");
            var content = string.Empty;
            string deadline;
            var specifiedZone = TimeZoneInfo.FindSystemTimeZoneById(GovernanceWorkflowHelper.TimeZoneId);
            var timezone = "{0} (" + specifiedZone.StandardName + ")"; //eg: (Pacific Standard Time). 
            if (!site.ComplianceState.FirstLockNotificationSent)
            {
                deadline = string.Format(CultureInfo.CurrentCulture, timezone,
                    site.ComplianceState.LockedDate.ToShortDateString());
                content = string.Format(SitePolicyResources.MailBodyFirstLock, deadline);
            }
            else if (!site.ComplianceState.SecondLockNotificationSent)
            {
                deadline = string.Format(CultureInfo.CurrentCulture, timezone,
                    site.ComplianceState.LockedDate.ToShortDateString());
                content = string.Format(SitePolicyResources.MailBodySecondLock, deadline);
            }
            else if (!site.ComplianceState.DeleteNotificationSent)
            {
                deadline = string.Format(CultureInfo.CurrentCulture, timezone,
                    site.ComplianceState.DeleteDate.ToShortDateString());
                content = string.Format(SitePolicyResources.MailBodyDelete, deadline);
            }
            return content;
        }

        public void SetNotifyFlag(SiteInformation site)
        {
            if (site == null)
                throw new ArgumentNullException("site");
            var now = GovernanceWorkflowHelper.GetCurrentBusinessTime();
            if (!site.ComplianceState.FirstLockNotificationSent)
            {
                site.ComplianceState.FirstLockNotificationSent = true;
                site.ComplianceState.FirstLockNotificationSentDate = now;
            }
            else if (!site.ComplianceState.SecondLockNotificationSent)
            {
                site.ComplianceState.SecondLockNotificationSent = true;
                site.ComplianceState.SecondLockNotificationSentDate = now;
            }
            else if (!site.ComplianceState.DeleteNotificationSent)
            {
                site.ComplianceState.DeleteNotificationSent = true;
                site.ComplianceState.DeleteNotificationSentDate = now;
            }
        }
    }
}