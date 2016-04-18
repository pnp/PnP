using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIGroups.Models;
using OfficeDevPnP.MSGraphAPIGroups.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIGroups.Controllers
{
	[Authorize]
	public class GroupsController : Controller
	{
		// GET: Groups
		public async Task<ActionResult> Index()
		{
			List<Group> groups = new List<Group>();

			ViewBag.Title = "All Groups";
			ViewBag.search = "";
			ViewBag.unifiedOnly = false;

			string APIURL = SettingsHelper.MSGraphResource + "/v1.0/myorganization/groups";
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

		[HttpPost]
		public async Task<ActionResult> Index(string search, string unifiedOnly)
		{
			List<Group> groups = new List<Group>();
			string apiUrl = String.Empty;

			ViewBag.Title = "Group search";

			if (!String.IsNullOrEmpty(search))
			{
				ViewBag.search = search;
				ViewBag.unifiedOnly = false;

				apiUrl = String.Format("{0}/v1.0/myorganization/groups?$filter=startswith(displayName,'{1}')",
																				SettingsHelper.MSGraphResource, search);
			}

			if (unifiedOnly == "on")
			{
				ViewBag.search = "";
				ViewBag.unifiedOnly = true;

				apiUrl = String.Format("{0}/v1.0/myorganization/groups?$filter=groupTypes/any(a:a%20eq%20'unified')",
																				SettingsHelper.MSGraphResource, search);

			}

			ViewBag.Message = "API URL: " + apiUrl;

			try
			{
				groups = await HttpHelper.GetGroups(apiUrl);
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

		public async Task<ActionResult> Details(string id)
		{
			Group group = null;

			ViewBag.Title = "Group Details";

			string apiUrl = String.Format("{0}/v1.0/myorganization/groups/{1}", SettingsHelper.MSGraphResource, id);
			ViewBag.Message = "API URL: " + apiUrl;

			try
			{
				string responseContent = await HttpHelper.GetHttpResource(apiUrl);
				group = JsonConvert.DeserializeObject<Group>(responseContent);
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
			return View(group);
		}

		public async Task<ActionResult> Conversations(string id)
		{
			List<Conversation> convos = new List<Conversation>();

			ViewBag.Title = "Group Conversations";
			ViewBag.GroupId = id;

			string apiUrl = String.Format("{0}/v1.0/myorganization/groups/{1}/conversations", SettingsHelper.MSGraphResource, id);
			ViewBag.Message = "API URL: " + apiUrl;

			try
			{
				string responseContent = await HttpHelper.GetHttpResource(apiUrl);
				var responseObject = JsonConvert.DeserializeObject<GraphResponse<Conversation>>(responseContent);
				foreach (var item in responseObject.value)
				{
					convos.Add(item);
				}
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
			return View(convos);
		}

		public async Task<ActionResult> Threads(string id, string itemId)
		{
			List<ConversationThread> threads = new List<ConversationThread>();

			ViewBag.Title = "Group ConversationThreads";
			ViewBag.GroupId = id;
			ViewBag.ConversationId = itemId;

			string apiUrl = String.Format("{0}/v1.0/myorganization/groups/{1}/conversations/{2}/threads", 
																		SettingsHelper.MSGraphResource, 
																		id, itemId);
			ViewBag.Message = "API URL: " + apiUrl;

			try
			{
				string responseContent = await HttpHelper.GetHttpResource(apiUrl);
				var responseObject = JsonConvert.DeserializeObject<GraphResponse<ConversationThread>>(responseContent);
				foreach (var item in responseObject.value)
				{
					threads.Add(item);
				}
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
			return View(threads);
		}

		public async Task<ActionResult> Posts(string id, string itemId)
		{
			List<Post> posts = new List<Post>();

			ViewBag.Title = "Group ConversationThread Posts";
			ViewBag.GroupId = id;

			string apiUrl = String.Format("{0}/v1.0/myorganization/groups/{1}/threads/{2}/posts", SettingsHelper.MSGraphResource, id, itemId);
			ViewBag.Message = "API URL: " + apiUrl;

			try
			{
				string responseContent = await HttpHelper.GetHttpResource(apiUrl);
				var responseObject = JsonConvert.DeserializeObject<GraphResponse<Post>>(responseContent);
				foreach (var item in responseObject.value)
				{
					posts.Add(item);
				}
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
			return View(posts);
		}

		public async Task<ActionResult> Events(string id)
		{
			List<Event> events = new List<Event>();

			ViewBag.Title = "Group Events";
			ViewBag.GroupId = id;

			string apiUrl = String.Format("{0}/v1.0/myorganization/groups/{1}/events", SettingsHelper.MSGraphResource, id);
			ViewBag.Message = "API URL: " + apiUrl;

			try
			{
				string responseContent = await HttpHelper.GetHttpResource(apiUrl);
				var responseObject = JsonConvert.DeserializeObject<GraphResponse<Event>>(responseContent);
				foreach (var item in responseObject.value)
				{
					events.Add(item);
				}
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
			return View(events);

		}

		public async Task<ActionResult> Files(string id)
		{
			List<DriveItem> files = new List<DriveItem>();

			ViewBag.Title = "Group Files";
			ViewBag.GroupId = id;

			string apiUrl = String.Format("{0}/v1.0/myorganization/groups/{1}/drive/root/children", SettingsHelper.MSGraphResource, id);
			ViewBag.Message = "API URL: " + apiUrl;

			try
			{
				string responseContent = await HttpHelper.GetHttpResource(apiUrl);
				var responseObject = JsonConvert.DeserializeObject<GraphResponse<DriveItem>>(responseContent);

				foreach (var item in responseObject.value)
				{
					files.Add(item);
				}
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
			return View(files);
		}
	}
}