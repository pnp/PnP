using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using OfficeDevPnP.Core.Framework.Authentication;
using AspNet5.Mvc6.StarterWeb.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNet5.Mvc6.StarterWeb.Controllers
{
    public class ListsController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var spLists = new List<SharePointListViewModel>();

            //build a client context to work with data
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var lists = clientContext.Web.Lists;

                    clientContext.Load(lists);

                    clientContext.ExecuteQuery();

                    foreach (var list in lists)
                    {
                        spLists.Add(new SharePointListViewModel() { ListTitle = list.Title });
                    } 
                }
            }

            return View(spLists);
        }
    }
}
