using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OfficeDevPnP.MSGraphAPIGroups
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
					name: "Me",
					url: "me/{action}",
					defaults: new { controller = "Me", action = "Index" }
			);
			routes.MapRoute(
					name: "Account",
					url: "account/{action}",
					defaults: new { controller = "Account", action = "SignIn" }
			);
			routes.MapRoute(
					name: "Groups",
					url: "groups/{id}/{action}/{itemId}",
					defaults: new { controller = "Groups", action = "Index", id = UrlParameter.Optional, itemId = UrlParameter.Optional }
			);
			routes.MapRoute(
					name: "Default",
					url: "{controller}/{action}/{id}",
					defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
