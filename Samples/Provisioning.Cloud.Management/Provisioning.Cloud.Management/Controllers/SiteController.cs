using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Provisioning.Cloud.Management.Repositories;
using model = Provisioning.Cloud.Management.Models;

namespace Provisioning.Cloud.Management.Controllers
{
    [Authorize]
    [HandleError(ExceptionType = typeof(AdalException))]
    public class SiteController : Controller
    {
        ISharePointRepository _sharePointRepository = null;

        public SiteController()
        {
            _sharePointRepository = new SharePointRepository();
        }

        // GET: Site
        public async Task<ActionResult> Index()
        {
            // Get the sites
            IEnumerable<model.Site> sites = await _sharePointRepository.GetSitesAsync();

            // Return the view
            return View(sites);
        }

        // GET: Site/Create
        public async Task<ActionResult> Create()
        {
            // View
            return View();
        }

        // POST: Site/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // Get properties from form
                model.Site siteToCreateProperties =
                    new model.Site()
                    {
                        Title = collection["title"]
                        ,
                        Uri = collection["uri"]
                        ,
                        Language = Convert.ToUInt32(collection["language"])
                        ,
                        Template = collection["template"]
                        ,
                        Owner = collection["owner"]
                        ,
                        StorageMaximumLevel = Convert.ToInt64(collection["storagemaximumlevel"])
                        ,
                        UserCodeMaximumLevel = Convert.ToDouble(collection["usercodemaximumlevel"])
                    };

                // Create site
                _sharePointRepository.CreateSiteAsync(siteToCreateProperties);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: Site/Delete/{uri}
        public async Task<ActionResult> Delete(string uri)
        {
            bool success = await _sharePointRepository.DeleteSiteAsync(uri);

            return RedirectToAction("Index");
        }

        //
        // POST: Site/Delete/{uri}
        [HttpPost]
        public async Task<ActionResult> Delete(string uri, FormCollection collection)
        {
            bool success = await _sharePointRepository.DeleteSiteAsync(uri);

            return RedirectToAction("Index");
        }
    }
}