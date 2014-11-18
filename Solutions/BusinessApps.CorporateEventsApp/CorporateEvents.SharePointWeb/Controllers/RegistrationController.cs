using CorporateEvents.SharePointWeb.Models;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateEvents.SharePointWeb.Controllers
{
    public class RegistrationController : Controller
    {
        #region Queries
        static readonly string QUERY_MYREGISTRATIONS = CAML.ViewQuery(
                CAML.Where(
                    CAML.Eq(CAML.FieldValue("Author", "Integer", CAML.Me))
                ),
                CAML.OrderBy(new OrderByField("RegistrationDate"))
            );
        static readonly string QUERY_REGISTRATION_BY_EVENT_ID = CAML.ViewQuery(
                CAML.Where(
                    CAML.Eq(CAML.FieldValue("RegistrationEventId", "Text", "{0}"))
                ),
                CAML.OrderBy(new OrderByField("RegistrationDate"))
            ); 
        #endregion

        // GET: Registration
        public ActionResult Index()
        {
            using (var clientContext = HttpContext.GetUserClientContextForSPHost()) {
                var list = clientContext.Web.Lists.GetByTitle(ListDetails.RegistrationListName);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                var camlQuery = new CamlQuery() {
                    ViewXml = QUERY_MYREGISTRATIONS
                };

                var results = list.GetItems(camlQuery);
                clientContext.Load(results);
                clientContext.ExecuteQuery();

                var myRegistrations = results.Cast<ListItem>()
                                           .Select(i => new Registration(i)).ToList();

                return View(myRegistrations);
            }
        }

        // GET: Registration/Details/5
        public ActionResult Details(int eventId)
        {
            return View();
        }

        // GET: Registration/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Registration/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Registration registration)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Registration/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Registration/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Registration registration)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
