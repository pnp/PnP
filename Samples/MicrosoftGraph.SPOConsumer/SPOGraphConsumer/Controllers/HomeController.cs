using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SPOGraphConsumer.Components;
using SPOGraphConsumer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPOGraphConsumer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var accessToken = HttpHelper.GetAccessTokenForCurrentUser();
            return View();
        }

        public ActionResult Sites()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Sites(SitesViewModel model)
        {
            try
            {
                var accessToken = HttpHelper.GetAccessTokenForCurrentUser();
                if (!String.IsNullOrEmpty(accessToken))
                {
                    var jsonSite = HttpHelper.MakeGetRequestForString(
                        model.SiteUrlOrId.Contains(",") ?
                            $"{GraphSettings.MicrosoftGraphV1BaseUri}sites/{model.SiteUrlOrId}" :
                            $"{GraphSettings.MicrosoftGraphV1BaseUri}sites/{GraphSettings.SpoTenant}:{model.SiteUrlOrId}",
                        accessToken);

                    if (!String.IsNullOrEmpty(jsonSite))
                    {
                        var site = JsonConvert.DeserializeObject<SiteInfoViewModel>(jsonSite);

                        if (site != null)
                        {
                            return View("SiteInfo", site);
                        }
                    }
                }
            }
            catch (AdalException adalEx)
            {
                // Skip any ADAL Exception and repeat the AuthN challenge
                Debug.WriteLine(adalEx.Message);
            }
            catch (Exception)
            {
                throw;
            }

            return View(model);
        }

        public ActionResult Lists(String siteId)
        {
            try
            {
                var accessToken = HttpHelper.GetAccessTokenForCurrentUser();
                if (!String.IsNullOrEmpty(accessToken))
                {
                    var jsonLists = HttpHelper.MakeGetRequestForString(
                        $"{GraphSettings.MicrosoftGraphV1BaseUri}sites/{siteId}/lists",
                        accessToken);

                    if (!String.IsNullOrEmpty(jsonLists))
                    {
                        var lists = JsonConvert.DeserializeObject<ListsViewModel>(jsonLists);

                        if (lists != null)
                        {
                            lists.SiteId = siteId;
                            return View("ListsTable", lists);
                        }
                    }
                }
            }
            catch (AdalException adalEx)
            {
                // Skip any ADAL Exception and repeat the AuthN challenge
                Debug.WriteLine(adalEx.Message);
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        public ActionResult CreateList(String siteId)
        {
            try
            {
                var accessToken = HttpHelper.GetAccessTokenForCurrentUser();
                if (!String.IsNullOrEmpty(accessToken))
                {
                    var jsonListCreated = HttpHelper.MakePostRequestForString(
                        $"{GraphSettings.MicrosoftGraphV1BaseUri}sites/{siteId}/lists",
                        new {
                            displayName = $"GraphList-{DateTime.Now.ToString("yyyyMMddhhmm")}",
                            list = new { template = "genericList" }
                        }, 
                        "application/json",
                        accessToken);

                    if (!String.IsNullOrEmpty(jsonListCreated))
                    {
                        var listCreated = JsonConvert.DeserializeObject<ListsViewModel>(jsonListCreated);

                        if (listCreated != null)
                        {
                            return View(listCreated);
                        }
                    }
                }
            }
            catch (AdalException adalEx)
            {
                // Skip any ADAL Exception and repeat the AuthN challenge
                Debug.WriteLine(adalEx.Message);
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }
    }
}