using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace AzureAD.RedisCacheUserProfile.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        // GET: UserProfile
        public ActionResult GetPropertiesForUser()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Models.UserProfile userProfile = Cache.UserProfile.GetUserProfile(ClaimsPrincipal.Current.Identities.First().Name);
            stopWatch.Stop();
            ViewData["EllapsedTime"] = stopWatch.ElapsedMilliseconds;
            return PartialView(userProfile);
        }
    }
}