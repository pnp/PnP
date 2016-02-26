using OfficeDevPnP.MSGraphAPIGroups.Models;
using OfficeDevPnP.MSGraphAPIGroups.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIGroups.Controllers
{
	[Authorize]
	public class MeController : Controller
	{
		// GET: Me
		public ActionResult Index()
		{
			return View();
		}
		public async Task<ActionResult> MyGroups()
		{
			List<Group> groups = new List<Group>();

			ViewBag.Title = "Groups I've Joined";
			ViewBag.EnableSearch = false;

			string APIURL = String.Format("{0}/v1.0/me/memberOf/$/microsoft.graph.group?$filter=groupTypes/any(a:a%20eq%20'unified')",
																			SettingsHelper.MSGraphResource);
			ViewBag.Message = "API URL: " + APIURL;

			try
			{
				groups = await HttpHelper.GetGroups(APIURL);
			}
			catch (WebException webException)
			{
				if (webException.Response != null)
				{
					using (var reader = new StreamReader(webException.Response.GetResponseStream()))
					{
						var responseContent = reader.ReadToEnd();
						ViewBag.Message = responseContent;
					}
				}
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
			}

			return View(groups);

		}

	}
}