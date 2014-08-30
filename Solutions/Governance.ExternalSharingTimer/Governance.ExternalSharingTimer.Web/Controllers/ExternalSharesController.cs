using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Governance.ExternalSharingTimer.Web;

namespace Governance.ExternalSharingTimer.Web.Controllers
{
    public class ExternalSharesController : Controller
    {
        private ExternalSharingDataEntities db = new ExternalSharingDataEntities();

        // GET: Details/92128104-7BA4-4FEE-BB6C-91CCE968F4DD
        public ActionResult Details(string id)
        {
           if (id == null)
            {
                return View("Error");
            }
            Guid uniqueID;
            try
            {
                uniqueID = new Guid(id);
            }
            catch (Exception)
            {
                return View("Error");
            }
            ExternalShare externalShare = db.ExternalShares.FirstOrDefault(i => i.UniqueIdentifier == uniqueID);
            if (externalShare == null)
            {
                return View("Error");
            }
            return View(externalShare);
        }

        // GET: Extend/92128104-7BA4-4FEE-BB6C-91CCE968F4DD
        public ActionResult Extend(string id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Guid uniqueID;
            try
            {
                uniqueID = new Guid(id);
            }
            catch (Exception)
            {
                return View("Error");
            }
            ExternalShare externalShare = db.ExternalShares.FirstOrDefault(i => i.UniqueIdentifier == uniqueID);
            if (externalShare == null)
            {
                return View("Error");
            }

            //update the share with a new RefreshSharedDate
            externalShare.RefreshSharedDate = DateTime.Now;
            db.SaveChanges();

            return View(externalShare);
        }

        // GET: Revoke/92128104-7BA4-4FEE-BB6C-91CCE968F4DD
        public ActionResult Revoke(string id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Guid uniqueID;
            try
            {
                uniqueID = new Guid(id);
            }
            catch (Exception)
            {
                return View("Error");
            }
            ExternalShare externalShare = db.ExternalShares.FirstOrDefault(i => i.UniqueIdentifier == uniqueID);
            if (externalShare == null)
            {
                return View("Error");
            }

            //get an AppOnly accessToken and clientContext for the site collection
            Uri siteUri = new Uri(externalShare.SiteCollectionUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;
            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken))
            {
                //remove the SPUser from the site
                clientContext.Web.SiteUsers.RemoveById(externalShare.UserId);
                clientContext.ExecuteQuery();

                //delete the record
                db.ExternalShares.Remove(externalShare);
                db.SaveChanges();
            }

            //display the confirmation
            return View(externalShare);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
