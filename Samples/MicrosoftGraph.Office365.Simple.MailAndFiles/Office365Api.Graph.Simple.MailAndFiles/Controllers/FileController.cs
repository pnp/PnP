using Office365Api.Graph.Simple.MailAndFiles.Helpers;
using Office365Api.Graph.Simple.MailAndFiles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Office365Api.Graph.Simple.MailAndFiles.Controllers
{
    /// <summary>
    /// Used to get the file information in JSON format for the JavaScript
    /// </summary>
    public class FileController : Controller
    {
        // GET: File
        [HttpGet]
        public async Task<JsonResult> Index()
        {
            string accessToken = (Session[SessionKeys.Login.AccessToken] as string);
            IEnumerable<OD4BFile> files = await GraphHelper.GetPersonalFiles(accessToken);
            return Json(files.OrderByDescending(m => m.LastModifiedDate), JsonRequestBehavior.AllowGet);
        }
    }
}