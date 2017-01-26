namespace AspNetCore.Mvc.StarterWeb.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using OfficeDevPnP.Core.Framework.Authentication;
    using AspNetCore.Mvc.StarterWeb.Models;
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    public class ListsController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            if (SharePointContextProvider.Current == null) return View(); //null if middleware not intercepted

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var spLists = new List<SharePointListViewModel>();

            if (spContext == null) return View(); //issues with configuration

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