using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using System.Web.Hosting;
using Microsoft.SharePoint.Client.WebParts;
using System.Text;

namespace Branding.DisplayTemplatesWeb
{
    public partial class Default : System.Web.UI.Page
    {
        #region properties
        private static string ContentTypeID = "0x010048017A06020440BE8498BB193B944C84";
        #endregion

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
        }

        protected void btnIniSiteContent_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);               
                clientContext.ExecuteQuery();
                string serverRelativeURL = web.ServerRelativeUrl;

                Web parentWeb = clientContext.Site.RootWeb;
                clientContext.Load(parentWeb, w => w.Url);
                clientContext.ExecuteQuery();
                string parentWebServerRelativeURL = parentWeb.Url;

                //Create hero list
                string listId = CreateList(clientContext, web);
                //Create content type for hero list
                string contentTypeId = CreateContentType(clientContext, web);
                //Create fields for hero list
                CreateFields(clientContext, web);
                //Bind fields to content type
                BindFieldsToContentType(clientContext, web, contentTypeId);
                //Bind content type to hero list
                AddContentTypeToList(clientContext, web, contentTypeId, listId);
                //Upload control template JS and item template JS to master page gallery.
                List masterPageList = web.Lists.GetByTitle("Master Page Gallery");
                //Upload master pages
                var masterpageContentTypeId = GetContentType(clientContext, masterPageList, "Master Page");
                UploadMasterpages(clientContext, web, masterPageList, masterpageContentTypeId, "Desktop.master", serverRelativeURL);
                UploadMasterpages(clientContext, web, masterPageList, masterpageContentTypeId, "iPad.master", serverRelativeURL);
                UploadMasterpages(clientContext, web, masterPageList, masterpageContentTypeId, "iPhone.master", serverRelativeURL);
                //Upload Control and Item Template JavaScript files
                var controlContentTypeId = GetContentType(clientContext, masterPageList, "Display Template Code");
                UploadControlTemplateJS(clientContext, web, masterPageList, controlContentTypeId, "HomePageHeroControlSlideshow.js", serverRelativeURL);
                UploadControlTemplateJS(clientContext, web, masterPageList, controlContentTypeId, "HomePageHeroControlSlideshow_rwd.js", serverRelativeURL);
                UploadControlTemplateJS(clientContext, web, masterPageList, controlContentTypeId, "HomePageHeroControlSlideshow_channel.js", serverRelativeURL);
                UploadItemTemplateJS(clientContext, web, masterPageList, controlContentTypeId, serverRelativeURL);
                //Create Pages
                CreatePage(clientContext, web, "desktop.aspx", "HeroWebpart_Desktop.txt", serverRelativeURL);
                CreatePage(clientContext, web, "rwd.aspx", "HeroWebpart_RWD.txt", serverRelativeURL);
                CreatePage(clientContext, web, "channels.aspx", "HeroWebpart_channel.txt", serverRelativeURL);
                //Create demo data including slider images and hero list data.
                IniListData(clientContext, web, serverRelativeURL);

                lblInfo.Text = "The deployment operations have successfully completed. Go to the <a href='" + parentWebServerRelativeURL + "/Pages'>Pages Library </a> to view the pages.  See the scenario documentation for instructions which describe each page and how to access the Device Channels with the channels.aspx page.";
            }
        }

        public void ActivateFeature(ClientContext clientContext, Web web, Guid featureId, bool force, FeatureDefinitionScope featdefScope)
        {
            var features = web.Features;
            clientContext.Load(features);
            clientContext.ExecuteQuery();

            features.Add(featureId, force, featdefScope);
            clientContext.ExecuteQuery();
        }

        private void UploadMasterpages(ClientContext clientContext, Web web, List list, string controlContentTypeId, string masterPageURL, string serverRelatedURL)
        {
            string fileURL = string.Format("Masterpages/{0}", masterPageURL);
            string remoteFileURL = string.Format("{0}/_catalogs/masterpage/{1}", serverRelatedURL, masterPageURL);

            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(string.Format("~/{0}", fileURL)));
            newFile.Url = remoteFileURL;
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = list.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();

            var listItem = uploadFile.ListItemAllFields;
            if (uploadFile.CheckOutType == CheckOutType.None)
            {
                uploadFile.CheckOut();
            }

            listItem["ContentTypeId"] = controlContentTypeId;
            listItem["UIVersion"] = Convert.ToString(15);
            listItem.Update();
            uploadFile.CheckIn("", CheckinType.MajorCheckIn);
            listItem.File.Publish("");
            clientContext.Load(listItem);
            clientContext.ExecuteQuery();
        }

        private void CreatePage(ClientContext clientContext, Web web, string pageName, string templateName, string serverRelatedURL)
        {
            var sitePageLibrary = web.Lists.GetByTitle("Pages");
            clientContext.Load(sitePageLibrary);
            clientContext.ExecuteQuery();

            FileCreationInformation newPage = new FileCreationInformation();
            newPage.Url = pageName;
            newPage.Overwrite = true;
            System.IO.StreamReader pageTemplate = new System.IO.StreamReader(HostingEnvironment.MapPath(string.Format("~/{0}", "Templates/PageTemplate.txt")));
            string pageContent = pageTemplate.ReadToEnd();
            pageTemplate.Close();
            pageContent = string.Format(pageContent, serverRelatedURL, pageName);

            newPage.Content = System.Text.Encoding.ASCII.GetBytes(pageContent);
            File page = sitePageLibrary.RootFolder.Files.Add(newPage);
            clientContext.Load(page);
            clientContext.ExecuteQuery();

            LimitedWebPartManager lwp = page.GetLimitedWebPartManager(PersonalizationScope.Shared);
            System.IO.StreamReader webpartTemplate = new System.IO.StreamReader(HostingEnvironment.MapPath(string.Format("~/Templates/{0}", templateName)));
            string webpartContent = webpartTemplate.ReadToEnd();
            webpartTemplate.Close();

            WebPartDefinition wpd = lwp.ImportWebPart(webpartContent);
            WebPartDefinition wpdNew = lwp.AddWebPart(wpd.WebPart, "MainZone", 0);

            clientContext.Load(wpdNew);
            clientContext.ExecuteQuery();
            try
            {
                page = web.GetFileByServerRelativeUrl(page.ServerRelativeUrl);
                clientContext.Load(page);
                clientContext.ExecuteQuery();
                page.CheckIn("", CheckinType.MajorCheckIn);
                clientContext.ExecuteQuery();
            }
            catch
            {

            }
        }

        private void UploadControlTemplateJS(ClientContext clientContext, Web web, List list, string controlContentTypeId, string jsURL, string serverRelatedURL)
        {
            string fileURL = string.Format("Scripts/{0}", jsURL);
            string romoteFileURL = string.Format("{0}/_catalogs/masterpage/Display%20Templates/Content%20Web%20Parts/{1}", serverRelatedURL, jsURL);

            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(string.Format("~/{0}", fileURL)));
            newFile.Url = romoteFileURL;
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = list.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();

            var listItem = uploadFile.ListItemAllFields;
            if (uploadFile.CheckOutType == CheckOutType.None)
            {
                uploadFile.CheckOut();
            }
            listItem["Title"] = jsURL.Split('.')[0];
            listItem["ContentTypeId"] = controlContentTypeId;
            listItem["DisplayTemplateLevel"] = "Control";
            listItem["TargetControlType"] = ";#Content Web Parts;#";
            listItem["DisplayTemplateLevel"] = "Control";
            listItem["TemplateHidden"] = "0";
            listItem["UIVersion"] = "15";
            listItem.Update();
            uploadFile.CheckIn("", CheckinType.MajorCheckIn);
            clientContext.Load(listItem);
            clientContext.ExecuteQuery();
        }

        private void UploadItemTemplateJS(ClientContext clientContext, Web web, List list, string controlContentTypeId, string serverRelatedURL)
        {
            string fileURL = "Scripts/HomePageHeroItemTemplate.js";
            string romoteFileURL = string.Format("{0}/_catalogs/masterpage/Display%20Templates/Content%20Web%20Parts/HomePageHeroItemTemplate.js", serverRelatedURL);
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(string.Format("~/{0}", fileURL)));
            newFile.Url = romoteFileURL;
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = list.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();

            var listItem = uploadFile.ListItemAllFields;
            if (uploadFile.CheckOutType == CheckOutType.None)
            {
                uploadFile.CheckOut();
            }

            listItem["ContentTypeId"] = controlContentTypeId;
            listItem["DisplayTemplateLevel"] = "Control";
            listItem["TargetControlType"] = ";#Content Web Parts;#";
            listItem["DisplayTemplateLevel"] = "Item";
            listItem["TemplateHidden"] = "0";
            listItem["UIVersion"] = "15";
            listItem["ManagedPropertyMapping"] = "'Title':'Title','Tag Line':'brandingTagLineOWSTEXT', 'Left Caption Background Color'{Left Caption Background Color}:'brandingLeftCaptionBGColorOWSTEXT',  'Left Caption Background Opacity'{Left Caption Background Opacity}:'brandingLeftCaptionBGOpacityOWSTEXT',    'HeroImage'{HeroImage}:'brandingHeroImageOWSIMGE','Hero URL Link'{Hero URL Link}:'brandingLinkURLOWSTEXT',    'Right Caption Title'{Right Caption Title}:'brandingRightCaptionTitleOWSTEXT',    'Right Caption Description'{Right Caption Description}:'brandingRightCaptionDescriptionOWSMTXT',    'Sort Order'{Sort Order}:'brandingSortOrderOWSNMBR'";
            listItem.Update();
            uploadFile.CheckIn("", CheckinType.MajorCheckIn);
            clientContext.Load(listItem);
            clientContext.ExecuteQuery();
        }

        private string GetContentType(ClientContext clientContext, List list, string contentType)
        {
            ContentTypeCollection collection = list.ContentTypes;
            clientContext.Load(collection);
            clientContext.ExecuteQuery();
            var ct = collection.Where(c => c.Name == contentType).FirstOrDefault();
            string contentTypeID = "";
            if (ct != null)
            {
                contentTypeID = ct.StringId;
            }

            return contentTypeID;
        }

        private void IniListData(ClientContext clientContext, Web web, string serverRelatedURL)
        {
            UploadImagesToDocumentLibrary(web, HostingEnvironment.MapPath(string.Format("~/{0}", "Images/example-slide-1.jpg")));
            UploadImagesToDocumentLibrary(web, HostingEnvironment.MapPath(string.Format("~/{0}", "Images/example-slide-2.jpg")));
            UploadImagesToDocumentLibrary(web, HostingEnvironment.MapPath(string.Format("~/{0}", "Images/example-slide-3.jpg")));
            UploadImagesToDocumentLibrary(web, HostingEnvironment.MapPath(string.Format("~/{0}", "Images/example-slide-4.jpg")));
            AddItemsToHeroList(web, "example-slide-1", string.Format("{0}/Shared%20Documents/example-slide-1.jpg", serverRelatedURL), "f3f3f3", "0.5", "#", "This is the left caption description 1", "Right caption title 1", "1", "Tag line 1");
            AddItemsToHeroList(web, "example-slide-2", string.Format("{0}/Shared%20Documents/example-slide-2.jpg", serverRelatedURL), "f3f3f3", "0.5", "#", "This is the left caption description 2", "Right caption title 2", "2", "Tag line 2");
            AddItemsToHeroList(web, "example-slide-3", string.Format("{0}/Shared%20Documents/example-slide-3.jpg", serverRelatedURL), "666666", "0.5", "#", "This is the left caption description 3", "Right caption title 3", "3", "Tag line 3");
            AddItemsToHeroList(web, "example-slide-4", string.Format("{0}/Shared%20Documents/example-slide-4.jpg", serverRelatedURL), "666666", "0.5", "#", "This is the left caption description 4", "Right caption title 4", "4", "Tag line 4");

            UploadFilesForHeroControl(clientContext, web, serverRelatedURL);
        }

        /// <summary>
        /// Create folders in style library upload images/css/js to the folders
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        private void UploadFilesForHeroControl(ClientContext clientContext, Web web, string serverRelatedURL)
        {
            //Create folders
            var list = web.Lists.GetByTitle("Style Library");
            var folder = list.RootFolder;
            clientContext.Load(folder);
            clientContext.ExecuteQuery();
            folder = folder.Folders.Add("hero");
            folder.Folders.Add("css");
            folder.Folders.Add("js");
            folder.Folders.Add("images");
            clientContext.ExecuteQuery();

            //Upload files 
            UploadFilesToLibary(clientContext, web, list, "CSS/hero_desktop.css", string.Format("{0}/Style%20Library/hero/css/hero_desktop.css", serverRelatedURL));
            UploadFilesToLibary(clientContext, web, list, "CSS/hero_rwd.css", string.Format("{0}/Style%20Library/hero/css/hero_rwd.css", serverRelatedURL));
            UploadFilesToLibary(clientContext, web, list, "CSS/hero_ipad.css", string.Format("{0}/Style%20Library/hero/css/hero_ipad.css", serverRelatedURL));
            UploadFilesToLibary(clientContext, web, list, "CSS/hero_iphone.css", string.Format("{0}/Style%20Library/hero/css/hero_iphone.css", serverRelatedURL));
            UploadFilesToLibary(clientContext, web, list, "Images/Hero-nav-control-circle-blue.png", string.Format("{0}/Style%20Library/hero/images/Hero-nav-control-circle-blue.png", serverRelatedURL));
            UploadFilesToLibary(clientContext, web, list, "Images/Hero-nav-control-circle-white.png", string.Format("{0}/Style%20Library/hero/images/Hero-nav-control-circle-white.png", serverRelatedURL));
            UploadFilesToLibary(clientContext, web, list, "Scripts/jquery-1.9.1.min.js", string.Format("{0}/Style%20Library/hero/js/jquery-1.9.1.min.js", serverRelatedURL));
        }

        private void UploadFilesToLibary(ClientContext clientContext, Web web, List list, string fileURL, string romoteFileURL)
        {
            try
            {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(string.Format("~/{0}", fileURL)));
                newFile.Url = romoteFileURL;
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File uploadFile = list.RootFolder.Files.Add(newFile);
                web.Context.Load(uploadFile);
                uploadFile.CheckIn("", CheckinType.MajorCheckIn);
                web.Context.ExecuteQuery();
            }
            catch
            {

            }
        }

        private void AddItemsToHeroList(Web web, string title, string imageUrl, string leftCaptionBackgroundColor, string leftCaptionBackgroundOpacity,
            string linkUrl, string rightCaptionDescrption, string rightCaptionTitle, string sortOrder, string tagLine)
        {
            List list = web.Lists.GetByTitle("Home Hero");
            Microsoft.SharePoint.Client.ListItem newListItem = list.AddItem(new ListItemCreationInformation());
            newListItem["Title"] = title;
            string image = string.Format("<a href='{0}'><img src='{1}' alt='this is my image'></a>", linkUrl, imageUrl);
            newListItem["branding_HeroImage"] = image;
            newListItem["branding_LeftCaptionBGColor"] = leftCaptionBackgroundColor;
            newListItem["branding_LeftCaptionBGOpacity"] = leftCaptionBackgroundOpacity;
            newListItem["branding_RightCaptionDescription"] = rightCaptionDescrption;
            newListItem["branding_RightCaptionTitle"] = rightCaptionTitle;
            newListItem["branding_SortOrder"] = sortOrder;
            newListItem["branding_TagLine"] = tagLine;
            newListItem["branding_LeftCaptionBGColor"] = leftCaptionBackgroundColor;

            newListItem.Update();

            web.Context.ExecuteQuery();
        }

        private void UploadImagesToDocumentLibrary(Web web, string fileAddress)
        {
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(fileAddress);
            newFile.Url = System.IO.Path.GetFileName(fileAddress);
            newFile.Overwrite = true;

            List documentLibrary = web.Lists.GetByTitle("Documents");
            Microsoft.SharePoint.Client.File uploadFile = documentLibrary.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();
        }

        private static void AddContentTypeToList(ClientContext clientContext, Web web, string contentTypeId, string listId)
        {
            List list = web.Lists.GetById(new Guid(listId));
            list.ContentTypesEnabled = true;
            var ct = web.ContentTypes.GetById(contentTypeId);
            clientContext.Load(ct);
            clientContext.ExecuteQuery();

            list.ContentTypes.AddExistingContentType(ct);

            clientContext.ExecuteQuery();

            List<string> fields = GetDemoFieldIds();
            foreach (string str in fields)
            {
                Field f = list.Fields.GetById(new Guid(str));
                f.SetShowInDisplayForm(true);
                f.SetShowInEditForm(true);
                f.SetShowInNewForm(true);

                f.Hidden = false;
                f.UpdateAndPushChanges(true);

            }
            clientContext.ExecuteQuery();

            DeleteDefaultContentTypeFromList(clientContext, web, list);
        }

        private static void DeleteDefaultContentTypeFromList(ClientContext clientContext, Web web, List list)
        {
            //Delete default content type            
            ContentTypeCollection collection = list.ContentTypes;
            clientContext.Load(collection);
            clientContext.ExecuteQuery();
            string contentTypeID = collection.Where(c => c.Name == "Item").FirstOrDefault().StringId;
            ContentType ct = list.ContentTypes.GetById(contentTypeID);
            ct.DeleteObject();
            clientContext.ExecuteQuery();
        }

        private void BindFieldsToContentType(ClientContext clientContext, Web web, string contentTypeId)
        {
            ContentType ct = web.ContentTypes.GetById(contentTypeId);
            clientContext.Load(ct);

            List<string> fields = GetDemoFieldIds();
            foreach (string str in fields)
            {
                FieldLinkCreationInformation fieldLink = new FieldLinkCreationInformation();
                var field = web.Fields.GetById(new Guid(str));
                fieldLink.Field = field;
                ct.FieldLinks.Add(fieldLink);
            }
            ct.Update(true);
            clientContext.ExecuteQuery();
        }

        private void CreateFields(ClientContext clientContext, Web web)
        {
            List<string> fieldsList = BuildDemoFields();
            foreach (string str in fieldsList)
            {
                Field field = web.Fields.AddFieldAsXml(str, false, AddFieldOptions.AddFieldToDefaultView);

            }

            clientContext.ExecuteQuery();
        }

        private string CreateContentType(ClientContext clientContext, Web web)
        {
            ContentTypeCollection contentTypeColl = clientContext.Web.ContentTypes;
            ContentTypeCreationInformation contentTypeCreation = new ContentTypeCreationInformation();
            contentTypeCreation.Name = "Home Hero";
            contentTypeCreation.Description = "Custom Content Type created for hero control.";
            contentTypeCreation.Group = "Branding";
            contentTypeCreation.Id = ContentTypeID;

            //Add the new content type to the collection
            ContentType ct = contentTypeColl.Add(contentTypeCreation);
            clientContext.Load(ct);
            clientContext.ExecuteQuery();
            return ct.Id.ToString();
        }

        private static List<string> BuildDemoFields()
        {
            List<string> fieldsSchemaList = new List<string>();
            fieldsSchemaList.Add("<Field Type='Text' DisplayName='Tag Line ' ID='{a2589f26-1642-41f2-a6c2-565a0d4e3a88}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_TagLine' Name='branding_TagLine' MaxLength='255' Group='Branding' Required='FALSE' Customization=''/>");
            fieldsSchemaList.Add("<Field Type='Text' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Left Caption Background Color' ID='{bcc81121-d55d-4973-baec-aaf221cfd4dc}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_LeftCaptionBGColor' Name='branding_LeftCaptionBGColor' MaxLength='255' Group='Branding' Required='TRUE' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Text' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Left Caption Background Opacity' ID='{d4cadb85-7c72-4aa7-9a19-fa42ebd96889}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_LeftCaptionBGOpacity' Name='branding_LeftCaptionBGOpacity' MaxLength='255' Group='Branding' Required='TRUE' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Image' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Hero Image' RichText='TRUE' RichTextMode='FullHtml' ID='{6ead18fe-1c31-4d83-8edc-e421db18c560}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_HeroImage' Name='branding_HeroImage' Group='Branding' Required='TRUE' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Text' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='LinkURL' ID='{3f18835a-ef65-4221-9f17-51ebe05a958d}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_LinkURL' Name='branding_LinkURL' MaxLength='255' Group='Branding' Required='FALSE' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Note' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Right Caption Description' ID='{5d50f254-0980-48a3-b5d4-cef46368226e}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_RightCaptionDescription' Name='branding_RightCaptionDescription' NumLines='3' UnlimitedLengthInDocumentLibrary='FALSE' AllowHyperlink='FALSE' RichText='FALSE' RichTextMode='Compatible' Group='Branding' Required='TRUE' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Text' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Right Caption Title' ID='{e50b4c7e-f3e2-4c00-9ea0-685e514f7a92}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_RightCaptionTitle' Name='branding_RightCaptionTitle' MaxLength='255' Group='Branding' Required='TRUE' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Number' ShowInDisplayForm='1' ShowInEditForm='1'  ShowInNewForm='True'  DisplayName='Sort Order' ID='{6b0c3485-17af-407a-90d0-63a9a4fb10e6}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_SortOrder' Name='branding_SortOrder' Min='0' Percentage='FALSE' Decimals='0' Group='Branding' Required='TRUE' Customization='' />");
            return fieldsSchemaList;
        }

        private static List<string> GetDemoFieldIds()
        {
            List<string> fieldIds = new List<string>();
            fieldIds.Add("a2589f26-1642-41f2-a6c2-565a0d4e3a88");
            fieldIds.Add("bcc81121-d55d-4973-baec-aaf221cfd4dc");
            fieldIds.Add("d4cadb85-7c72-4aa7-9a19-fa42ebd96889");
            fieldIds.Add("6ead18fe-1c31-4d83-8edc-e421db18c560");
            fieldIds.Add("3f18835a-ef65-4221-9f17-51ebe05a958d");
            fieldIds.Add("5d50f254-0980-48a3-b5d4-cef46368226e");
            fieldIds.Add("e50b4c7e-f3e2-4c00-9ea0-685e514f7a92");
            fieldIds.Add("6b0c3485-17af-407a-90d0-63a9a4fb10e6");
            return fieldIds;
        }

        protected string CreateList(ClientContext clientContext, Web web)
        {
            string listName = "Home Hero";
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = listName;
            creationInfo.TemplateType = (int)ListTemplateType.GenericList;
            List list = web.Lists.Add(creationInfo);
            list.Description = "Home Hero";
            list.Update();
            clientContext.Load(list);
            clientContext.ExecuteQuery();
            return list.Id.ToString();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                Web web = clientContext.Web;
                DeleteItems(clientContext, web);
            }
        }

        protected void DeleteItems(ClientContext clientContext, Web web)
        {
            //Delete List
            try
            {
                List list = web.Lists.GetByTitle("Home Hero");

                list.DeleteObject();
                clientContext.ExecuteQuery();
            }
            catch
            {


            }

            //Delete Content type
            try
            {
                ContentType ct = clientContext.Web.ContentTypes.GetById(ContentTypeID);

                //// Delete the content type
                ct.DeleteObject();
                clientContext.ExecuteQuery();
            }
            catch
            {

            }

            //Delete Fields
            try
            {
                List<string> fields = GetDemoFieldIds();
                foreach (string str in fields)
                {
                    Field f = web.Fields.GetById(new Guid(str));
                    f.DeleteObject();
                }
                clientContext.ExecuteQuery();
            }
            catch
            {


            }

            //Delete pages
            try
            {
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                string serverRelatedURL = web.ServerRelativeUrl;

                DeleteFiles(clientContext, web, "/Pages/desktop.aspx", serverRelatedURL);
                DeleteFiles(clientContext, web, "/Pages/channels.aspx", serverRelatedURL);
                DeleteFiles(clientContext, web, "/Pages/rwd.aspx", serverRelatedURL);
                DeleteFiles(clientContext, web, "/Style%20Library/hero/css/hero_desktop.css", serverRelatedURL);
                DeleteFiles(clientContext, web, "/Style%20Library/hero/css/hero_ipad.css", serverRelatedURL);
                DeleteFiles(clientContext, web, "/Style%20Library/hero/css/hero_iphone.css", serverRelatedURL);
                DeleteFiles(clientContext, web, "/Style%20Library/hero/css/hero_rwd.css", serverRelatedURL);
            }
            catch
            {


            }

            lblInfo.Text = "The delete operations have successfully completed. Click the Deploy button to redeploy the artifacts.";
        }

        protected void DeleteFiles(ClientContext clientContext, Web web, string filename, string serverRelatedURL)
        {
            File file = web.GetFileByServerRelativeUrl(serverRelatedURL + filename);
            clientContext.Load(file);
            file.DeleteObject();
            clientContext.ExecuteQuery();
        }
    }
}