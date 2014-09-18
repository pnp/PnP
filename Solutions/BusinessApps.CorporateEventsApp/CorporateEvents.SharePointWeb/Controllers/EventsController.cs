using CorporateEvents.SharePointWeb.Models;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateEvents.SharePointWeb.Controllers {
    public class EventsController : Controller {
        #region [ Index ]
        [SharePointContextFilter]
        public ActionResult Index(int offset = 0) {
            using (var clientContext = HttpContext.GetUserClientContextForSPHost()) {
                var list = clientContext.Web.GetListByTitle(ListDetails.EventsListName);
                var caml = new CamlQuery() {
                    ViewXml = CAML.ViewQuery()
                };
                var events = list.GetItems(caml);
                clientContext.ExecuteQuery();
                var eventsList = events.Cast<ListItem>().Select(item => new Event(item)).ToList();
                return View(eventsList);
            }
        } 
        #endregion

        #region [ Item ]
        /// <summary>
        /// Gets a specific item and renders a custom interface for the event.
        /// </summary>
        /// <param name="id">Event list item ID</param>
        /// <returns></returns>
        [SharePointContextFilter]
        public ActionResult Item(int id) {
            using (var clientContext = HttpContext.GetUserClientContextForSPHost()) {
                if (!clientContext.Web.ServerObjectIsNull.HasValue) {
                    clientContext.Load(clientContext.Web);
                    clientContext.ExecuteQuery();
                }
                var list = clientContext.Web.GetListByTitle(ListDetails.EventsListName);
                var eventListItem = list.GetItemById(id);
                clientContext.Load(eventListItem);
                clientContext.ExecuteQuery();
                var eventItem = new Event(eventListItem);
                return View(eventItem);
            }
        } 
        #endregion

        #region [ Featured ]
        /// <summary>
        /// Gets the top number of featured events.
        /// </summary>
        /// <param name="maxCount">Maximum number of items to return.</param>
        /// <returns></returns>
        [SharePointContextFilter]
        public ActionResult Featured(int maxCount = 5) {
            using (var clientContext = HttpContext.GetUserClientContextForSPHost()) {
                ViewBag.SPHostUrl = HttpContext.Request.QueryString["SPHostUrl"];

                var caml = new CamlQuery() {
                    ViewXml = CAML.ViewQuery(
                        CAML.Where(
                            CAML.And(
                                CAML.Geq(
                                    CAML.FieldValue(Event.FIELD_DATE, "Date", CAML.Today())
                                ),
                                CAML.Eq(
                                    CAML.FieldValue(Event.FIELD_CATEGORY, "Text", "Featured")
                                )
                            )
                        ),
                        CAML.OrderBy(new OrderByField(Event.FIELD_DATE)))
                };

                var list = clientContext.Web.Lists.GetByTitle(ListDetails.EventsListName);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                var items = list.GetItems(caml);
                clientContext.Load(items);
                clientContext.ExecuteQuery();

                var result = items.Cast<ListItem>()
                                  .Select(i => new Event(i)).ToList();
                return View(result);
            }
        } 
        #endregion

        #region [ Register ]
        [SharePointContextFilter]
        public ActionResult Register(string eventId) {
            ViewBag.EventId = eventId;
            return View();
        }
        [HttpPost]
        [SharePointContextFilter]
        public ActionResult Register(Registration registration) {
            if (!ModelState.IsValid)
                View(registration);

            try {
                using (var clientContext = HttpContext.GetUserClientContextForSPHost()) {
                    registration.Save(clientContext.Web);
                }
                return RedirectToAction("Index", "Registration");
            }
            catch (Exception) {
                return View();
            }
        }
        #endregion
    }
}
