using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System.Configuration;
using System.Web.Configuration;
using System.Text;
using System.IO;
using Microsoft.SharePoint.Client.Publishing;

namespace Core.DataStorageModelsWeb
{
    public class Util
    {
        public static string SupportCaseCtyeId = "0x010099F3EACDCC6ED04FA78B124A715C0D77";
        public static string SupportCaseCtypeName = "SupportCases18";
        public static void SetConfigSetting(string key, string value)
        {
            //Open the Root level web.config file.
            Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");
            //Modify the value
            webConfigApp.AppSettings.Settings[key].Value = value;
            //Save the modified value
            webConfigApp.Save();
        }

        public static string GetConfigSetting(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public static void ActivePublishingFeature(ClientContext ctx)
        {
            Guid publishingSiteFeatureId = new Guid("f6924d36-2fa8-4f0b-b16d-06b7250180fa");
            Guid publishingWebFeatureId = new Guid("94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb");

            Site clientSite = ctx.Site;
            ctx.Load(clientSite);

            FeatureCollection clientSiteFeatures = clientSite.Features;
            ctx.Load(clientSiteFeatures);

            //Activate the site feature
            clientSiteFeatures.Add(publishingSiteFeatureId, true, FeatureDefinitionScope.Farm);
            ctx.ExecuteQuery();

            FeatureCollection clientWebFeatures = ctx.Web.Features;
            ctx.Load(clientWebFeatures);

            //Activate the web feature
            clientWebFeatures.Add(publishingWebFeatureId, true, FeatureDefinitionScope.Farm);
            ctx.ExecuteQuery();
        }

        public static List CreateList(ClientContext ctx, int templateType,
                                       string title, string url, QuickLaunchOptions quickLaunchOptions)
        {
            ListCreationInformation listCreationInfo = new ListCreationInformation
            {
                TemplateType = templateType,
                Title = title,
                Url = url,
                QuickLaunchOption = quickLaunchOptions
            };
            List spList = ctx.Web.Lists.Add(listCreationInfo);
            ctx.Load(spList);
            ctx.ExecuteQuery();

            return spList;
        }

        public static ContentType CreateContentType(ClientContext ctx, string ctyName, string group, string ctyId)
        {
            ContentTypeCreationInformation contentTypeCreation = new ContentTypeCreationInformation();
            contentTypeCreation.Name = ctyName;
            contentTypeCreation.Description = "Custom Content Type";
            contentTypeCreation.Group = group;
            contentTypeCreation.Id = ctyId;

            //Add the new content type to the collection
            ContentType ct = ctx.Web.ContentTypes.Add(contentTypeCreation);
            ctx.Load(ct);
            ctx.ExecuteQuery();

            return ct;
        }

        public static void AddDemoDataToSupportCasesList(ClientContext ctx, List list, string title,
                                                       string status, string csr, string customerID)
        {
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem newItem = list.AddItem(itemCreateInfo);
            newItem["Title"] = title;
            newItem["FTCAM_Status"] = status;
            newItem["FTCAM_CSR"] = csr;
            newItem["FTCAM_CustomerID"] = customerID;
            newItem.Update();
            ctx.ExecuteQuery();
        }

        public static string ToAzureKeyString(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str
                .Where(c => c != '/'
                            && c != '\\'
                            && c != '#'
                            && c != '/'
                            && c != '?'
                            && !char.IsControl(c)))
                sb.Append(c);
            return sb.ToString();
        }

        public static void UploadPageLayout(ClientContext ctx, string sourcePath, string targetListTitle, string targetUrl)
        {
            using (FileStream fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(data, 0, data.Length);
                    var newfile = new FileCreationInformation();
                    newfile.Content = ms.ToArray();
                    newfile.Url = targetUrl;
                    newfile.Overwrite = true;

                    List docs = ctx.Web.Lists.GetByTitle(targetListTitle);
                    Microsoft.SharePoint.Client.File uploadedFile = docs.RootFolder.Files.Add(newfile);
                    uploadedFile.CheckOut();
                    uploadedFile.CheckIn("Data storage model", CheckinType.MajorCheckIn);
                    uploadedFile.Publish("Data storage model layout.");

                    ctx.Load(uploadedFile);
                    ctx.ExecuteQuery();
                }
            }
        }

