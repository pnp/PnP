using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Office365AddIn.GroupAuthorization
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext ctx)
        {
            if (!ctx.HttpContext.User.Identity.IsAuthenticated)
                base.HandleUnauthorizedRequest(ctx);
            else
            {
                ctx.Result = new ViewResult { ViewName = "Error", ViewBag = { message = "Unauthorized." } };
                ctx.HttpContext.Response.StatusCode = 403;
            }
        }


    }
}