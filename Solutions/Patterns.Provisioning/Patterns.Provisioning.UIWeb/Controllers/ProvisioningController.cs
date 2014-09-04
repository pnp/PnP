using Microsoft.Online.SharePoint.TenantAdministration;
using Patterns.Provisioning.Common;
using Patterns.Provisioning.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Patterns.Provisioning.UIWeb.Controllers
{
    public class ProvisioningController : Controller
    {
        //
        // GET: /Provisioning/
        public ActionResult Index() {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            ViewBag.HostUrl = SharePointContextProvider.Current.GetSharePointContext(HttpContext).SPHostUrl;

            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost()) {
                var requestFactory = SiteRequestFactory.GetInstance();

                //var repository = requestFactory.GetSPSiteRepository(clientContext, "Site Requests"); // , "Lists/SiteRequests"
                //var siteRequests = repository.GetNewRequests();

                //return View(siteRequests);
                return View(new List<Patterns.Provisioning.Common.SiteRequestInformation>());
            }
        }

        [ActionName("SiteExists")]
        public JsonResult CheckIfSiteExists(string url) {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost()) {
                var requestFactory = SiteRequestFactory.GetInstance();

                var repository = requestFactory.GetSPSiteRepository(clientContext, "Site Requests"); // , "Lists/SiteRequests"
                var siteExists = repository.DoesSiteRequestExist(url);

                return Json(siteExists, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetSiteCollectionTemplates() {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost()) {
                Tenant tenant = new Tenant(clientContext);
                var templates = tenant.GetSPOTenantWebTemplates(clientContext.Web.Language, 15);

                clientContext.Load(templates,
                    t => t.Select(tmp => tmp.Name),
                    t => t.Select(tmp => tmp.Title),
                    t => t.Select(tmp => tmp.Description),
                    t => t.Select(tmp => tmp.DisplayCategory));
                clientContext.ExecuteQuery();

                var result = from tmp in templates
                             orderby tmp.Title
                             select new SiteTemplate {
                                 Title = tmp.Title,
                                 Name = tmp.Name,
                                 Description = tmp.Description,
                                 Category = tmp.DisplayCategory
                             };

                return Json(result.ToList());
            }
        }
	}
}