using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Web;

using OfficeDevPnP.Core;

using Microsoft.SharePoint.Client;

namespace Portal.DataAccessLayer
{
    public class Helper
    {
        private static int contextCount = 0;
        private static bool alreadyAuthorized = false;

        public enum MasterPageOptions
        {
            BothMasterPages = 0,
            SiteMasterPageOnly,
            SystemMasterPageOnly
        }

        public static ClientContext CreateAuthenticatedUserContext(string domain, string username, SecureString password, string siteUrl)
        {
            ClientContext userContext = new ClientContext(siteUrl);
            try
            {
                if (String.IsNullOrEmpty(domain))
                {
                    // use o365 authentication (SPO-MT or vNext)
                    userContext.Credentials = new SharePointOnlineCredentials(username, password);
                }
                else
                {
                    // use Windows authentication (SPO-D or On-Prem) 
                    userContext.Credentials = new NetworkCredential(username, password, domain);
                }

                // Let's prevent account lock-outs...
                if (alreadyAuthorized == false)
                {
                    Web web = userContext.Web;
                    userContext.Load(web);
                    userContext.ExecuteQueryRetry();
                    contextCount = 0;
                    alreadyAuthorized = true;
                }
                return userContext;
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Message.ToLower().Contains("unauthorized") && alreadyAuthorized == false)
                {
                    contextCount++;
                    if (contextCount == 1)
                    {
                        System.Console.WriteLine();
                        Logger.LogWarningMessage(String.Format("Attempt [{0}]: You have entered invalid login credentials. You have 2 more attempts allowed.", contextCount, 3 - contextCount), true);
                    }
                    else if (contextCount == 2)
                    {
                        System.Console.WriteLine();
                        Logger.LogWarningMessage(String.Format("Attempt [{0}]: You have entered invalid login credentials. You have 1 more attempt allowed.", contextCount, 3 - contextCount), true);
                    }
                    else if (contextCount == 3)
                    {
                        System.Console.WriteLine();
                        Logger.LogErrorMessage(String.Format("Attempt [{0}]: You have entered invalid login credentials. You have no more attempts allowed. Press any key to terminate the application.", contextCount, 3 - contextCount), true);

                        System.Console.ReadKey();
                        Environment.Exit(0);
                    }

                    Program.GetCredentials();
                    userContext = CreateAuthenticatedUserContext(Program.AdminDomain, Program.AdminUsername, Program.AdminPassword, siteUrl);
                }
            }
            catch (System.ArgumentNullException ex)
            {
                contextCount++;
                if (contextCount == 1)
                {
                    System.Console.WriteLine();
                    Logger.LogWarningMessage(String.Format("Attempt [{0}]: You have entered invalid login credentials. You have 2 more attempts allowed.", contextCount, 3 - contextCount), true);
                }
                else if (contextCount == 2)
                {
                    System.Console.WriteLine();
                    Logger.LogWarningMessage(String.Format("Attempt [{0}]: You have entered invalid login credentials. You have 1 more attempt allowed.", contextCount, 3 - contextCount), true);
                }
                else if (contextCount == 3)
                {
                    System.Console.WriteLine();
                    Logger.LogErrorMessage(String.Format("Attempt [{0}]: You have entered invalid login credentials. You have no more attempts allowed. Press any key to terminate the application.", contextCount, 3 - contextCount), true);

                    System.Console.ReadKey();
                    Environment.Exit(0);
                }

                Program.GetCredentials();
                userContext = CreateAuthenticatedUserContext(Program.AdminDomain, Program.AdminUsername, Program.AdminPassword, siteUrl);
            }

            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("\nCreateAuthenticatedUserContext() failed for {0}: Error={1}", siteUrl, ex.Message), false);
            }

            return userContext;
        }

        /// <summary>
        /// Creates a Secure String
        /// </summary>
        /// <param name="data">string to be converted</param>
        /// <returns>secure string instance</returns>
        public static SecureString CreateSecureString(string data)
        {
            if (data == null || string.IsNullOrEmpty(data))
            {
                return null;
            }

            System.Security.SecureString secureString = new System.Security.SecureString();

            char[] charArray = data.ToCharArray();

            foreach (char ch in charArray)
            {
                secureString.AppendChar(ch);
            }

            return secureString;
        }

        public static Field EnsureSiteColumn(Web web, Guid fieldID, string fieldAsXml)
        {
            Field existingField = web.GetFieldById<Field>(fieldID);
            if (existingField != null)
            {
                return existingField;
            }
            Field newField = web.CreateField(fieldAsXml, true);
            return newField;
        }

        public static File GetFileFromWeb(Web web, string serverRelativeUrl)
        {
            try
            {
                File webFile = web.GetFileByServerRelativeUrl(serverRelativeUrl);
                web.Context.Load(webFile);
                web.Context.Load(webFile.ListItemAllFields);
                web.Context.ExecuteQueryRetry();

                return webFile;
            }
            catch {}

            return null;
        }

        private static string GetContentType(Web web, List list, string contentType)
        {
            ContentTypeCollection collection = list.ContentTypes;
            web.Context.Load(collection);
            web.Context.ExecuteQueryRetry();
            var ct = collection.Where(c => c.Name == contentType).FirstOrDefault();
            string contentTypeID = "";
            if (ct != null)
            {
                contentTypeID = ct.StringId;
            }

            return contentTypeID;
        }

        public static string GetSiteUrl(string siteTitle)
        {
            string url = String.Empty;
            ConsoleKey key = ConsoleKey.NoName;
            do
            {
                do
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                    System.Console.WriteLine(String.Format("Please enter the absolute Url of the {0} site collection:", siteTitle));
                    System.Console.ResetColor();
                    url = System.Console.ReadLine().Trim();

                } while (String.IsNullOrEmpty(url));

                System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                System.Console.WriteLine(String.Format("Please press 'Y' to confirm the {0} site Url: {1} ", siteTitle, url));
                System.Console.ResetColor();
                key = System.Console.ReadKey().Key;
                System.Console.WriteLine();

            } while (key != ConsoleKey.Y);

            return url;
        }

        public static string UploadMasterPage(Web web, string mpFileName, string localFilePath, string title, string description)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Uploading Master Page File: {0} ...", mpFileName), true);

                List mpGallery = web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
                Folder mpGalleryRoot = mpGallery.RootFolder;
                web.Context.Load(mpGallery);
                web.Context.Load(mpGalleryRoot);
                web.Context.ExecuteQueryRetry();

                string mpFilePath = mpGalleryRoot.ServerRelativeUrl + "/" + mpFileName;
                File mpFile = GetFileFromWeb(web, mpFilePath);
                if (mpFile == null)
                {
                    // Get the file name from the provided path
                    Byte[] fileBytes = System.IO.File.ReadAllBytes(localFilePath);

                    // Use CSOM to upload the file in
                    FileCreationInformation newFile = new FileCreationInformation();
                    newFile.Content = fileBytes;
                    newFile.Overwrite = true;
                    newFile.Url = mpFileName;

                    File uploadFile = mpGalleryRoot.Files.Add(newFile);
                    web.Context.Load(uploadFile);
                    web.Context.ExecuteQueryRetry();
                }

                // Grab the file we just uploaded so we can edit its properties
                mpFile = GetFileFromWeb(web, mpFilePath);
                if (mpGallery.ForceCheckout || mpGallery.EnableVersioning)
                {
                    if (mpFile.CheckOutType == CheckOutType.None)
                    {
                        mpFile.CheckOut();
                    }
                }

                ListItem fileListItem = mpFile.ListItemAllFields;
                fileListItem["MasterPageDescription"] = description;
                fileListItem["UIVersion"] = "15";
                if (mpGallery.AllowContentTypes && mpGallery.ContentTypesEnabled)
                {
                    fileListItem["Title"] = title;
                    fileListItem["ContentTypeId"] = Constants.MASTERPAGE_CONTENT_TYPE;
                }
                fileListItem.Update();

                if (mpGallery.ForceCheckout || mpGallery.EnableVersioning)
                {
                    mpFile.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                    if (mpGallery.EnableModeration)
                    {
                        mpFile.Approve("");
                    }
                }
                web.Context.ExecuteQueryRetry();

                Logger.LogSuccessMessage(String.Format("Uploaded Master Page File: {0}", mpFile.ServerRelativeUrl), false);
                return mpFile.ServerRelativeUrl;
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("UploadMasterPage() failed for {0}: Error={1}", web.Url, ex.Message), false);
                return String.Empty;
            }
        }

        public static string PublishMasterPage(Web web, string mpFilePath, string title, string description)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Publishing Master Page File: {0} ...", mpFilePath), true);

                List mpGallery = web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
                Folder mpGalleryRoot = mpGallery.RootFolder;
                web.Context.Load(mpGallery);
                web.Context.Load(mpGalleryRoot);
                web.Context.ExecuteQueryRetry();

                File mpFile = GetFileFromWeb(web, mpFilePath);
                if (mpFile == null)
                {
                    Logger.LogErrorMessage(String.Format("PublishMasterPage() failed for {0}: Error=File Not Found", web.Url), false);
                    return String.Empty;
                }

                if (mpGallery.ForceCheckout || mpGallery.EnableVersioning)
                {
                    if (mpFile.CheckOutType == CheckOutType.None)
                    {
                        mpFile.CheckOut();
                    }
                }

                ListItem fileListItem = mpFile.ListItemAllFields;
                fileListItem["MasterPageDescription"] = description;
                fileListItem["UIVersion"] = "15";
                if (mpGallery.AllowContentTypes && mpGallery.ContentTypesEnabled)
                {
                    fileListItem["Title"] = title;
                    fileListItem["ContentTypeId"] = Constants.MASTERPAGE_CONTENT_TYPE;
                }
                fileListItem.Update();

                if (mpGallery.ForceCheckout || mpGallery.EnableVersioning)
                {
                    mpFile.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                    if (mpGallery.EnableModeration)
                    {
                        mpFile.Approve("");
                    }
                }
                web.Context.ExecuteQueryRetry();

                Logger.LogSuccessMessage(String.Format("Published Master Page File: {0}", mpFile.ServerRelativeUrl), false);
                return mpFile.ServerRelativeUrl;
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("UploadMasterPage() failed for {0}: Error={1}", web.Url, ex.Message), false);
                return String.Empty;
            }
        }

        /// <summary>
        /// Configures the Master Page settings of the specified Web.
        /// </summary>
        /// <param name="web">Web object to process</param>
        /// <param name="mpServerRelativeUrl">server-relative path to master page file; if null/empty, Web will inherit MPs from parent</param>
        /// <param name="mpOption">0 - All Masters; 1: Site Master Only; 2: System Master Only</param>
        public static void SetMasterPages(Web web, string mpServerRelativeUrl, bool inheritMaster, MasterPageOptions mpOption)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Setting Master Pages for: {0} ...", web.Url), true);

                web.Context.Load(web.AllProperties);
                web.Context.ExecuteQueryRetry();

                if (mpOption == MasterPageOptions.BothMasterPages || mpOption == MasterPageOptions.SiteMasterPageOnly)
                {
                    web.CustomMasterUrl = mpServerRelativeUrl;
                    web.Update();
                    web.Context.ExecuteQueryRetry();
                    web.SetPropertyBagValue(Constants.PropertyBagInheritsCustomMaster, (inheritMaster ? "True" : "False"));
                    Logger.LogSuccessMessage(String.Format("Set Site Master Page to {0}{1}", (inheritMaster ? "inherit " : ""), mpServerRelativeUrl), false);
                }
                if (mpOption == MasterPageOptions.BothMasterPages || mpOption == MasterPageOptions.SystemMasterPageOnly)
                {
                    web.MasterUrl = mpServerRelativeUrl;
                    web.Update();
                    web.Context.ExecuteQueryRetry();
                    web.SetPropertyBagValue(Constants.PropertyBagInheritsMaster, (inheritMaster ? "True" : "False"));
                    Logger.LogSuccessMessage(String.Format("Set System Master Page to {0}{1}", (inheritMaster ? "inherit " : ""), mpServerRelativeUrl), false);
                }
                Logger.LogSuccessMessage(String.Format("Set Master Pages for: {0}", web.Url), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("SetMasterPages() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }
        }
        public static void EnsureSiteColumns(Web web)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Ensuring Site Columns for {0} ...", web.Url), true);

                string fieldAsXML = String.Empty;

                Logger.LogInfoMessage(String.Format("- Ensuring Site Column [{0}] ...", Constants.PnPConfigKey_DisplayName), false);
                fieldAsXML = String.Format(@"<Field ID='{0}' Name='{1}' DisplayName='{2}' Group='{3}' Description='{2}' Type='Text' />",
                    Constants.PnPConfigKey_GUID, Constants.PnPConfigKey_InternalName, Constants.PnPConfigKey_DisplayName, Constants.PnPSiteColumns_GroupName);
                Helper.EnsureSiteColumn(web, new Guid(Constants.PnPConfigKey_GUID), fieldAsXML);

                Logger.LogInfoMessage(String.Format("- Ensuring Site Column [{0}] ...", Constants.PnPConfigValue_DisplayName), false);
                fieldAsXML = String.Format(@"<Field ID='{0}' Name='{1}' DisplayName='{2}' Group='{3}' Description='{2}' Type='Note' NumLines='6' />",
                    Constants.PnPConfigValue_GUID, Constants.PnPConfigValue_InternalName, Constants.PnPConfigValue_DisplayName, Constants.PnPSiteColumns_GroupName);
                Helper.EnsureSiteColumn(web, new Guid(Constants.PnPConfigValue_GUID), fieldAsXML);

                Logger.LogInfoMessage(String.Format("- Ensuring Site Column [{0}] ...", Constants.PnPLinkText_DisplayName), false);
                fieldAsXML = String.Format(@"<Field ID='{0}' Name='{1}' DisplayName='{2}' Group='{3}' Description='{2}' Type='Text' />",
                    Constants.PnPLinkText_GUID, Constants.PnPLinkText_InternalName, Constants.PnPLinkText_DisplayName, Constants.PnPSiteColumns_GroupName);
                Helper.EnsureSiteColumn(web, new Guid(Constants.PnPLinkText_GUID), fieldAsXML);

                Logger.LogInfoMessage(String.Format("- Ensuring Site Column [{0}] ...", Constants.PnPLinkUrl_DisplayName), false);
                fieldAsXML = String.Format(@"<Field ID='{0}' Name='{1}' DisplayName='{2}' Group='{3}' Description='{2}' Type='Text' />",
                    Constants.PnPLinkUrl_GUID, Constants.PnPLinkUrl_InternalName, Constants.PnPLinkUrl_DisplayName, Constants.PnPSiteColumns_GroupName);
                Helper.EnsureSiteColumn(web, new Guid(Constants.PnPLinkUrl_GUID), fieldAsXML);

                Logger.LogInfoMessage(String.Format("- Ensuring Site Column [{0}] ...", Constants.PnPDisplayOrder_DisplayName), false);
                fieldAsXML = String.Format(@"<Field ID='{0}' Name='{1}' DisplayName='{2}' Group='{3}' Description='{2}' Type='Number' Decimals='0' Min='1'/>",
                    Constants.PnPDisplayOrder_GUID, Constants.PnPDisplayOrder_InternalName, Constants.PnPDisplayOrder_DisplayName, Constants.PnPSiteColumns_GroupName);
                Helper.EnsureSiteColumn(web, new Guid(Constants.PnPDisplayOrder_GUID), fieldAsXML);

                FieldCollection siteColumns = web.Fields;
                web.Context.Load(siteColumns);
                web.Context.ExecuteQueryRetry();

                Logger.LogInfoMessage(String.Format("Ensured Site Columns for {0}", web.Url), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureSiteColumns() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }
        }
        public static List EnsurePortalConfigList(Web web)
        {
            List list = null;
            try
            {
                Logger.LogInfoMessage(String.Format("Ensuring Portal Config list for {0} ...", web.Url), true);

                list = web.GetListByUrl(Constants.ConfigurationListWebRelativeUrl);
                if (list == null)
                {
                    Logger.LogInfoMessage(String.Format("Creating Portal Config list ..."), true);
                    list = web.CreateList(Microsoft.SharePoint.Client.ListTemplateType.GenericList, Constants.ConfigurationListTitle, false, true, Constants.ConfigurationListWebRelativeUrl, false);
                }

                FieldCollection fields = list.Fields;
                View defaultView = list.DefaultView;
                ViewFieldCollection viewFields = defaultView.ViewFields;
                web.Context.Load(fields);
                web.Context.Load(defaultView);
                web.Context.Load(viewFields);
                web.Context.ExecuteQueryRetry();

                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPConfigKey_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPConfigKey_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPConfigKey_GUID), true));
                    viewFields.Add(Constants.PnPConfigKey_InternalName);
                    defaultView.Update();
                }
                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPConfigValue_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPConfigValue_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPConfigValue_GUID), true));
                    viewFields.Add(Constants.PnPConfigValue_InternalName);
                    defaultView.Update();
                }
                string viewQuery = String.Format(Constants.ListViewQueryFormatString, Constants.PnPConfigKey_InternalName);
                if (!defaultView.ViewQuery.Equals(viewQuery, StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultView.ViewQuery = viewQuery;
                    defaultView.Update();
                }
                list.Update();
                web.Context.ExecuteQueryRetry();

                //Initialize the list
                ListItemCollection listItems = list.GetItems(CamlQuery.CreateAllItemsQuery());
                web.Context.Load(listItems);
                web.Context.ExecuteQueryRetry();

                // Initialize the list only if it is empty.
                if (listItems.Count == 0)
                {
                    Logger.LogInfoMessage(String.Format("Initializing Portal Config list ..."), false);

                    Logger.LogInfoMessage(String.Format("- Adding list item..."), false);

                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem listItem = list.AddItem(itemCreateInfo);

                    listItem["Title"] = Constants.ConfigurationListFooterKey;
                    listItem[Constants.PnPConfigKey_InternalName] = Constants.ConfigurationListFooterKey;
                    listItem[Constants.PnPConfigValue_InternalName] = String.Format(
                            "<table border='0' cellpadding='0' cellspacing='0' align='center'>" +
                             "<tbody style='float:right;'>" +
                              "<tr>" +
                               "<td align='center' style=''>" +
                                "<a href='{0}' style='color: white'>Footer 1</a> | " +
                                "<a href='{0}' style='color: white'>Footer 2</a> | " +
                                "<a href='{0}' style='color: white'>Footer 3</a> | " +
                                "<a href='{0}' style='color: white'>Footer 4</a> | " +
                                "<a href='{0}' style='color: white'>Footer 5</a> " +
                               "</td>" +
                              "</tr>" +
                              "<tr>" +
                               "<td align='center' style=''>" +
                                "{1} Contoso, Inc. All Rights Reserved." +
                               "</td>" +
                              "</tr>" +
                             "</tbody>" +
                            "</table>", Constants.FooterNavLinkUrl, DateTime.Now.Year);

                    listItem.Update();
                    web.Context.ExecuteQueryRetry();
                }
                Logger.LogSuccessMessage(String.Format("Ensured Portal Config list for {0}", web.Url), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsurePortalConfigList() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }

            return list;
        }
        public static List EnsureGlobalNavConfigList(Web web)
        {
            List list = null;
            try
            {
                Logger.LogInfoMessage(String.Format("Ensuring Global Nav Config list for {0} ...", web.Url), true);

                list = web.GetListByUrl(Constants.GlobalNavListWebRelativeUrl);
                if (list == null)
                {
                    Logger.LogInfoMessage(String.Format("Creating Global Nav Config list ..."), true);
                    list = web.CreateList(Microsoft.SharePoint.Client.ListTemplateType.GenericList, Constants.GlobalNavListTitle, false, true, Constants.GlobalNavListWebRelativeUrl, false);
                }

                FieldCollection fields = list.Fields;
                View defaultView = list.DefaultView;
                ViewFieldCollection viewFields = defaultView.ViewFields;
                web.Context.Load(fields);
                web.Context.Load(defaultView);
                web.Context.Load(viewFields);
                web.Context.ExecuteQueryRetry();

                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPDisplayOrder_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPDisplayOrder_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPDisplayOrder_GUID), true));
                    viewFields.Add(Constants.PnPDisplayOrder_InternalName);
                    defaultView.Update();
                }
                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPLinkText_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPLinkText_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPLinkText_GUID), true));
                    viewFields.Add(Constants.PnPLinkText_InternalName);
                    defaultView.Update();
                }
                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPLinkUrl_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPLinkUrl_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPLinkUrl_GUID), true));
                    viewFields.Add(Constants.PnPLinkUrl_InternalName);
                    defaultView.Update();
                }
                string viewQuery = String.Format(Constants.ListViewQueryFormatString, Constants.PnPDisplayOrder_InternalName);
                if (!defaultView.ViewQuery.Equals(viewQuery, StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultView.ViewQuery = viewQuery;
                    defaultView.Update();
                }
                list.Update();
                web.Context.ExecuteQueryRetry();

                //Initialize the list
                ListItemCollection listItems = list.GetItems(CamlQuery.CreateAllItemsQuery());
                web.Context.Load(listItems);
                web.Context.ExecuteQueryRetry();

                // Initialize the list only if it is empty.
                if (listItems.Count == 0)
                {
                    Logger.LogInfoMessage(String.Format("Initializing Global Nav Config list ..."), false);

                    for (int i = 0; i < 5; i++)
                    {
                        Logger.LogInfoMessage(String.Format("- Adding list item..."), false);

                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem listItem = list.AddItem(itemCreateInfo);

                        listItem["Title"] = String.Format("Global Link {0}", i + 1);
                        listItem[Constants.PnPDisplayOrder_InternalName] = (i + 1);
                        listItem[Constants.PnPLinkText_InternalName] = String.Format("Global {0}", i + 1);
                        listItem[Constants.PnPLinkUrl_InternalName] = Constants.GlobalNavLinkUrl;
                        listItem.Update();
                        web.Context.ExecuteQueryRetry();
                    }
                }
                Logger.LogSuccessMessage(String.Format("Ensured Global Nav Config list for {0}", web.Url), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureGlobalNavConfigList() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }

            return list;
        }
        public static List EnsureCompanyLinksConfigList(Web web)
        {
            List list = null;
            try
            {
                Logger.LogInfoMessage(String.Format("Ensuring Company Links Config list for {0} ...", web.Url), true);

                list = web.GetListByUrl(Constants.CompanyLinksListWebRelativeUrl);
                if (list == null)
                {
                    Logger.LogInfoMessage(String.Format("Creating Company Links Config list ..."), true);
                    list = web.CreateList(Microsoft.SharePoint.Client.ListTemplateType.GenericList, Constants.CompanyLinksListTitle, false, true, Constants.CompanyLinksListWebRelativeUrl, false);
                }

                FieldCollection fields = list.Fields;
                View defaultView = list.DefaultView;
                ViewFieldCollection viewFields = defaultView.ViewFields;
                web.Context.Load(fields);
                web.Context.Load(defaultView);
                web.Context.Load(viewFields);
                web.Context.ExecuteQueryRetry();

                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPDisplayOrder_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPDisplayOrder_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPDisplayOrder_GUID), true));
                    viewFields.Add(Constants.PnPDisplayOrder_InternalName);
                    defaultView.Update();
                }
                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPLinkText_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPLinkText_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPLinkText_GUID), true));
                    viewFields.Add(Constants.PnPLinkText_InternalName);
                    defaultView.Update();
                }
                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPLinkUrl_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPLinkUrl_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPLinkUrl_GUID), true));
                    viewFields.Add(Constants.PnPLinkUrl_InternalName);
                    defaultView.Update();
                }
                string viewQuery = String.Format(Constants.ListViewQueryFormatString, Constants.PnPDisplayOrder_InternalName);
                if (!defaultView.ViewQuery.Equals(viewQuery, StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultView.ViewQuery = viewQuery;
                    defaultView.Update();
                }
                list.Update();
                web.Context.ExecuteQueryRetry();

                //Initialize the list
                ListItemCollection listItems = list.GetItems(CamlQuery.CreateAllItemsQuery());
                web.Context.Load(listItems);
                web.Context.ExecuteQueryRetry();

                // Initialize the list only if it is empty.
                if (listItems.Count == 0)
                {
                    Logger.LogInfoMessage(String.Format("Initializing Company Links Config list ..."), false);

                    for (int i = 0; i < Constants.CompanyLinkTitles.Length; i++)
                    {
                        Logger.LogInfoMessage(String.Format("- Adding list item..."), false);

                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem listItem = list.AddItem(itemCreateInfo);

                        listItem["Title"] = String.Format("Company Link {0}", i + 1);
                        listItem[Constants.PnPDisplayOrder_InternalName] = (i + 1);
                        listItem[Constants.PnPLinkText_InternalName] = Constants.CompanyLinkTitles[i];
                        listItem[Constants.PnPLinkUrl_InternalName] = Constants.CompanyLinkUrls[i];
                        listItem.Update();
                        web.Context.ExecuteQueryRetry();
                    }
                }
                Logger.LogSuccessMessage(String.Format("Ensured Company Links Config list for {0}", web.Url), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureCompanyLinksConfigList() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }

            return list;
        }
        public static List EnsureLocalNavConfigList(Web web)
        {
            List list = null;
            try
            {
                Logger.LogInfoMessage(String.Format("Ensuring Local Nav Config list for {0} ...", web.Url), true);

                list = web.GetListByUrl(Constants.LocalNavListWebRelativeUrl);
                if (list == null)
                {
                    Logger.LogInfoMessage(String.Format("Creating Local Nav Config list ..."), true);
                    list = web.CreateList(Microsoft.SharePoint.Client.ListTemplateType.GenericList, Constants.LocalNavListTitle, false, true, Constants.LocalNavListWebRelativeUrl, false);
                }

                FieldCollection fields = list.Fields;
                View defaultView = list.DefaultView;
                ViewFieldCollection viewFields = defaultView.ViewFields;
                web.Context.Load(fields);
                web.Context.Load(defaultView);
                web.Context.Load(viewFields);
                web.Context.ExecuteQueryRetry();

                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPDisplayOrder_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPDisplayOrder_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPDisplayOrder_GUID), true));
                    viewFields.Add(Constants.PnPDisplayOrder_InternalName);
                    defaultView.Update();
                }
                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPLinkText_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPLinkText_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPLinkText_GUID), true));
                    viewFields.Add(Constants.PnPLinkText_InternalName);
                    defaultView.Update();
                }
                Logger.LogInfoMessage(String.Format("- Ensuring List Field [{0}] ...", Constants.PnPLinkUrl_DisplayName), false);
                if (list.FieldExistsById(new Guid(Constants.PnPLinkUrl_GUID)) == false)
                {
                    fields.Add(web.GetFieldById<Field>(new Guid(Constants.PnPLinkUrl_GUID), true));
                    viewFields.Add(Constants.PnPLinkUrl_InternalName);
                    defaultView.Update();
                }
                string viewQuery = String.Format(Constants.ListViewQueryFormatString, Constants.PnPDisplayOrder_InternalName);
                if (!defaultView.ViewQuery.Equals(viewQuery, StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultView.ViewQuery = viewQuery;
                    defaultView.Update();
                }
                list.Update();
                web.Context.ExecuteQueryRetry();

                //Initialize the list
                ListItemCollection listItems = list.GetItems(CamlQuery.CreateAllItemsQuery());
                web.Context.Load(listItems);
                web.Context.ExecuteQueryRetry();

                // Initialize the list only if it is empty.
                if (listItems.Count == 0)
                {
                    Logger.LogInfoMessage(String.Format("Initializing Local Nav Config list ..."), false);

                    for (int i=0; i<5; i++)
                    {
                        Logger.LogInfoMessage(String.Format("- Adding list item..."), false);

                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem listItem = list.AddItem(itemCreateInfo);

                        listItem["Title"] = String.Format("Local Link {0}", i + 1);
                        listItem[Constants.PnPDisplayOrder_InternalName] = (i + 1);
                        listItem[Constants.PnPLinkText_InternalName] = String.Format("{0} Link {1}", web.Title, i + 1);
                        listItem[Constants.PnPLinkUrl_InternalName] = Constants.LocalNavLinkUrl;
                        listItem.Update();
                        web.Context.ExecuteQueryRetry();
                    }
                }
                Logger.LogSuccessMessage(String.Format("Ensured Local Nav Config list for {0}", web.Url), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureLocalNavConfigList() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }

            return list;
        }
        public static string EnsureMasterPage(Web web, string mpFileName)
        {
            string mpServerRelativeUrl = String.Empty;
            try
            {
                Logger.LogInfoMessage(String.Format("Ensuring Master Page [{1}] for {0} ...", web.Url, mpFileName), true);

                List mpGallery = web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
                Folder mpGalleryRoot = mpGallery.RootFolder;
                web.Context.Load(mpGallery);
                web.Context.Load(mpGalleryRoot);
                web.Context.ExecuteQueryRetry();

                mpServerRelativeUrl = mpGalleryRoot.ServerRelativeUrl + "/" + mpFileName;

                File mpFile = GetFileFromWeb(web, mpServerRelativeUrl);
                if (mpFile == null)
                {
                    string localFilePath = Environment.CurrentDirectory + "\\Master Pages\\" + mpFileName;

                    mpServerRelativeUrl = Helper.UploadMasterPage(web, mpFileName, localFilePath, Constants.PortalMasterPageTitle, Constants.PortalMasterPageDescription);
                    mpServerRelativeUrl = Helper.PublishMasterPage(web, mpServerRelativeUrl, Constants.PortalMasterPageTitle, Constants.PortalMasterPageDescription);
                }
                Logger.LogSuccessMessage(String.Format("Ensured Master Page for {0} ...", web.Url), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureMasterPage() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }

            return mpServerRelativeUrl;
        }
    }
}
