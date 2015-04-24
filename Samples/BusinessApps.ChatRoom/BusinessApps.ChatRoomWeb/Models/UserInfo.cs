using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.ChatRoomWeb.Models
{
    public class UserInfo
    {
        public string DisplayName { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime LastPing { get; set; }
    }
}