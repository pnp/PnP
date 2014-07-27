using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Microsoft.SharePoint.Client;

namespace Core.DataStorageModelsWeb.Controllers
{
    [SharePointContextFilter]
    public class CustomerDashboardController : Controller
    {
        public ActionResult Home()
        {
            ViewBag.SharePointContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            return View();
        }

        public ActionResult Notes()
        {
            ViewBag.SharePointContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            return View();
        }

        public ActionResult Orders(string customerId)
        {            
            Order[] orders;
            using (var db = new NorthWindEntities())
            {
                orders = db.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Include(o => o.Shipper)
                    .Where(c => c.CustomerID == customerId)
                    .ToArray();
            }

            ViewBag.SharePointContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            return View(orders);
        }
    }
}