        public static void CreatePublishingPage(ClientContext clientContext, string pageName, string pagelayoutname, string url, string queryurl)
        {
            var publishingPageName = pageName + ".aspx";

            Web web = clientContext.Web;
            clientContext.Load(web);

            List pages = web.Lists.GetByTitle("Pages");
            clientContext.Load(pages.RootFolder, f => f.ServerRelativeUrl);
            clientContext.ExecuteQuery();

            Microsoft.SharePoint.Client.File file =
                web.GetFileByServerRelativeUrl(pages.RootFolder.ServerRelativeUrl + "/" + pageName + ".aspx");
            clientContext.Load(file, f => f.Exists);
            clientContext.ExecuteQuery();
            if(file.Exists)
            {
                file.DeleteObject();
                clientContext.ExecuteQuery();
            }
            PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(clientContext, web);
            clientContext.Load(publishingWeb);

            if (publishingWeb != null)
            {
                List publishingLayouts = clientContext.Site.RootWeb.Lists.GetByTitle("Master Page Gallery");

                ListItemCollection allItems = publishingLayouts.GetItems(CamlQuery.CreateAllItemsQuery());
                clientContext.Load(allItems, items => items.Include(item => item.DisplayName).Where(obj => obj.DisplayName == pagelayoutname));
                clientContext.ExecuteQuery();

                ListItem layout = allItems.Where(x => x.DisplayName == pagelayoutname).FirstOrDefault();
                clientContext.Load(layout);

                PublishingPageInformation publishingpageInfo = new PublishingPageInformation()
                {
                    Name = publishingPageName,
                    PageLayoutListItem = layout,
                };

                PublishingPage publishingPage = publishingWeb.AddPublishingPage(publishingpageInfo);
                publishingPage.ListItem.File.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                publishingPage.ListItem.File.Publish(string.Empty);
                clientContext.ExecuteQuery();
            }
            SetSupportCaseContent(clientContext, "SupportCasesPage", url, queryurl);
        }

        public static void SetSupportCaseContent(ClientContext ctx, string pageName, string url, string queryurl)
        {
            List pages = ctx.Web.Lists.GetByTitle("Pages");
            ctx.Load(pages.RootFolder, f => f.ServerRelativeUrl);
            ctx.ExecuteQuery();

            Microsoft.SharePoint.Client.File file =
                ctx.Web.GetFileByServerRelativeUrl(pages.RootFolder.ServerRelativeUrl + "/" + pageName + ".aspx");
            ctx.Load(file);
            ctx.ExecuteQuery();

            file.CheckOut();

            LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            string quicklaunchmenuFormat =
                @"<div><a href='{0}/{1}'>Sample Home Page</a></div>
                <br />
                <div style='font-weight:bold'>CSR Dashboard</div>
                <div class='cdsm_mainmenu'>
                    <ul>
                        <li><a href='{0}/CSRInfo/{1}'>My CSR Info</a></li>
                        <li><a href='{0}/CallQueue/{1}'>Call Queue</a></li>
                        <li>
                            <span class='collapse_arrow'></span>
                            <span><a href='{0}/CustomerDashboard/{1}'>Customer Dashboard</a></span>
                            <ul>
                                <li><a href='{0}/CustomerDashboard/Orders{1}'>Recent Orders</a></li>
                                <li><a class='current' href='#'>Support Cases</a></li>
                                <li><a href='{0}/CustomerDashboard/Notes{1}'>Notes</a></li>
                            </ul>
                        </li>
                    </ul>
                </div>
                <div class='cdsm_submenu'>

                </div>";

            string quicklaunchmenu = string.Format(quicklaunchmenuFormat, url, queryurl);

            string qlwebPartXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><webParts><webPart xmlns=\"http://schemas.microsoft.com/WebPart/v3\"><metaData><type name=\"Microsoft.SharePoint.WebPartPages.ScriptEditorWebPart, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\" /><importErrorMessage>Cannot import this Web Part.</importErrorMessage></metaData><data><properties><property name=\"Content\" type=\"string\"><![CDATA[" + quicklaunchmenu + "​​​]]></property><property name=\"ChromeType\" type=\"chrometype\">None</property></properties></data></webPart></webParts>";
            WebPartDefinition qlWpd = limitedWebPartManager.ImportWebPart(qlwebPartXml);
            WebPartDefinition qlWpdNew = limitedWebPartManager.AddWebPart(qlWpd.WebPart, "SupportCasesZoneLeft", 0);
            ctx.Load(qlWpdNew);

            //Customer Dropdown List Script Web Part
            string dpwebPartXml = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Assets/CustomerDropDownlist.webpart");
            WebPartDefinition dpWpd = limitedWebPartManager.ImportWebPart(dpwebPartXml);
            WebPartDefinition dpWpdNew = limitedWebPartManager.AddWebPart(dpWpd.WebPart, "SupportCasesZoneTop", 0);
            ctx.Load(dpWpdNew);

            //Support Case CBS Info Web Part
            string cbsInfoWebPartXml = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Assets/SupportCaseCBSWebPartInfo.webpart");
            WebPartDefinition cbsInfoWpd = limitedWebPartManager.ImportWebPart(cbsInfoWebPartXml);
            WebPartDefinition cbsInfoWpdNew = limitedWebPartManager.AddWebPart(cbsInfoWpd.WebPart, "SupportCasesZoneMiddle", 0);
            ctx.Load(cbsInfoWpdNew);

            //Support Case Content By Search Web Part
            string cbswebPartXml = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Assets/SupportCase CBS Webpart/SupportCaseCBS.webpart");
            WebPartDefinition cbsWpd = limitedWebPartManager.ImportWebPart(cbswebPartXml);
            WebPartDefinition cbsWpdNew = limitedWebPartManager.AddWebPart(cbsWpd.WebPart, "SupportCasesZoneMiddle", 1);
            ctx.Load(cbsWpdNew);

            //Support Cases App Part
            string appPartXml = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Assets/SupportCaseAppPart.webpart");
            WebPartDefinition appPartWpd = limitedWebPartManager.ImportWebPart(appPartXml);
            WebPartDefinition appPartdNew = limitedWebPartManager.AddWebPart(appPartWpd.WebPart, "SupportCasesZoneBottom", 0);
            ctx.Load(appPartdNew);

            //Get Host Web Query String and show support case list web part
            string querywebPartXml = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Assets/GetHostWebQueryStringAndShowList.webpart");
            WebPartDefinition queryWpd = limitedWebPartManager.ImportWebPart(querywebPartXml);
            WebPartDefinition queryWpdNew = limitedWebPartManager.AddWebPart(queryWpd.WebPart, "SupportCasesZoneBottom", 1);
            ctx.Load(queryWpdNew);


            file.CheckIn("Data storage model", CheckinType.MajorCheckIn);
            file.Publish("Data storage model");
            ctx.Load(file);
            ctx.ExecuteQuery();
        }

