using System.Web;
using System.Web.Mvc;

namespace Office365AddIn.GroupAuthorization
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
