using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Branding.ClientSideRenderingWeb
{
    public partial class Default : System.Web.UI.Page
    {
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
            string script = @"
            function chromeLoaded() {
                $('body').show();
            }

            //function callback to render chrome after SP.UI.Controls.js loads
            function renderSPChrome() {
                //Set the chrome options for launching Help, Account, and Contact pages
                var options = {
                    'appTitle': document.title,
                    'onCssLoaded': 'chromeLoaded()'
                };

                //Load the Chrome Control in the divSPChrome element of the page
                var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                chromeNavigation.setVisible(true);
            }";

            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, web => web.Url);
                clientContext.ExecuteQuery();
                link1.NavigateUrl = clientContext.Web.Url + "/" + "Lists/CSRPriorityColor/CSR%20Sample%20View.aspx";
                link2.NavigateUrl = clientContext.Web.Url + "/" + "_layouts/15/start.aspx#/Lists/CSRSubstringlongtext/CSR%20Sample%20View.aspx";
                link3.NavigateUrl = clientContext.Web.Url + "/" + "_layouts/15/start.aspx#/CSRConfidentialDocuments/Forms/CSR%20Sample%20View.aspx";
                link4.NavigateUrl = clientContext.Web.Url + "/" + "Lists/CSRTasksPercentComplete/CSR%20Sample%20View.aspx";
                link5.NavigateUrl = clientContext.Web.Url + "/" + "/Lists/CSRAccordion/CSR%20Sample%20View.aspx";
                link6.NavigateUrl = clientContext.Web.Url + "/" + "_layouts/15/start.aspx#/Lists/CSREmailRegexValidator/NewForm.aspx";
                link7.NavigateUrl = clientContext.Web.Url + "/" + "_layouts/15/start.aspx#/Lists/CSRReadonlySPControls/EditForm.aspx?ID=1";
                link8.NavigateUrl = clientContext.Web.Url + "/" + "Lists/CSRHideControls/NewForm.aspx";
            }
        }

        protected void btnCreateSamples_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, web => web.Title);
                Web rootWeb = clientContext.Site.RootWeb;

                UploadJSFiles(clientContext.Web);

                ProvisionSample1(clientContext.Web);
                ProvisionSample2(clientContext.Web);
                ProvisionSample3(clientContext.Web);
                ProvisionSample4(clientContext.Web);
                ProvisionSample5(clientContext.Web);
                ProvisionSample6(clientContext.Web);
                ProvisionSample7(clientContext.Web);
                ProvisionSample8(clientContext.Web);

                lblInfo.Text = "Provisioning operations have successfully completed.  You may now view the samples.";
            }
        }

        void ProvisionSample1(Web web)
        {
            //Delete list if it already exists
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Priority-Color"));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                existingList.DeleteObject();
                web.Context.ExecuteQuery();
            }

            //Create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = "CSR-Priority-Color";
            creationInfo.TemplateType = (int)ListTemplateType.Tasks;
            List newlist = web.Lists.Add(creationInfo);

            newlist.Update();
            web.Context.Load(newlist);
            web.Context.ExecuteQuery();

            //Add items
            Microsoft.SharePoint.Client.ListItem item1 = newlist.AddItem(new ListItemCreationInformation());
            item1["Title"] = "Task 1";
            item1["StartDate"] = "2014-1-1";
            item1["DueDate"] = "2014-2-1";
            item1["Priority"] = "(1) High";
            item1.Update();

            Microsoft.SharePoint.Client.ListItem item2 = newlist.AddItem(new ListItemCreationInformation());
            item2["Title"] = "Task 2";
            item2["StartDate"] = "2014-1-1";
            item2["DueDate"] = "2014-2-1";
            item2["Priority"] = "(2) Normal";
            item2.Update();

            Microsoft.SharePoint.Client.ListItem item3 = newlist.AddItem(new ListItemCreationInformation());
            item3["Title"] = "Task 3";
            item3["StartDate"] = "2014-1-1";
            item3["DueDate"] = "2014-2-1";
            item3["Priority"] = "(3) Low";
            item3.Update();

            //Create sample view
            ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
            sampleViewCreateInfo.Title = "CSR Sample View";
            sampleViewCreateInfo.ViewFields = new string[] { "DocIcon", "LinkTitle", "StartDate", "DueDate", "Priority" };
            sampleViewCreateInfo.SetAsDefaultView = true;
            Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
            sampleView.Update();
            web.Context.ExecuteQuery();

            web.Context.Load(newlist, l => l.DefaultViewUrl);
            web.Context.ExecuteQuery();

            //Register JS files via JSLink properties
            RegisterJStoWebPart(web, newlist.DefaultViewUrl, "~sitecollection/Style Library/JSLink-Samples/PriorityColor.js");
        }

        void ProvisionSample2(Web web)
        {
            //Delete list if it already exists
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Substring-long-text"));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                existingList.DeleteObject();
                web.Context.ExecuteQuery();
            }

            //Create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = "CSR-Substring-long-text";
            creationInfo.TemplateType = (int)ListTemplateType.Announcements;
            List newlist = web.Lists.Add(creationInfo);

            newlist.Update();
            web.Context.Load(newlist);
            web.Context.ExecuteQuery();

            //Add items
            Microsoft.SharePoint.Client.ListItem item1 = newlist.AddItem(new ListItemCreationInformation());
            item1["Title"] = "Announcement 1";
            item1["Body"] = "Aaaaaa Bbbbbb Cccccc Dccccc Eeeeee Ffffff Gggggg Hhhhhh Iiiiii Jjjjjj Kkkkkk Llllll Mmmmmm Nnnnnn Oooooo Pppppp Qqqqqq Rrrrrr Ssssss Tttttt Uuuuuu Vvvvvv Wwwwww Xxxxx Yyyyyy Zzzzzz";
            item1.Update();

            Microsoft.SharePoint.Client.ListItem item2 = newlist.AddItem(new ListItemCreationInformation());
            item2["Title"] = "Announcement 2";
            item2["Body"] = "Aaaaaa Bbbbbb Cccccc Dccccc Eeeeee Ffffff Gggggg Hhhhhh Iiiiii Jjjjjj Kkkkkk Llllll Mmmmmm Nnnnnn Oooooo Pppppp Qqqqqq Rrrrrr Ssssss Tttttt Uuuuuu Vvvvvv Wwwwww Xxxxx Yyyyyy Zzzzzz";
            item2.Update();

            Microsoft.SharePoint.Client.ListItem item3 = newlist.AddItem(new ListItemCreationInformation());
            item3["Title"] = "Announcement 3";
            item3["Body"] = "Aaaaaa Bbbbbb Cccccc Dccccc Eeeeee Ffffff Gggggg Hhhhhh Iiiiii Jjjjjj Kkkkkk Llllll Mmmmmm Nnnnnn Oooooo Pppppp Qqqqqq Rrrrrr Ssssss Tttttt Uuuuuu Vvvvvv Wwwwww Xxxxx Yyyyyy Zzzzzz";
            item3.Update();

            //Create sample view
            ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
            sampleViewCreateInfo.Title = "CSR Sample View";
            sampleViewCreateInfo.ViewFields = new string[] { "Title", "Body" };
            sampleViewCreateInfo.SetAsDefaultView = true;
            Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
            sampleView.Update();
            web.Context.Load(newlist, l => l.DefaultViewUrl);
            web.Context.ExecuteQuery();

            //Register JS files via JSLink properties
            RegisterJStoWebPart(web, newlist.DefaultViewUrl, "~sitecollection/Style Library/JSLink-Samples/SubstringLongText.js");
        }

        void ProvisionSample3(Web web)
        {
            //Delete list if it already exists
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Confidential-Documents"));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                existingList.DeleteObject();
                web.Context.ExecuteQuery();
            }

            //Create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = "CSR-Confidential-Documents";
            creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
            List newlist = web.Lists.Add(creationInfo);
            newlist.Update();
            web.Context.Load(newlist);
            web.Context.Load(newlist.Fields);
            web.Context.ExecuteQuery();

            //Add field
            FieldCollection fields = web.Fields;
            web.Context.Load(fields, fc => fc.Include(f => f.InternalName));
            web.Context.ExecuteQuery();
            Field field = fields.FirstOrDefault(f => f.InternalName == "Confidential");
            if (field == null)
            {
                field = newlist.Fields.AddFieldAsXml("<Field Type=\"YES/NO\" Name=\"Confidential\" DisplayName=\"Confidential\" ID=\"" + Guid.NewGuid() + "\" Group=\"CSR Samples\" />", false, AddFieldOptions.DefaultValue);
                web.Update();
                web.Context.ExecuteQuery();
            }
            newlist.Fields.Add(field);
            newlist.Update();
            web.Context.ExecuteQuery();

            //Upload sample docs
            UploadTempDoc(newlist, "Doc1.doc");
            UploadTempDoc(newlist, "Doc2.doc");
            UploadTempDoc(newlist, "Doc3.ppt");
            UploadTempDoc(newlist, "Doc4.ppt");
            UploadTempDoc(newlist, "Doc5.xls");
            UploadTempDoc(newlist, "Doc6.xls");
            Microsoft.SharePoint.Client.ListItem item1 = newlist.GetItemById(1);
            item1["Confidential"] = 1;
            item1.Update();
            Microsoft.SharePoint.Client.ListItem item2 = newlist.GetItemById(2);
            item2["Confidential"] = 1;
            item2.Update();
            Microsoft.SharePoint.Client.ListItem item3 = newlist.GetItemById(3);
            item3["Confidential"] = 0;
            item3.Update();
            Microsoft.SharePoint.Client.ListItem item4 = newlist.GetItemById(4);
            item4["Confidential"] = 1;
            item4.Update();
            Microsoft.SharePoint.Client.ListItem item5 = newlist.GetItemById(5);
            item5["Confidential"] = 0;
            item5.Update();
            Microsoft.SharePoint.Client.ListItem item6 = newlist.GetItemById(6);
            item6["Confidential"] = 1;
            item6.Update();
            web.Context.ExecuteQuery();

            //Create sample view
            ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
            sampleViewCreateInfo.Title = "CSR Sample View";
            sampleViewCreateInfo.ViewFields = new string[] { "DocIcon", "LinkFilename", "Modified", "Editor", "Confidential" };
            sampleViewCreateInfo.SetAsDefaultView = true;
            Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
            sampleView.Update();
            web.Context.Load(newlist, l => l.DefaultViewUrl);
            web.Context.ExecuteQuery();

            //Register JS files via JSLink properties
            RegisterJStoWebPart(web, newlist.DefaultViewUrl, "~sitecollection/Style Library/JSLink-Samples/ConfidentialDocuments.js");
        }

        void ProvisionSample4(Web web)
        {
            //Delete list if it already exists
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Tasks-Percent-Complete"));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                existingList.DeleteObject();
                web.Context.ExecuteQuery();
            }

            //Create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = "CSR-Tasks-Percent-Complete";
            creationInfo.TemplateType = (int)ListTemplateType.Tasks;
            List newlist = web.Lists.Add(creationInfo);

            newlist.Update();
            web.Context.Load(newlist);
            web.Context.ExecuteQuery();

            //Add items
            Microsoft.SharePoint.Client.ListItem item1 = newlist.AddItem(new ListItemCreationInformation());
            item1["Title"] = "Task 1";
            item1["StartDate"] = "2014-1-1";
            item1["DueDate"] = "2014-2-1";
            item1["PercentComplete"] = "0.59";
            item1.Update();

            Microsoft.SharePoint.Client.ListItem item2 = newlist.AddItem(new ListItemCreationInformation());
            item2["Title"] = "Task 2";
            item2["StartDate"] = "2014-1-1";
            item2["DueDate"] = "2014-2-1";
            item2["PercentComplete"] = "0.40";
            item2.Update();

            Microsoft.SharePoint.Client.ListItem item3 = newlist.AddItem(new ListItemCreationInformation());
            item3["Title"] = "Task 3";
            item3["StartDate"] = "2014-1-1";
            item3["DueDate"] = "2014-2-1";
            item3["PercentComplete"] = "1.0";
            item3.Update();

            Microsoft.SharePoint.Client.ListItem item4 = newlist.AddItem(new ListItemCreationInformation());
            item4["Title"] = "Task 4";
            item4["StartDate"] = "2014-1-1";
            item4["DueDate"] = "2014-2-1";
            item4["PercentComplete"] = "0.26";
            item4.Update();

            Microsoft.SharePoint.Client.ListItem item5 = newlist.AddItem(new ListItemCreationInformation());
            item5["Title"] = "Task 5";
            item5["StartDate"] = "2014-1-1";
            item5["DueDate"] = "2014-2-1";
            item5["PercentComplete"] = "0.50";
            item5.Update();

            //Create sample view
            ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
            sampleViewCreateInfo.Title = "CSR Sample View";
            sampleViewCreateInfo.ViewFields = new string[] { "DocIcon", "LinkTitle", "DueDate", "AssignedTo", "PercentComplete" };
            sampleViewCreateInfo.SetAsDefaultView = true;
            Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
            sampleView.Update();
            web.Context.Load(newlist, l => l.DefaultViewUrl,
                l => l.DefaultDisplayFormUrl,
                l => l.DefaultEditFormUrl,
                l => l.DefaultNewFormUrl);
            web.Context.ExecuteQuery();

            //Register JS files via JSLink properties
            RegisterJStoWebPart(web, newlist.DefaultViewUrl, "~sitecollection/Style Library/JSLink-Samples/PercentComplete.js");
            RegisterJStoWebPart(web, newlist.DefaultDisplayFormUrl, "~sitecollection/Style Library/JSLink-Samples/PercentComplete.js");
            RegisterJStoWebPart(web, newlist.DefaultEditFormUrl, "~sitecollection/Style Library/JSLink-Samples/PercentComplete.js");
            RegisterJStoWebPart(web, newlist.DefaultNewFormUrl, "~sitecollection/Style Library/JSLink-Samples/PercentComplete.js");
        }

        void ProvisionSample5(Web web)
        {
            //Delete list if it already exists
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Accordion"));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                existingList.DeleteObject();
                web.Context.ExecuteQuery();
            }

            //Create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = "CSR-Accordion";
            creationInfo.TemplateType = (int)ListTemplateType.GenericList;
            List newlist = web.Lists.Add(creationInfo);
            newlist.Update();
            web.Context.Load(newlist);
            web.Context.ExecuteQuery();

            //Add field
            newlist.Fields.AddFieldAsXml("<Field Type=\"" + FieldType.Note + "\" Name=\"Description\" DisplayName=\"Description\" ID=\"" + Guid.NewGuid() + "\" Group=\"CSR Samples\" />", false, AddFieldOptions.DefaultValue);
            newlist.Update();
            web.Context.ExecuteQuery();

            //Add items
            Microsoft.SharePoint.Client.ListItem item1 = newlist.AddItem(new ListItemCreationInformation());
            item1["Title"] = "Accordion Item 1";
            item1["Description"] = "Accordian description 1.";
            item1.Update();

            Microsoft.SharePoint.Client.ListItem item2 = newlist.AddItem(new ListItemCreationInformation());
            item2["Title"] = "Accordion Item 2";
            item2["Description"] = "Accordian description 2. ";
            item2.Update();

            Microsoft.SharePoint.Client.ListItem item3 = newlist.AddItem(new ListItemCreationInformation());
            item3["Title"] = "Accordion Item 3";
            item3["Description"] = "Accordian description 3.";
            item3.Update();

            web.Context.ExecuteQuery();

            //Create sample view
            ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
            sampleViewCreateInfo.Title = "CSR Sample View";
            sampleViewCreateInfo.ViewFields = new string[] { "LinkTitle", "Description" };
            sampleViewCreateInfo.SetAsDefaultView = true;
            Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
            sampleView.Update();
            web.Context.Load(newlist, l => l.DefaultViewUrl);
            web.Context.ExecuteQuery();

            //Register JS files via JSLink properties
            RegisterJStoWebPart(web, newlist.DefaultViewUrl, "~sitecollection/Style Library/JSLink-Samples/Accordion.js");
        }

        void ProvisionSample6(Web web)
        {
            //Delete list if it already exists
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Email-Regex-Validator"));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                existingList.DeleteObject();
                web.Context.ExecuteQuery();
            }

            //Create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = "CSR-Email-Regex-Validator";
            creationInfo.TemplateType = (int)ListTemplateType.GenericList;
            List newlist = web.Lists.Add(creationInfo);
            newlist.Update();
            web.Context.Load(newlist);
            web.Context.ExecuteQuery();

            //Add field
            newlist.Fields.AddFieldAsXml("<Field Type=\"" + FieldType.Text + "\" Name=\"Email\" DisplayName=\"Email\" ID=\"" + Guid.NewGuid() + "\" Group=\"CSR Samples\" />", false, AddFieldOptions.DefaultValue);
            newlist.Update();
            web.Context.ExecuteQuery();

            //Add items
            Microsoft.SharePoint.Client.ListItem item1 = newlist.AddItem(new ListItemCreationInformation());
            item1["Title"] = "Email address";
            item1["Email"] = "csr@csr.com";
            item1.Update();

            //Create sample view
            ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
            sampleViewCreateInfo.Title = "CSR Sample View";
            sampleViewCreateInfo.ViewFields = new string[] { "LinkTitle", "Email" };
            sampleViewCreateInfo.SetAsDefaultView = true;
            Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
            sampleView.Update();

            web.Context.Load(newlist, l => l.DefaultNewFormUrl, l => l.DefaultEditFormUrl);
            web.Context.ExecuteQuery();

            //Register JS files via JSLink properties
            RegisterJStoWebPart(web, newlist.DefaultNewFormUrl, "~sitecollection/Style Library/JSLink-Samples/RegexValidator.js");
            RegisterJStoWebPart(web, newlist.DefaultEditFormUrl, "~sitecollection/Style Library/JSLink-Samples/RegexValidator.js");
        }

        void ProvisionSample7(Web web)
        {
            //Delete list if it already exists
            ListCollection lists = web.Lists;
            web.Context.Load(web.CurrentUser, i => i.Id);
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Read-only-SP-Controls"));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                existingList.DeleteObject();
                web.Context.ExecuteQuery();
            }

            //Create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = "CSR-Read-only-SP-Controls";
            creationInfo.TemplateType = (int)ListTemplateType.Tasks;
            List newlist = web.Lists.Add(creationInfo);

            newlist.Update();
            web.Context.Load(newlist);
            web.Context.ExecuteQuery();

            //Add items
            Microsoft.SharePoint.Client.ListItem item1 = newlist.AddItem(new ListItemCreationInformation());
            item1["Title"] = "Task 1";
            item1["StartDate"] = "2014-1-1";
            item1["DueDate"] = "2014-2-1";
            item1["AssignedTo"] = new FieldLookupValue { LookupId = web.CurrentUser.Id };
            item1.Update();


            //Create sample view
            ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
            sampleViewCreateInfo.Title = "CSR Sample View";
            sampleViewCreateInfo.ViewFields = new string[] { "DocIcon", "LinkTitle", "DueDate", "AssignedTo" };
            sampleViewCreateInfo.SetAsDefaultView = true;
            Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
            sampleView.Update();
            web.Context.Load(newlist,
                l => l.DefaultEditFormUrl);
            web.Context.ExecuteQuery();

            //Register JS files via JSLink properties
            RegisterJStoWebPart(web, newlist.DefaultEditFormUrl, "~sitecollection/Style Library/JSLink-Samples/ReadOnlySPControls.js");
        }

        void ProvisionSample8(Web web)
        {
            //Delete list if it already exists
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Hide-Controls"));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                existingList.DeleteObject();
                web.Context.ExecuteQuery();
            }

            //Create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = "CSR-Hide-Controls";
            creationInfo.TemplateType = (int)ListTemplateType.Tasks;
            List newlist = web.Lists.Add(creationInfo);

            newlist.Update();
            web.Context.Load(newlist);
            web.Context.ExecuteQuery();

            //Add items
            Microsoft.SharePoint.Client.ListItem item1 = newlist.AddItem(new ListItemCreationInformation());
            item1["Title"] = "Task 1";
            item1["StartDate"] = "2014-1-1";
            item1["DueDate"] = "2014-2-1";
            item1.Update();

            //Create sample view
            ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
            sampleViewCreateInfo.Title = "CSR Sample View";
            sampleViewCreateInfo.ViewFields = new string[] { "DocIcon", "LinkTitle", "DueDate", "AssignedTo" };
            sampleViewCreateInfo.SetAsDefaultView = true;
            Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
            sampleView.Update();
            web.Context.Load(newlist,
                l => l.DefaultEditFormUrl,
                l => l.DefaultNewFormUrl);
            web.Context.ExecuteQuery();

            //Register JS files via JSLink properties
            RegisterJStoWebPart(web, newlist.DefaultNewFormUrl, "~sitecollection/Style Library/JSLink-Samples/HiddenField.js");
            RegisterJStoWebPart(web, newlist.DefaultEditFormUrl, "~sitecollection/Style Library/JSLink-Samples/HiddenField.js");
        }

        void RegisterJStoWebPart(Web web, string url, string jsPath)
        {
            Microsoft.SharePoint.Client.File newFormPageFile = web.GetFileByServerRelativeUrl(url);
            LimitedWebPartManager limitedWebPartManager = newFormPageFile.GetLimitedWebPartManager(PersonalizationScope.Shared);
            web.Context.Load(limitedWebPartManager.WebParts);
            web.Context.ExecuteQuery();
            if (limitedWebPartManager.WebParts.Count > 0)
            {
                WebPartDefinition webPartDef = limitedWebPartManager.WebParts.FirstOrDefault();
                webPartDef.WebPart.Properties["JSLink"] = jsPath;
                webPartDef.SaveWebPartChanges();
                web.Context.ExecuteQuery();
            }
        }

        void UploadTempDoc(List list, string path)
        {
            path = Server.MapPath(path);

            if (!System.IO.File.Exists(path))
            {
                //Create a file to write to
                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine("Temp doc");
                }
            }

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                FileCreationInformation flciNewFile = new FileCreationInformation();

                flciNewFile.ContentStream = fs;
                flciNewFile.Url = System.IO.Path.GetFileName(path);
                flciNewFile.Overwrite = true;

                Microsoft.SharePoint.Client.File uploadFile = list.RootFolder.Files.Add(flciNewFile);

                list.Context.Load(uploadFile);
                list.Context.ExecuteQuery();
            }
        }

        void UploadJSFiles(Web web)
        {
            //Delete the folder if it exists
            Microsoft.SharePoint.Client.List list = web.Lists.GetByTitle("Style Library");
            IEnumerable<Folder> results = web.Context.LoadQuery<Folder>(list.RootFolder.Folders.Where(folder => folder.Name == "JSLink-Samples"));
            web.Context.ExecuteQuery();
            Folder samplesJSfolder = results.FirstOrDefault();

            if (samplesJSfolder != null)
            {
                samplesJSfolder.DeleteObject();
                web.Context.ExecuteQuery();
            }

            samplesJSfolder = list.RootFolder.Folders.Add("JSLink-Samples");
            web.Context.Load(samplesJSfolder);
            web.Context.ExecuteQuery();

            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/Accordion.js"), samplesJSfolder);
            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/ConfidentialDocuments.js"), samplesJSfolder);
            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/DisableInput.js"), samplesJSfolder);
            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/HiddenField.js"), samplesJSfolder);
            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/PercentComplete.js"), samplesJSfolder);
            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/PriorityColor.js"), samplesJSfolder);
            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/ReadOnlySPControls.js"), samplesJSfolder);
            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/RegexValidator.js"), samplesJSfolder);
            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/SubstringLongText.js"), samplesJSfolder);

            Folder imgsFolder = samplesJSfolder.Folders.Add("imgs");
            web.Context.Load(imgsFolder);
            web.Context.ExecuteQuery();

            UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/imgs/Confidential.png"), imgsFolder);
        }

        public static void UploadFileToFolder(Web web, string filePath, Folder folder)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                FileCreationInformation flciNewFile = new FileCreationInformation();

                flciNewFile.ContentStream = fs;
                flciNewFile.Url = System.IO.Path.GetFileName(filePath);
                flciNewFile.Overwrite = true;

                Microsoft.SharePoint.Client.File uploadFile = folder.Files.Add(flciNewFile);
                uploadFile.CheckIn("CSR sample js file", CheckinType.MajorCheckIn);

                folder.Context.Load(uploadFile);
                folder.Context.ExecuteQuery();
            }
        }
    }
}