        public static void UploadItemTemplateJS(ClientContext ctx, List materpagelist, string remoteFolderURL, string fileName)
        {
            string remoteFileURL = string.Format("{0}/Display%20Templates/Content%20Web%20Parts/{1}", remoteFolderURL, fileName);
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Assets/SupportCase CBS Webpart/" + fileName);
            newFile.Url = remoteFileURL;
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = materpagelist.RootFolder.Files.Add(newFile);
            ctx.Load(uploadFile);
            ctx.ExecuteQuery();

            var listItem = uploadFile.ListItemAllFields;
            if (uploadFile.CheckOutType == CheckOutType.None)
            {
                uploadFile.CheckOut();
            }
            listItem["Title"] = fileName.Substring(0, fileName.IndexOf(".js"));
            listItem["ContentTypeId"] = "0x0101002039C03B61C64EC4A04F5361F385106603";
            listItem["TargetControlType"] = ";#Content Web Parts;#";
            listItem["DisplayTemplateLevel"] = "Item";
            listItem["TemplateHidden"] = "0";
            listItem["UIVersion"] = "15";
            listItem["ManagedPropertyMapping"] = "'Title'{Title}:'Title','ID'{ID}:'ListItemId','Status'{Status}:'FTCAMStatusOWSTEXT','CSR'{CSR}:'FTCAMCSROWSTEXT','Customer ID'{Customer ID}:'FTCAMCustomerIDOWSTEXT'";
            listItem.Update();
            uploadFile.CheckIn("", CheckinType.MajorCheckIn);
            ctx.ExecuteQuery();
        }

        public static void UploadControlTemplateJS(ClientContext ctx, List materpagelist, string remoteFolderURL, string fileName)
        {
            string romoteFileURL = string.Format("{0}/Display%20Templates/Content%20Web%20Parts/{1}", remoteFolderURL, fileName);
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Assets/SupportCase CBS Webpart/" + fileName);
            newFile.Url = romoteFileURL;
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = materpagelist.RootFolder.Files.Add(newFile);
            ctx.Load(uploadFile);
            ctx.ExecuteQuery();

            var listItem = uploadFile.ListItemAllFields;
            if (uploadFile.CheckOutType == CheckOutType.None)
            {
                uploadFile.CheckOut();
            }
            listItem["Title"] = fileName.Substring(0, fileName.IndexOf(".js"));
            listItem["ContentTypeId"] = "0x0101002039C03B61C64EC4A04F5361F385106601";
            listItem["TargetControlType"] = ";#Content Web Parts;#";
            listItem["DisplayTemplateLevel"] = "Control";
            listItem["TemplateHidden"] = "0";
            listItem["UIVersion"] = "15";
            listItem.Update();
            uploadFile.CheckIn("", CheckinType.MajorCheckIn);
            ctx.ExecuteQuery();
        }
    }
}