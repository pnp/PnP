using ECM.DocumentSetsWeb.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.DocumentSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECM.DocumentSetsWeb.Controllers
{
    public class HomeController : Controller
    {
        #region Variable
        private const string CONTENTTYPENAME = "TestSOWContentType";
        private const string CONTENTTYPEGROUP = "SP2TestPublish1";
        private const string DOCUMENTLIBNAME = "TesDoc";
        private const string DOCUMENTSETNAME = "TesDocumentSet";
        private const string FIELDNAME = "TestField";

        #endregion
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
        public ActionResult CreateDocumentSet()
        {
            AddDocumentSetViewModel model = new AddDocumentSetViewModel()
            {
                ContentTypeName = CONTENTTYPENAME,
                DocumentLibName = DOCUMENTLIBNAME
            };
            for(int i=0;i<5;i++)
            {
                model.FieldNames.Add(String.Format("{0}{1}", FIELDNAME, i + 1));
            }
            return View("AddDocumentSet", model);
        }

        [HttpPost, ActionName("CreateDocumentSet")]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public ActionResult CreateDocumentSet([Bind(Include = "DocumentLibName,FieldNames,ContentTypeName")] AddDocumentSetViewModel model)
        {

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List<Field> fields = new List<Field>();
                foreach (string fieldName in model.FieldNames)
                {
                    Field field = CreateDocumentSetField(clientContext, fieldName);
                    fields.Add(field);
                }

                ContentType ctTestSow = CreateDocumentSetContentType(clientContext, model.ContentTypeName, fields);
                DocumentSetTemplate docSetTemplate = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, ctTestSow);
                clientContext.Load(docSetTemplate, d => d.SharedFields, d => d.WelcomePageFields);
                clientContext.ExecuteQuery();

                if (!docSetTemplate.SharedFields.Contains(fields[0]))
                {
                    docSetTemplate.SharedFields.Add(fields[0]);
                    docSetTemplate.Update(true);
                    clientContext.ExecuteQuery();
                }
                if (!docSetTemplate.WelcomePageFields.Contains(fields[0]))
                {
                    docSetTemplate.WelcomePageFields.Add(fields[0]);
                    docSetTemplate.Update(true);
                    clientContext.ExecuteQuery();
                }

                //
                List listTestDoc = GetTestDocDocumentLibrary(clientContext, ctTestSow, model.DocumentLibName);
                clientContext.Load(listTestDoc.RootFolder);
                clientContext.ExecuteQuery();

                ClientResult<string> result = DocumentSet.Create(clientContext, listTestDoc.RootFolder, DOCUMENTSETNAME, ctTestSow.Id);
                clientContext.ExecuteQuery();
            }
            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        public ActionResult AddSharedField()
        {
            AddSharedFieldViewModel model = new AddSharedFieldViewModel();

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();

                    ContentType docSetContentType = web.ContentTypes.FirstOrDefault(ct => ct.Name == CONTENTTYPENAME);
                    if (docSetContentType != null)
                    {
                        clientContext.Load(docSetContentType, ct => ct.Fields);
                        clientContext.ExecuteQuery();

                        List<FieldModel> fieldModels = new List<FieldModel>();
                        foreach(Field field in docSetContentType.Fields )
                        {
                            fieldModels.Add(new FieldModel() { Name = field.Title, Id = field.Id, Type = field.TypeAsString });
                        }
                        ViewBag.SelectedFieldId = new SelectList(fieldModels, "Id", "Name", model.SelectedFieldId);
                    }
                }
            }
            return View("AddSharedField", model);
        }

        [HttpPost, ActionName("AddSharedField")]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public ActionResult AddSharedFieldAction([Bind(Include = "SelectedFieldId")] AddSharedFieldViewModel model)
        {

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, w => w.ContentTypes);
                clientContext.ExecuteQuery();

                ContentType docSetContentType = clientContext.Web.ContentTypes.FirstOrDefault(ct => ct.Name == CONTENTTYPENAME);
                Field selectedField = clientContext.Web.Fields.GetById(model.SelectedFieldId);
                clientContext.Load(selectedField);
                clientContext.ExecuteQuery();

                if (docSetContentType != null && selectedField.ServerObjectIsNull ==false )
                {
                    DocumentSetTemplate docSetTemplate = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, docSetContentType);
                    clientContext.Load(docSetTemplate, d => d.SharedFields);
                    clientContext.ExecuteQuery();


                    if (!docSetTemplate.SharedFields.Contains(selectedField))
                    {
                        docSetTemplate.SharedFields.Add(selectedField);
                        docSetTemplate.Update(true);
                        clientContext.ExecuteQuery();
                    }
                }
            }
            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        public ActionResult DeleteSharedField(string id)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, w => w.ContentTypes);
                clientContext.ExecuteQuery();

                ContentType docSetContentType = clientContext.Web.ContentTypes.FirstOrDefault(ct => ct.Name == CONTENTTYPENAME);
                Field selectedField = clientContext.Web.Fields.GetById(new Guid(id));
                clientContext.Load(selectedField);
                clientContext.ExecuteQuery();

                if (docSetContentType != null && selectedField.ServerObjectIsNull == false)
                {
                    DocumentSetTemplate docSetTemplate = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, docSetContentType);
                    clientContext.Load(docSetTemplate, d => d.SharedFields);
                    clientContext.ExecuteQuery();


                    if (!docSetTemplate.SharedFields.Contains(selectedField))
                    {
                        docSetTemplate.SharedFields.Remove(selectedField);
                        docSetTemplate.Update(true);
                        clientContext.ExecuteQuery();
                    }
                }
            }

            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        public ActionResult AddWelcomeField()
        {
            AddWelcomeFieldViewModel model = new AddWelcomeFieldViewModel();

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
                    clientContext.ExecuteQuery();

                    ContentType docSetContentType = web.ContentTypes.FirstOrDefault(ct => ct.Name == CONTENTTYPENAME);
                    if (docSetContentType != null)
                    {
                        clientContext.Load(docSetContentType, ct => ct.Fields);
                        clientContext.ExecuteQuery();

                        List<FieldModel> fieldModels = new List<FieldModel>();
                        foreach (Field field in docSetContentType.Fields)
                        {
                            fieldModels.Add(new FieldModel() { Name = field.Title, Id = field.Id, Type = field.TypeAsString });
                        }
                        ViewBag.SelectedFieldId = new SelectList(fieldModels, "Id", "Name", model.SelectedFieldId);
                    }
                }
            }
            return View("AddWelcomeField", model);
        }

        [HttpPost, ActionName("AddWelcomeField")]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public ActionResult AddWelcomeFieldAction([Bind(Include = "SelectedFieldId")] AddWelcomeFieldViewModel model)
        {

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, w => w.ContentTypes);
                clientContext.ExecuteQuery();

                ContentType docSetContentType = clientContext.Web.ContentTypes.FirstOrDefault(ct => ct.Name == CONTENTTYPENAME);
                Field selectedField = clientContext.Web.Fields.GetById(model.SelectedFieldId);
                clientContext.Load(selectedField);
                clientContext.ExecuteQuery();

                if (docSetContentType != null && selectedField.ServerObjectIsNull == false)
                {
                    DocumentSetTemplate docSetTemplate = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, docSetContentType);
                    clientContext.Load(docSetTemplate, d => d.WelcomePageFields);
                    clientContext.ExecuteQuery();


                    if (!docSetTemplate.WelcomePageFields.Contains(selectedField))
                    {
                        docSetTemplate.WelcomePageFields.Add(selectedField);
                        docSetTemplate.Update(true);
                        clientContext.ExecuteQuery();
                    }
                }
            }
            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        public ActionResult DeleteWelcomeField(string id)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, w => w.ContentTypes);
                clientContext.ExecuteQuery();

                ContentType docSetContentType = clientContext.Web.ContentTypes.FirstOrDefault(ct => ct.Name == CONTENTTYPENAME);
                Field selectedField = clientContext.Web.Fields.GetById(new Guid(id));
                clientContext.Load(selectedField);
                clientContext.ExecuteQuery();

                if (docSetContentType != null && selectedField.ServerObjectIsNull == false)
                {
                    DocumentSetTemplate docSetTemplate = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, docSetContentType);
                    clientContext.Load(docSetTemplate, d => d.WelcomePageFields);
                    clientContext.ExecuteQuery();


                    if (!docSetTemplate.WelcomePageFields.Contains(selectedField))
                    {
                        docSetTemplate.WelcomePageFields.Remove(new Guid(id));
                        docSetTemplate.Update(true);
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

                    ContentType ctSow = web.ContentTypes.FirstOrDefault(ct => ct.Name == CONTENTTYPENAME);
                    if (ctSow != null)// && DocumentSetTemplate.IsChildOfDocumentSetContentType(clientContext, ctSow).Value)
                    {
                        template = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, ctSow);
                        clientContext.Load(template, t => t.AllowedContentTypes, t => t.DefaultDocuments, t => t.SharedFields, t => t.WelcomePageFields);
                        clientContext.ExecuteQuery();

                        model.DocSetContentType = ctSow;
                        //template = GetDocumentSetTemplate(clientContext);
                        if (template != null)
                        {
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

            ContentType ctSow = web.ContentTypes.FirstOrDefault(ct => ct.Name == CONTENTTYPENAME);
            if (ctSow != null && DocumentSetTemplate.IsChildOfDocumentSetContentType(clientContext, ctSow).Value)
            {
                template = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, ctSow);
                clientContext.Load(template, t => t.AllowedContentTypes, t => t.DefaultDocuments, t => t.SharedFields, t => t.WelcomePageFields);
                clientContext.ExecuteQuery();
            }

            //foreach (ContentType ct in web.ContentTypes)
            //{
            //    //find out documentset and child content type
            //    if (ct.Name.IndexOf("document set", StringComparison.CurrentCultureIgnoreCase) != -1 ||
            //        DocumentSetTemplate.IsChildOfDocumentSetContentType(clientContext, ct).Value)
            //    {
            //        template = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, ct);
            //        clientContext.Load(template, t => t.AllowedContentTypes, t => t.DefaultDocuments, t => t.SharedFields, t => t.WelcomePageFields);
            //        clientContext.ExecuteQuery();

            //        break;
            //    }
            //}
            return template;

        }

        private ContentType CreateDocumentSetContentType(ClientContext clientContext, string contentTypeName, List<Field> fields)
        {
            ContentType ret = null;
            Web web = clientContext.Web;
            clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
            clientContext.ExecuteQuery();

            //Get default document set content type.
            ContentType ctDocumentSet = web.ContentTypes.FirstOrDefault(ct => ct.Name.IndexOf("document set", StringComparison.CurrentCultureIgnoreCase) != -1);
            ret = web.ContentTypes.FirstOrDefault(ct => ct.Name == contentTypeName);
            if (ret == null)
            {
                ContentTypeCreationInformation ctNewInfo = new ContentTypeCreationInformation();
                ctNewInfo.Name = CONTENTTYPENAME;
                ctNewInfo.Description = CONTENTTYPENAME;
                ctNewInfo.Group = CONTENTTYPEGROUP;
                ctNewInfo.ParentContentType = ctDocumentSet;
                ret = web.ContentTypes.Add(ctNewInfo);
            }
            clientContext.Load(ret, ct => ct.FieldLinks, ct => ct.Id);
            clientContext.ExecuteQuery();
            foreach (Field field in fields)
            {
                FieldLink flAuthor = ret.FieldLinks.FirstOrDefault(link => link.Name == field.InternalName);
                if (flAuthor == null)
                {
                    FieldLinkCreationInformation fNewInfo = new FieldLinkCreationInformation();
                    fNewInfo.Field = field;
                    ret.FieldLinks.Add(fNewInfo);
                    ret.Update(true);
                    clientContext.Load(ret, ct => ct.FieldLinks);
                    clientContext.ExecuteQuery();
                }
            }
            return ret;

        }
        private List GetTestDocDocumentLibrary(ClientContext clientContext, ContentType ct, string documentLibName)
        {
            List ret = null;
            //List listDoc = clientContext.Web.Lists.GetByTitle(documentLibName);
            clientContext.Load(clientContext.Web, web => web.Lists);
            clientContext.ExecuteQuery();
            List listDoc = clientContext.Web.Lists.FirstOrDefault(l => l.Title == documentLibName);

            if (listDoc == null)
            {
                ListCreationInformation listNew = new ListCreationInformation();
                listNew.Title = documentLibName;
                listNew.TemplateType = 101;
                ret = clientContext.Web.Lists.Add(listNew);
                clientContext.Load(ret);
                clientContext.ExecuteQuery();
            }
            else
                ret = listDoc;

            ret.ContentTypesEnabled = true;
            ret.ContentTypes.AddExistingContentType(ct);
            ret.EnableFolderCreation = false;
            ret.Update();
            clientContext.ExecuteQuery();

            return ret;
        }
        private Field CreateDocumentSetField(ClientContext clientContext, string fieldName)
        {
            clientContext.Load(clientContext.Web, web => web.Fields);
            clientContext.ExecuteQuery();

            Field sowNumber = clientContext.Web.Fields.FirstOrDefault(f => f.Title == fieldName);

            if (sowNumber == null)
            {
                sowNumber = clientContext.Web.Fields.AddFieldAsXml(String.Format("<Field DisplayName='{0}' Name='{0}' ID='{{{1}}}' Group='SP2TestPublish1Columns' Type='Text' />", fieldName, Guid.NewGuid().ToString().ToLower()),
                    false,
                    AddFieldOptions.AddFieldInternalNameHint);
                //clientContext.ExecuteQuery();
            }
            clientContext.Load(sowNumber);
            clientContext.ExecuteQuery();

            return sowNumber;
        }
        #endregion

    }
}
