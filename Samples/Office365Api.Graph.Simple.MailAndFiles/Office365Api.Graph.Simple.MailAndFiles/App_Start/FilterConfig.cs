using System.Web;
using System.Web.Mvc;

namespace Office365Api.Graph.Simple.MailAndFiles
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
