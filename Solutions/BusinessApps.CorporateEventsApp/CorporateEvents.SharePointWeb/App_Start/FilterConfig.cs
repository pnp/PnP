using System.Web;
using System.Web.Mvc;

namespace CorporateEvents.SharePointWeb {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
