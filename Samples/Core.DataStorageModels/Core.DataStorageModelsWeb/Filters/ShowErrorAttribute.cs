using System.Web.Mvc;

namespace Core.DataStorageModelsWeb.Filters
{
    public class ShowErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var viewData = new ViewDataDictionary<HandleErrorInfo>();
            viewData.Add("ErrorMessage", filterContext.Exception.Message);

            filterContext.Result = new ViewResult
            {
                ViewName = "Error",
                ViewData = viewData,
                TempData = filterContext.Controller.TempData
            };
            filterContext.ExceptionHandled = true;
        }
    }
}