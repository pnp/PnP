using Microsoft.SharePoint.Client;
using Patterns.Provisioning.Common;
using Patterns.Provisioning.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Patterns.Provisioning.UIWeb.Controllers
{
    public class WizardController : Controller
    {
        //
        // GET: /Wizard/
        public ActionResult Index()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            ViewBag.SiteTemplates = GetSiteTemplates();
            ViewBag.AppSiteUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            ViewBag.HostUrl = spContext.SPHostUrl.ToString().TrimEnd('/');
            ViewBag.AppWebUrl = spContext.SPAppWebUrl.ToString().TrimEnd('/');
            ViewBag.Language = spContext.SPLanguage;
            return View();
        }

        public ActionResult Complete() {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitRequest(WizardModel requestInfo) {
            if (!ModelState.IsValid) {
                return View();
            }

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            SharePointUser currentUser;

            using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                clientContext.Load(clientContext.Web);
                clientContext.Load(clientContext.Web.CurrentUser);
                clientContext.ExecuteQuery();

                var user = clientContext.Web.CurrentUser;
                currentUser = new SharePointUser() {
                    Email = user.Email,
                    Login = user.LoginName,
                    Name = user.Title
                };
            }


            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost()) {
                clientContext.Load(clientContext.Web);
                clientContext.ExecuteQuery();

                var otherOwnersStr = requestInfo.OtherOwners.TrimEnd(';');
                List<SharePointUser> otherOwners = null;

                if (otherOwnersStr.Length > 0) {
                    var ownersSplit = otherOwnersStr.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);

                    otherOwners = new List<SharePointUser>();

                    var users = new List<User>();

                    foreach (var loginName in ownersSplit) {
                        var user = clientContext.Web.SiteUsers.GetByLoginName(loginName);
                        clientContext.Load(user);
                        users.Add(user);
                    }
                    clientContext.ExecuteQuery();

                    foreach (var user in users) {
                        otherOwners.Add(new SharePointUser() {
                            Email = user.Email,
                            Login = user.LoginName,
                            Name = user.Title
                        });
                    }
                }

                var siteRequestInfo = new SiteRequestInformation() {
                    Title = requestInfo.SiteName,
                    Description = requestInfo.Description,
                    EnumStatus = SiteRequestStatus.New,
                    Template = requestInfo.Template,
                    SiteOwner = currentUser,
                    Url = spContext.SPHostUrl.ToString().TrimEnd('/') + "/" + requestInfo.SiteUrl.Trim('/'),
                    AdditionalAdministrators = otherOwners
                };

                var requestFactory = SiteRequestFactory.GetInstance();
                var repository = requestFactory.GetSPSiteRepository(clientContext, Provisioning.Common.Lists.SiteRepositoryTitle); // , "Lists/SiteRequests"
                repository.CreateNewSiteRequest(siteRequestInfo);
            }

            return RedirectToAction("Complete", new { SPHostUrl = spContext.SPHostUrl });
        }


        #region [ GetSiteTemplates ]
        /// <summary>
        /// Gets site templates. This should come from a site template data source.
        /// </summary>
        /// <returns></returns>
        IList<SiteTemplate> GetSiteTemplates() {
            var templates = new List<SiteTemplate>() {
                new SiteTemplate {
                    TemplateId = "100",
                    Name = "STS#0",
                    Title = "Team Site",
                    Description = "Sapien elit in malesuada semper mi, id sollicitudin urna fermentum."
                },
                new SiteTemplate {
                    TemplateId = "200",
                    Name = "STS#1",
                    Title = "Blank Site",
                    Description = "Ut fusce varius nisl ac ipsum gravida vel pretium tellus tincidunt integer eu augue augue nunc elit dolor, luctus placerat."
                },
                new SiteTemplate {
                    TemplateId = "300",
                    Name = "BLOG#0",
                    Title = "Blog Site",
                    Description = "In malesuada semper mi, id sollicitudin urna fermentum ut fusce varius nisl."
                }//,
                //new SiteTemplate {
                //    TemplateId = "400",
                //    Name = "BLOG#0",
                //    Title = "Blog Site",
                //    Description = "In malesuada semper mi, id sollicitudin urna fermentum ut fusce varius nisl."
                //}
            };
            return templates;
        }
        #endregion
	}
}