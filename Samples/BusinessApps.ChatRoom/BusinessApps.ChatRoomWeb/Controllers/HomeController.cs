using BusinessApps.ChatRoomWeb.Hubs;
using BusinessApps.ChatRoomWeb.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace BusinessApps.ChatRoomWeb.Controllers
{
    public class HomeController : Controller
    {
        private static ChatHub _chatHub;

        [SharePointContextFilter]
        public ActionResult Index()
        {
            if (_chatHub == null)
                _chatHub = new ChatHub();

            GetUserDetails();

            return View();
        }

        [SharePointContextFilter]
        public void SendMessage(string message)
        {
            if (Session["UserAccountName"] == null || HttpRuntime.Cache[Session["UserAccountName"].ToString()] == null)
                GetUserDetails();

            UserInfo userInfo = (UserInfo)HttpRuntime.Cache[Session["UserAccountName"].ToString()];

            _chatHub.PushMessage(userInfo.DisplayName, userInfo.PhotoUrl, DateTime.UtcNow.ToString(), message);
        }

        [SharePointContextFilter]
        public void JoinRoom()
        {
            if (Session["UserAccountName"] != null)
            {
                UserInfo userInfo = (UserInfo)HttpRuntime.Cache[Session["UserAccountName"].ToString()];
                _chatHub.JoinRoom(userInfo.DisplayName);
            }
        }

        [SharePointContextFilter]
        public void LeaveRoom()
        {
            if (Session["UserAccountName"] != null)
            {
                UserInfo userInfo = (UserInfo)HttpRuntime.Cache[Session["UserAccountName"].ToString()];
                _chatHub.LeaveRoom(userInfo.DisplayName);
            }
        }

        [SharePointContextFilter]
        public void Ping()
        {
            if (Session["UserAccountName"] != null && HttpRuntime.Cache[Session["UserAccountName"].ToString()] != null)
            {
                ((UserInfo)HttpRuntime.Cache[Session["UserAccountName"].ToString()]).LastPing = DateTime.Now;
            }
        }

        private void GetUserDetails()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    PeopleManager peopleManager = new PeopleManager(clientContext);
                    PersonProperties properties = peopleManager.GetMyProperties();
                    clientContext.Load(properties);
                    clientContext.ExecuteQuery();

                    UserInfo userInfo = new UserInfo()
                    {
                        DisplayName = properties.DisplayName,
                        PhotoUrl = spContext.SPHostUrl + "/_layouts/userphoto.aspx?accountname=" + properties.Email,
                        LastPing = DateTime.Now
                    };


                    HttpRuntime.Cache.Add(properties.AccountName, userInfo, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 3, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(CacheRemovalCallback));

                    Session["UserAccountName"] = properties.AccountName;
                }
            }
        }

        private void CacheRemovalCallback(string key, object value, CacheItemRemovedReason reason)
        {
            _chatHub.LeaveRoom(((UserInfo)value).DisplayName);
        }
    }
}
