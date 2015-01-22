using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Office365Api.MVCDemo.Models;
using Office365Api.Helpers;
using System.Security.Claims;

namespace Office365Api.MVCDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult UseOffice365API()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ListMyFiles()
        {
            HomeViewModel model = new HomeViewModel();

            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            authenticationHelper.EnsureAuthenticationContext(new ADALTokenCache(signInUserId));

            MyFilesHelper myFilesHelper = new MyFilesHelper(authenticationHelper);
            var myFiles = await myFilesHelper.GetMyFiles();

            model.Office365ActionResult = String.Format("Found {0} my files! Showing first 10, if any.", myFiles.Count());

            foreach (var item in myFiles.Take(10))
            {
                model.Items.Add(String.Format(
                    "URL: {0}",
                    !String.IsNullOrEmpty(item.WebUrl) ? item.WebUrl : String.Empty));
            }

            return View("UseOffice365API", model);
        }

        [Authorize]
        public async Task<ActionResult> ListMyContacts()
        {
            HomeViewModel model = new HomeViewModel();

            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            authenticationHelper.EnsureAuthenticationContext(new ADALTokenCache(signInUserId));

            ContactsHelper contactsHelper = new ContactsHelper(authenticationHelper);
            var contacts = await contactsHelper.GetContacts();

            model.Office365ActionResult = String.Format("Found {0} contacts! Showing first 10, if any.", contacts.Count());

            foreach (var item in contacts.Take(10))
            {
                model.Items.Add(String.Format(
                    "Name: {0} - Email: {1}",
                    !String.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : String.Empty,
                    item.EmailAddresses != null ? item.EmailAddresses.First().Address : String.Empty));
            }

            return View("UseOffice365API", model);
        }

        [Authorize]
        public async Task<ActionResult> ListMyMessages()
        {
            HomeViewModel model = new HomeViewModel();

            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            authenticationHelper.EnsureAuthenticationContext(new ADALTokenCache(signInUserId));

            MailHelper mailHelper = new MailHelper(authenticationHelper);
            var mails = await mailHelper.GetMessages();

            model.Office365ActionResult = String.Format("Found {0} mails! Showing first 10, if any.", mails.Count());

            foreach (var item in mails.Take(10))
            {
                model.Items.Add(String.Format(
                    "From: {0} - Subject: {1}",
                    item.From != null ? item.From.EmailAddress.Address : "",
                    !String.IsNullOrEmpty(item.Subject) ? item.Subject : String.Empty));
            }

            return View("UseOffice365API", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SendMail(String targetEMail)
        {
            HomeViewModel model = new HomeViewModel();

            model.TargetEMail = targetEMail;

            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            authenticationHelper.EnsureAuthenticationContext(new ADALTokenCache(signInUserId));

            MailHelper mailHelper = new MailHelper(authenticationHelper);
            await mailHelper.SendMail(targetEMail, "Let's Hack-A-Thon - Office365Api.MVCDemo", "This will be <B>fun...</B>");
            model.Office365ActionResult = "Email sent!";

            return View("UseOffice365API", model);
        }
    }
}