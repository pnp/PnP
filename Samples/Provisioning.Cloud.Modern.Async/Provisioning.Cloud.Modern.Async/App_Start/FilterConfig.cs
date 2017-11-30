using System.Web;
using System.Web.Mvc;

namespace Provisioning.Cloud.Modern.Async
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
