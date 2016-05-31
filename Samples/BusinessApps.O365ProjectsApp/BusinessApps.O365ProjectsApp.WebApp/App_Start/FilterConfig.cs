using System.Web;
using System.Web.Mvc;

namespace BusinessApps.O365ProjectsApp.WebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
