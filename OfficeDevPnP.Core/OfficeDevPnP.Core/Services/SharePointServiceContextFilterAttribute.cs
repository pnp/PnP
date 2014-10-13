using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OfficeDevPnP.Core.Services
{
    public class SharePointServiceContextFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (SharePointServiceHelper.HasCacheEntry(actionContext.ControllerContext))
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
