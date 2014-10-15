using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OfficeDevPnP.Core.WebAPI
{
    public class WebAPIContextFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (WebAPIHelper.HasCacheEntry(actionContext.ControllerContext))
            {
                return;
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, CoreResources.Services_AccessDenied);
                return;
            }
        }
    }
}
