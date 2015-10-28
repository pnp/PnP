using System.Web;
using System.Web.Mvc;

namespace Governance.TimerJobs.RemediationUx
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
