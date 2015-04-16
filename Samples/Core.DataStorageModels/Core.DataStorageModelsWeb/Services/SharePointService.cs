using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Core.DataStorageModelsWeb.Services
{
    public class SharePointService
    {
        public SharePointContext SharePointContext { get; private set; }

        public SharePointService(SharePointContext sharePointContext)
        {
            this.SharePointContext = sharePointContext;
        }

        public string Deploy()
        {
            var messageBuilder = new StringBuilder();

            using (var clientContext = SharePointContext.CreateUserClientContextForSPAppWeb())
            {
                CreateAppWebNoteList(clientContext);
                messageBuilder.AppendLine("App Web Notes List successfully created.");
            }

            using (var clientContext = SharePointContext.CreateUserClientContextForSPHost())
            {
                CreateHostWebSupportCasesList(clientContext);
                messageBuilder.AppendLine("Host Web Support Cases List successfully created.");

                AddDemoDataToSupportCasesList(clientContext);
                messageBuilder.AppendLine("Successfully added three sample list items to the Host Web Support Case List.");

                CreateSupportCasePageLayoutAndPage(clientContext, SharePointContext.SPHostUrl.ToString());
                messageBuilder.AppendLine("Support Cases Page Layout, Page, CBS Display Template JavaScript files successfully created and deployed.");
            }

            messageBuilder.AppendLine("Deployment successfully completed.  See the Explore The Sample section below for next steps. ");

            return messageBuilder.ToString();
        }

        public string FillAppWebNotesListToThreshold()
        {
            using (var clientContext = SharePointContext.CreateUserClientContextForSPAppWeb())
            {
                List notesList = clientContext.Web.Lists.GetByTitle("Notes");

                var itemCreateInfo = new ListItemCreationInformation();
                for (int i = 0; i < 500; i++)
                {
                    ListItem newItem = notesList.AddItem(itemCreateInfo);
                    newItem["Title"] = "Notes Title." + i.ToString();
                    newItem["FTCAM_Description"] = "Notes description";
                    newItem.Update();
                    if (i % 100 == 0)
                        clientContext.ExecuteQuery();
                }
                clientContext.ExecuteQuery();

                clientContext.Load(notesList, l => l.ItemCount);
                clientContext.ExecuteQuery();

                if (notesList.ItemCount >= 5000)
                    return "The App Web Notes List has " + notesList.ItemCount + " items, and exceeds the threshold.";
                else
                    return 500 + " items have been added to the App Web Notes List. " +
                                   "There are " + (5000-notesList.ItemCount) + " items left to add.";          
            }
        }

        public string FillHostWebSupportCasesToThreshold()
        {
            using (var clientContext = SharePointContext.CreateUserClientContextForSPHost())
            {
                List supportCasesList = clientContext.Web.Lists.GetByTitle("Support Cases");
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                for (int i = 0; i < 500; i++)
                {
                    ListItem newItem = supportCasesList.AddItem(itemCreateInfo);
                    newItem["Title"] = "Wrong product received." + i.ToString();
                    newItem["FTCAM_Status"] = "Open";
                    newItem["FTCAM_CSR"] = "bjones";
                    newItem["FTCAM_CustomerID"] = "thresholds test";
                    newItem.Update();
                    if (i % 100 == 0)
                        clientContext.ExecuteQuery();
                }
                clientContext.ExecuteQuery();

               
                clientContext.Load(supportCasesList, l => l.ItemCount);
                clientContext.ExecuteQuery();

                if(supportCasesList.ItemCount>=5000)
                    return "The Host Web Support Cases List has " + supportCasesList.ItemCount + " items, and exceeds the threshold.";
                else
                    return 500 + " items have been added to the Host Web Support Cases List. " +
                     "There are " + (5000 - supportCasesList.ItemCount) + " items left to add.";    
            }
        }

        public int FillAppWebNotesWith1G()
        {
            using (var ctx = SharePointContext.CreateUserClientContextForSPAppWeb())
            {
                List notesList = ctx.Web.Lists.GetByTitle("Notes");
                int maxNum = 100;
                for (int i = 0; i < maxNum; i++)
                {
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem newItem = notesList.AddItem(itemCreateInfo);
                    newItem["Title"] = "Note " + i.ToString() + ".";
                    newItem["FTCAM_Description"] = "Description " + i.ToString() + ".";
                    newItem.Update();
                    ctx.ExecuteQuery();

                    var file = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Assets/SampleData.rar");
                    var filestream = System.IO.File.OpenRead(file);
                    var info = new AttachmentCreationInformation
                    {
                        FileName = "SampleData.rar",
                        ContentStream = filestream
                    };
                    newItem.AttachmentFiles.Add(info);
                    ctx.ExecuteQuery();
                    filestream.Close();
                }
                return maxNum;
            }
        }

        public void UninstallTheApp()
        {
            Guid appInstanceId;
            using (var ctx = SharePointContext.CreateUserClientContextForSPAppWeb())
            {
                Web web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQuery();

                appInstanceId = web.AppInstanceId;
            }

            using (var hostCtx = SharePointContext.CreateUserClientContextForSPHost())
            {
                AppInstance test = hostCtx.Web.GetAppInstanceById(appInstanceId);
                hostCtx.Load(test);

                ClientResult<Guid> uninstallResult = test.Uninstall();
                hostCtx.ExecuteQuery();
            }
        }

        public User GetCurrentUser()
        {
            using (var clientContext = SharePointContext.CreateUserClientContextForSPHost())
            {
                User currentUser = clientContext.Web.CurrentUser;
                clientContext.Load(currentUser);
                clientContext.ExecuteQuery();
                return currentUser;
            }
        }

        //Deploy the App Web Notes List
        private void CreateAppWebNoteList(ClientContext clientContext)
        {
            ListCollection listCollection = clientContext.Web.Lists;
            clientContext.Load(listCollection, lists => lists.Include(list => list.Title)
                .Where(list => list.Title == "Notes"));
            clientContext.ExecuteQuery();

            if (listCollection.Count == 0)
            {
                List notelist = Util.CreateList(clientContext, (int)ListTemplateType.GenericList,
                                                  "Notes", "Lists/Notes", QuickLaunchOptions.On);

                FieldCollection ff = clientContext.Web.Fields;
                clientContext.Load(ff);
                clientContext.ExecuteQuery();
                bool bexist = false;
                foreach (Field field in ff)
                {
                    if (field.StaticName == "FTCAM_Description")
                    {
                        bexist = true;
                        notelist.Fields.Add(field);
                        break;
                    }
                }
                if (!bexist)
                {
                    Field field = clientContext.Web.Fields.AddFieldAsXml("<Field Type='Note'  DisplayName='Description' StaticName='FTCAM_Description' Name='FTCAM_Description' Group='FTC_TO_AM'></Field>", true, AddFieldOptions.DefaultValue);
                    notelist.Fields.Add(field);
                }
                clientContext.ExecuteQuery();
            }
        }

        //Deploy the Host Web Support Cases List
        private void CreateHostWebSupportCasesList(ClientContext clientContext)
        {
            ListCollection listCollection = clientContext.Web.Lists;
            clientContext.Load(listCollection, lists => lists.Include(list => list.Title)
                .Where(list => list.Title == "Support Cases"));
            clientContext.ExecuteQuery();

            if (listCollection.Count == 0)
            {
                List surveylist = Util.CreateList(clientContext, (int)ListTemplateType.GenericList,
                                                      "Support Cases", "Lists/SupportCases", QuickLaunchOptions.On);

                ContentTypeCollection ff = clientContext.Web.ContentTypes;
                clientContext.Load(ff);
                clientContext.ExecuteQuery();

                bool bexist = false;
                foreach (ContentType ctype in ff)
                {
                    if (ctype.StringId == Util.SupportCaseCtyeId)
                    {
                        bexist = true;
                        surveylist.ContentTypes.AddExistingContentType(ctype);
                        clientContext.ExecuteQuery();

                        break;
                    }
                }

                if (!bexist)
                {
                    //Create content type for Support Cases list
                    ContentType ctype = Util.CreateContentType(clientContext, Util.SupportCaseCtypeName, "FTC_TO_AM", Util.SupportCaseCtyeId);

                    //Create Files
                    List<string> fieldsList = new List<string>();
                    fieldsList.Add("<Field Type='Text'  DisplayName='Status' Name='FTCAM_Status' Group='FTC_TO_AM' ></Field>");
                    fieldsList.Add("<Field Type='Text'  DisplayName='CSR' Name='FTCAM_CSR' Group='FTC_TO_AM'></Field>");
                    fieldsList.Add("<Field Type='Text'  DisplayName='Customer ID' Name='FTCAM_CustomerID' Group='FTC_TO_AM'></Field>");
                    //Bind Field to ctype
                    foreach (string str in fieldsList)
                    {
                        Field field = clientContext.Web.Fields.AddFieldAsXml(str, true, AddFieldOptions.DefaultValue);
                        clientContext.ExecuteQuery();

                        FieldLinkCreationInformation fieldLink = new FieldLinkCreationInformation();
                        fieldLink.Field = field;
                        ctype.FieldLinks.Add(fieldLink);
                        ctype.Update(true);

                        clientContext.ExecuteQuery();
                    }
                    surveylist.ContentTypes.AddExistingContentType(ctype);
                    clientContext.ExecuteQuery();
                }

                //Delete the item content type
                ContentTypeCollection ctycollection = surveylist.ContentTypes;
                clientContext.Load(ctycollection);
                clientContext.ExecuteQuery();

                ContentType defualtitemcty = ctycollection.Where(c => c.Name == "Item").FirstOrDefault();
                defualtitemcty.DeleteObject();
                clientContext.ExecuteQuery();
            }
        }

        private void AddDemoDataToSupportCasesList(ClientContext clientContext)
        {
            List supportcaseslist = clientContext.Web.Lists.GetByTitle("Support Cases");
            Util.AddDemoDataToSupportCasesList(clientContext, supportcaseslist, "Wrong product received.", "Open", "bjones", "ALFKI");
            Util.AddDemoDataToSupportCasesList(clientContext, supportcaseslist, "Purple thing is green but doesn't work.", "Resolved", "ismith", "ALFKI");
            Util.AddDemoDataToSupportCasesList(clientContext, supportcaseslist, "Purple thing should be green.", "Resolved", "ismith", "ALFKI");
            Util.AddDemoDataToSupportCasesList(clientContext, supportcaseslist, "Product was damaged upon arrival.", "Resolved", "tbag", "ALFKI");
            Util.AddDemoDataToSupportCasesList(clientContext, supportcaseslist, "Not all parts were included in shipment.", "Resolved", "sherm", "ALFKI");
            Util.AddDemoDataToSupportCasesList(clientContext, supportcaseslist, "User manual not included in shipment.", "Resolved", "cloris", "ALFKI");
            Util.AddDemoDataToSupportCasesList(clientContext, supportcaseslist, "Order never received.", "Resolved", "tyler", "ALFKI");
        }

        private void CreateSupportCasePageLayoutAndPage(ClientContext clientContext, string spHostUrl)
        {
            List masterpagelist = clientContext.Web.Lists.GetByTitle("Master Page Gallery");
            clientContext.Load(masterpagelist.RootFolder, l => l.ServerRelativeUrl);
            clientContext.ExecuteQuery();
            Util.UploadItemTemplateJS(clientContext, masterpagelist,
                                        masterpagelist.RootFolder.ServerRelativeUrl,
                                        "SupportCaseListItemDisplay.js"
                                        );
            Util.UploadControlTemplateJS(clientContext, masterpagelist,
                                           masterpagelist.RootFolder.ServerRelativeUrl,
                                           "SupportCaseListControlDisplay.js");

            Util.UploadPageLayout(clientContext, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Assets/SupportCasesPageLayout.aspx",
                                  "Master Page Gallery", "SupportCasesPageLayout.aspx");

            var Request = HttpContext.Current.Request;
            string url = Request.Url.ToString().Substring(0, Request.Url.ToString().IndexOf(Request.Url.AbsolutePath.ToString()));
            string queryurl = "?SPHostUrl=" + HttpUtility.UrlEncode(spHostUrl);

            Util.CreatePublishingPage(clientContext, "SupportCasesPage", "SupportCasesPageLayout", url, queryurl);
        }
    }
}