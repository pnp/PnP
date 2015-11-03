using System;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs.Policy
{
    public interface ILifeCycleHelper
    {
        DateTime GetExpiredDate(SiteInformation site);
        int GetDefaultLifeTimeInMonth(SiteInformation site);
    }
}