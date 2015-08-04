using System;
using System.Configuration;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs.Policy
{
    /// <summary>
    /// GovernanceWorkflowHelper provides methods and properties which describe the site collection governance process
    /// </summary>
    public static class GovernanceWorkflowHelper
    {
        public static readonly string TimeZoneId = "UTC";

        static GovernanceWorkflowHelper()
        {
            FirstLockNotificationDays =
                Convert.ToInt32(ConfigurationManager.AppSettings["DefaultFirstLockNotificationDays"]);
            SecondLockNotificationDays =
                Convert.ToInt32(ConfigurationManager.AppSettings["DefaultSecondLockNotificationDays"]);
            DeleteNotificationDays = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultDeleteNotificationDays"]);
            DeleteDays = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultDeleteDays"]);
        }

        public static int FirstLockNotificationDays // 30;
        { get; set; }

        public static int SecondLockNotificationDays // 15;
        { get; set; }

        public static int DeleteNotificationDays // 30;
        { get; set; }

        public static int DeleteDays // 90;
        { get; set; }

        public static DateTime GetCurrentBusinessTime()
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZoneId);
        }

        public static bool NeedNotifyLock(DateTime lockDate, ComplianceState state)
        {
            if (lockDate == DateTime.MinValue)
                return false;
            var now = GetCurrentBusinessTime();
            var first = (lockDate - now).TotalDays <= FirstLockNotificationDays &&
                        !state.FirstLockNotificationSent;
            var second = (lockDate - now).TotalDays <= SecondLockNotificationDays
                         && !state.SecondLockNotificationSent;
            return first || second;
        }

        public static bool NeedExtend(DateTime expireDate)
        {
            var now = GetCurrentBusinessTime();
            var ret = (expireDate - now).TotalDays <= FirstLockNotificationDays;
            return ret;
        }

        public static bool NeedLock(DateTime lockDate, ComplianceState state)
        {
            if (lockDate == DateTime.MinValue)
                return false;
            if (state.IsLocked)
                return false;
            var now = GetCurrentBusinessTime();
            var ret = (lockDate - now).TotalDays <= 0;
            return ret;
        }

        public static bool NeedNotifyDelete(DateTime lockDate, ComplianceState state)
        {
            if (lockDate == DateTime.MinValue)
                return false;
            var now = GetCurrentBusinessTime();
            var ret = (now - lockDate).TotalDays >= DeleteNotificationDays &&
                      !state.DeleteNotificationSent;
            if (ret)
            {
                state.DeleteDate = lockDate.AddDays(DeleteDays);
            }

            return ret;
        }

        public static bool NeedDelete(DateTime lockDate)
        {
            if (lockDate == DateTime.MinValue)
                return false;
            var now = GetCurrentBusinessTime();
            var ret = (now - lockDate).TotalDays >= DeleteDays;
            return ret;
        }
    }
}