using System.Web;
using System.Web.Mvc;

namespace OutlookNotificationsAPI.WebAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // Enforce https as http based access causes an infinite OWIN redirect loop
            filters.Add(new RequireHttpsAttribute());
        }
    }
}
