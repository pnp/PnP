using Core.DataStorageModelsWeb.Filters;
using System.Web;
using System.Web.Mvc;

namespace Core.DataStorageModelsWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ShowErrorAttribute());
        }
    }
}
