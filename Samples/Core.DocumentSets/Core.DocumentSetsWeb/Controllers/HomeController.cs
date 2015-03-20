using Core.DocumentSetsWeb.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.DocumentSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.DocumentSetsWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            HomeViewModel model = InitHomeViewModel();
            return View(model);
        }
        public ActionResult DeleteAll()
        {
            //return RedirectToAction("Index");
            DocumentSetTemplate template = null;
            List<ContentTypeModel> model = new List<ContentTypeModel>();
            
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;

                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();
                    foreach (ContentType ct in web.ContentTypes)
                    {
                        //find out documentset and child content type
                        if (ct.Name.IndexOf("document set", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                            DocumentSetTemplate.IsChildOfDocumentSetContentType(clientContext, ct).Value)
                        {
                            template = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, ct);
                            clientContext.Load(template, t => t.AllowedContentTypes, t => t.DefaultDocuments, t => t.SharedFields, t => t.WelcomePageFields);
                            clientContext.ExecuteQuery();

                            foreach (ContentTypeId ctId in template.AllowedContentTypes)
                            {
                                ContentType ctAllowed = clientContext.Web.ContentTypes.First(d => d.StringId == ctId.StringValue);
                                if (ctAllowed != null)
                                    model.Add(new ContentTypeModel() { Id = ctId, Name = ctAllowed.Name });
                            }

                            break;
                        }
                    }

                }
            }
            return View("AllowedContentTypesDeleteAll", model);
        }
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public ActionResult AllowedContentTypeDeleteAll()
        {
            DocumentSetTemplate template = null;
            List<ContentTypeModel> model = new List<ContentTypeModel>();

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;

                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();
                    foreach (ContentType ct in web.ContentTypes)
                    {
                        //find out documentset and child content type
                        if (ct.Name.IndexOf("document set", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                            DocumentSetTemplate.IsChildOfDocumentSetContentType(clientContext, ct).Value)
                        {
                            template = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, ct);
                            clientContext.Load(template, t => t.AllowedContentTypes, t => t.DefaultDocuments, t => t.SharedFields, t => t.WelcomePageFields);
                            clientContext.ExecuteQuery();

                            List<ContentTypeId> delList = new List<ContentTypeId>();
                            foreach (ContentTypeId ctId in template.AllowedContentTypes)
                            {
                                delList.Add(ctId);
                            }
                            //remove recorded content type id from allowedContentTypes list. 
                            delList.ForEach(ctId=>template.AllowedContentTypes.Remove(ctId));
                            break;
                        }
                    }

                }
            }

            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });

        }
        public ActionResult AddAllowedContentType()
        {
            //return RedirectToAction("Index");
            AddAllowedContentTypeViewModel model = new AddAllowedContentTypeViewModel();
            
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();

                    //var query = from ct in web.ContentTypes
                    //            where ct.Name == "SOWContentType"
                    //            select ct;
                    //ContentType ctSow = query.First();
                    //model = new AddAllowedContentTypeViewModel() { Id = ctSow.Id, Name = ctSow.Name };
                    List<ContentTypeModel> allContentTypes = new List<ContentTypeModel>();
                    foreach(ContentType ct in web.ContentTypes )
                    {
                        allContentTypes.Add(new ContentTypeModel() { Name = ct.Name, Id = ct.Id});
                    }
                    ViewBag.SelectedStringId = new SelectList(allContentTypes, "StringId", "Name", model.SelectedStringId);
                }
            }
            return View("AddAllowedContentType", model);
        }
        [HttpPost, ActionName("AddAllowedContentType")]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public ActionResult AddAllowedContentTypeAction([Bind(Include = "SelectedStringId")] AddAllowedContentTypeViewModel model)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;

                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();

                    var query = from ct in web.ContentTypes
                                where ct.Id.StringValue == model.SelectedStringId 
                                select ct;
                    ContentType ctFound = query.First();

                    DocumentSetTemplate template = GetDocumentSetTemplate(clientContext);
                    if(template !=null)
                    {
                        template.AllowedContentTypes.Add(ctFound.Id);
                        template.Update(true);
                        clientContext.Load(template);
                        clientContext.ExecuteQuery();
                    }
                }
            }

            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });

        }

        public ActionResult DeleteAllowedContentType(string id)
        {
            //return RedirectToAction("Index");
            ContentTypeModel model = new ContentTypeModel();

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();

                    var query = from ct in web.ContentTypes
                                where ct.Id.StringValue == id
                                select ct;
                    ContentType ctFound = query.First();
                    model.Name = ctFound.Name;
                    model.Id = ctFound.Id;
                }
            }
            return View("DeleteAllowedContentType", model);
        }
        [HttpPost, ActionName("DeleteAllowedContentType")]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public ActionResult DeleteAllowedContentTypeAction(string id)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;

                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();

                    var query = from ct in web.ContentTypes
                                where ct.Id.StringValue == id
                                select ct;
                    ContentType ctFound = query.First();

                    DocumentSetTemplate template = GetDocumentSetTemplate(clientContext);
                    if (template != null)
                    {
                        template.AllowedContentTypes.Remove(ctFound.Id);
                        template.Update(true);
                        clientContext.Load(template);
                        clientContext.ExecuteQuery();
                    }
                }
            }

            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region Private Method
        private HomeViewModel InitHomeViewModel()
        {
            DocumentSetTemplate template = null;
            HomeViewModel model = new HomeViewModel();
            User spUser = null;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(spUser, user => user.Title);
                    clientContext.ExecuteQuery();
                    ViewBag.UserName = spUser.Title;

                    Web web = clientContext.Web;

                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();
                    template = GetDocumentSetTemplate(clientContext);
                    foreach (ContentTypeId ctId in template.AllowedContentTypes)
                    {
                        ContentType ctAllowed = clientContext.Web.ContentTypes.First(d => d.StringId == ctId.StringValue);
                        if (ctAllowed != null)
                            model.AllowedContentTypes.Add(new ContentTypeModel() { Id = ctId, Name = ctAllowed.Name });
                    }

                    foreach (Field field in template.SharedFields)
                    {
                        model.SharedFields.Add(new FieldModel() { Id = field.Id, Name = field.InternalName, Type = field.TypeDisplayName });
                    }

                    foreach (Field field in template.WelcomePageFields)
                    {
                        model.WelcomeFields.Add(new FieldModel() { Id = field.Id, Name = field.InternalName, Type = field.TypeDisplayName });
                    }

                }
            }
            return model;
        }
        private DocumentSetTemplate GetDocumentSetTemplate(ClientContext clientContext)
        {
            DocumentSetTemplate template = null;
            Web web = clientContext.Web;

            clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
            clientContext.ExecuteQuery();

            foreach (ContentType ct in web.ContentTypes)
            {
                //find out documentset and child content type
                if (ct.Name.IndexOf("document set", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                    DocumentSetTemplate.IsChildOfDocumentSetContentType(clientContext, ct).Value)
                {
                    template = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, ct);
                    clientContext.Load(template, t => t.AllowedContentTypes, t => t.DefaultDocuments, t => t.SharedFields, t => t.WelcomePageFields);
                    clientContext.ExecuteQuery();

                    break;
                }
            }
            return template;

        }
        #endregion

    }
}
