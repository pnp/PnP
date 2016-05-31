using System.Web;
using System.Web.Mvc;

namespace MicrosoftGraph.Office365.DotNetSDK.WebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
