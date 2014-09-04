using System.Web;
using System.Web.Mvc;

namespace Patterns.Provisioning.UIWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
