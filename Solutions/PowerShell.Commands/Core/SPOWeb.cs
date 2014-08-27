using OfficeDevPnP.PowerShell.Core.Utils;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.SharePoint.Client.Publishing;

namespace OfficeDevPnP.PowerShell.Core
{
    [Obsolete("Use OfficeDev.PnP.Core extensions")]
    public static class SPOWeb
    {
        /// <summary>
        /// Sets the master page of the current web;
        /// </summary>
        /// <param name="MasterPageUrl"></param>
        /// <param name="CustomMasterPageUrl"></param>
        [Obsolete("Use OfficeDevPnP.Core extensions")]
        public static void SetMasterPage(string MasterPageUrl, string CustomMasterPageUrl, Web web, ClientContext clientContext)
        {
            if (!string.IsNullOrEmpty(MasterPageUrl))
            {
                web.MasterUrl = MasterPageUrl;
            }
            if (!string.IsNullOrEmpty(CustomMasterPageUrl))
            {
                web.CustomMasterUrl = CustomMasterPageUrl;
            }
            web.Update();
            clientContext.ExecuteQuery();
        }

        [Obsolete("Use CSOM object model instead")]
        public static ExpandoObject GetMasterPage(Web web, ClientContext clientContext)
        {
            clientContext.Load(web, w => w.MasterUrl, w => w.CustomMasterUrl);
            clientContext.ExecuteQuery();

            dynamic returnObject = new ExpandoObject();
            returnObject.MasterUrl = web.MasterUrl;
            returnObject.CustomMasterUrl = web.CustomMasterUrl;

            return returnObject;
        }

        /// <summary>
        /// Creates a new web
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="locale"></param>
        /// <param name="description"></param>
        /// <param name="webtemplate"></param>
        /// <param name="useSamePermissionAsParentSite"></param>
        [Obsolete("Use Web.CreateWeb() in OfficeDev/PnP.Core")]
        public static Web CreateWeb(string url, string title, int locale, string description, string webtemplate, Web web, ClientContext clientContext, bool useSamePermissionAsParentSite = false)
        {
            var webCreateInfo = new WebCreationInformation();

            webCreateInfo.Description = description;
            webCreateInfo.Language = locale;
            webCreateInfo.Title = title;
            webCreateInfo.Url = url;

            webCreateInfo.UseSamePermissionsAsParentSite = useSamePermissionAsParentSite;
            webCreateInfo.WebTemplate = webtemplate;

            Web newWeb = web.Webs.Add(webCreateInfo);

            clientContext.Load(newWeb);
            clientContext.ExecuteQuery();

            return newWeb;
        }

        /// <summary>
        /// Adds a file to the specified web
        /// </summary>
        /// <param name="path">Local path of the file, including filename</param>
        /// <param name="url">Remote url of the file, including filename</param>
        /// <param name="web">The web to add the file to</param>
        [Obsolete("Use OfficeDev/PnP.Core extensions methods")]
        public static void AddFile(string path, string url, Web web, bool Checkout, bool useWebDav, ClientContext clientContext, bool publish = false, string publishComment = "", bool approve = false, string approveComment = "")
        {
            //FileCollection files = null;

            if (url.EndsWith("/"))
            {
                throw new Exception(Properties.Resources.URLShouldIncludeFileName);
            }

            if (Checkout)
            {
                CheckOutFile(url, web, clientContext);
            }

            if (useWebDav)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    clientContext.ExecuteQuery();
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, url, fs, true);
                }
            }
            else
            {
                var files = web.RootFolder.Files;
                clientContext.Load(files);

                clientContext.ExecuteQuery();

                if (files != null)
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        FileCreationInformation createInfo = new FileCreationInformation();
                        createInfo.ContentStream = stream;

                        createInfo.Overwrite = true;
                        createInfo.Url = url;
                        files.Add(createInfo);
                        clientContext.ExecuteQuery();
                    }
                }
            }

            if (Checkout)
            {
                CheckInFile(url, CheckinType.MajorCheckIn, "", web, clientContext);
            }
            if (publish)
            {
                PublishFile(url, publishComment, web, clientContext);
            }
            if (approve)
            {
                ApproveFile(url, approveComment, web, clientContext);
            }


            //    if (Checkout)
            //    {
            //        CheckInFile(url, CheckinType.MajorCheckIn, "", web, clientContext);
            //    }
            //    if (publish)
            //    {
            //        PublishFile(url, publishComment, web, clientContext);
            //    }
            //    if (approve)
            //    {
            //        ApproveFile(url, approveComment, web, clientContext);
            //    }
            //}
            //}
        }

        [Obsolete("Use Web.GetFileAsString() in OfficeDev/PnP.Core")]
        public static string GetFile(string url, Web web, ClientContext clientContext)
        {
            string returnString = string.Empty;

            var file = web.GetFileByServerRelativeUrl(url);

            clientContext.Load(file);

            clientContext.ExecuteQuery();

            ClientResult<Stream> stream = file.OpenBinaryStream();

            clientContext.ExecuteQuery();

            using (Stream memStream = new MemoryStream())
            {
                CopyStream(stream.Value, memStream);

                memStream.Position = 0;

                StreamReader reader = new StreamReader(memStream);

                returnString = reader.ReadToEnd();
            }
            return returnString;
        }

        [Obsolete("Use Web.SaveFileToLocal() in OfficeDev/PnP.Core")]
        public static void GetFile(string url, string pathOut, string fileName, Web web, ClientContext clientContext)
        {
            var file = web.GetFileByServerRelativeUrl(url);

            clientContext.Load(file);

            clientContext.ExecuteQuery();

            ClientResult<Stream> stream = file.OpenBinaryStream();

            clientContext.ExecuteQuery();

            string fileOut;


            if (!string.IsNullOrEmpty(fileName))
            {
                fileOut = Path.Combine(pathOut, fileName);
            }
            else
            {
                fileOut = Path.Combine(pathOut, file.Name);
            }

            using (Stream fileStream = new FileStream(fileOut, FileMode.Create))
            {
                CopyStream(stream.Value, fileStream);
            }
        }

        private static void CopyStream(Stream source, Stream destination)
        {
            byte[] buffer = new byte[32768];
            int bytesRead;
            do
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);
        }

        [Obsolete("Use Web.UploadDocument() or Web.UploadFileToServerRelativeUrl() in OfficeDev/PnP.Core")]
        public static Microsoft.SharePoint.Client.File AddFile(byte[] bytes, string url, Web web, bool Checkout, ClientContext clientContext, bool publish = false, string publishComment = "", bool approve = false, string approveComment = "")
        {
            FileCollection files = null;

            if (url.EndsWith("/"))
            {
                throw new Exception(Properties.Resources.URLShouldIncludeFileName);
            }

            try
            {
                if (Checkout)
                {
                    CheckOutFile(url, web, clientContext);
                }
            }
            catch (Exception exc)
            {
                throw new Exception(string.Format(Properties.Resources.ErrorCheckingOutFile0, exc.Message));
            }

            try
            {
                //clientContext.Load(web);
                clientContext.Load(web.RootFolder);
                clientContext.ExecuteQuery();

                files = web.RootFolder.Files;
                clientContext.Load(files);
                clientContext.ExecuteQuery();
            }
            catch (Exception)
            {

            }

            Microsoft.SharePoint.Client.File uploadedFile = null;
            if (files != null)
            {
                try
                {
                    FileCreationInformation createInfo = new FileCreationInformation();
                    createInfo.Content = bytes;
                    createInfo.Overwrite = true;
                    createInfo.Url = url;
                    uploadedFile = files.Add(createInfo);
                    clientContext.Load(uploadedFile);
                    clientContext.ExecuteQuery();
                }
                catch (Exception exc)
                {
                    throw new Exception(string.Format(Properties.Resources.ErrorAddingFile0, exc.Message));
                }

                if (Checkout)
                {
                    try
                    {
                        CheckInFile(url, CheckinType.MajorCheckIn, "", web, clientContext);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format(Properties.Resources.ErrorCheckingInFile0, exc.Message));
                    }
                }

                if (publish)
                {
                    try
                    {
                        PublishFile(url, publishComment, web, clientContext);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format(Properties.Resources.ErrorPublishingFile0, exc.Message));
                    }
                }

                if (approve)
                {
                    try
                    {
                        ApproveFile(url, approveComment, web, clientContext);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format(Properties.Resources.ErrorApprovingFile0, exc.Message));
                    }
                }

            }
            return uploadedFile;
        }

        /// <summary>
        /// Sets the homepage of the site
        /// </summary>
        /// <param name="rootFolderRelativeUrl">The url of the homepage relative to the  </param>
        /// <param name="web"></param>
        /// <param name="clientContext"></param>
        [Obsolete("Use Web.SetHomePage() in OfficeDev/PnP.Core")]
        public static void SetHomePage(string rootFolderRelativeUrl, Web web, ClientContext clientContext)
        {
            Folder folder = web.RootFolder;

            folder.WelcomePage = rootFolderRelativeUrl;

            folder.Update();

            clientContext.ExecuteQuery();
        }

        [Obsolete("Use Microsoft.SharePoint.Client Object Model")]
        public static string GetHomePage(Web web, ClientContext clientContext)
        {
            Folder folder = web.RootFolder;

            clientContext.Load(folder, f => f.WelcomePage);

            clientContext.ExecuteQuery();

            return folder.WelcomePage;
        }

        [Obsolete("Use Web.PublishFile() in OfficeDev/PnP.Core")]
        public static void PublishFile(string url, string comment, Web web, ClientContext clientContext)
        {
            Microsoft.SharePoint.Client.File file = null;
            file = web.GetFileByServerRelativeUrl(url);
            clientContext.Load(file, x => x.Exists, x => x.CheckOutType);
            clientContext.ExecuteQuery();
            if (file.Exists)
            {
                file.Publish(comment);
            }
            clientContext.ExecuteQuery();
        }

        [Obsolete("Use Web.ApproveFile() in OfficeDev/PnP.Core")]
        public static void ApproveFile(string url, string comment, Web web, ClientContext clientContext)
        {
            Microsoft.SharePoint.Client.File file = null;
            file = web.GetFileByServerRelativeUrl(url);
            clientContext.Load(file, x => x.Exists, x => x.CheckOutType);
            clientContext.ExecuteQuery();
            if (file.Exists)
            {
                file.Approve(comment);
            }
            clientContext.ExecuteQuery();
        }

        [Obsolete("Use Web.CheckOutFile in OfficeDev/PnP.Core")]
        public static void CheckOutFile(string url, Web web, ClientContext clientContext)
        {

            Microsoft.SharePoint.Client.File file = null;
            file = web.GetFileByServerRelativeUrl(url);
            clientContext.Load(file, x => x.Exists, x => x.CheckOutType);
            clientContext.ExecuteQuery();

            if (file.Exists)
            {
                if (file.CheckOutType == CheckOutType.None)
                {
                    file.CheckOut();
                    clientContext.ExecuteQuery();
                }
            }
        }
        [Obsolete("Use Web.CheckInFile in OfficeDev/PnP.Core")]
        public static void CheckInFile(string url, CheckinType checkinType, string comment, Web web, ClientContext clientContext)
        {
            Microsoft.SharePoint.Client.File file = null;
            file = web.GetFileByServerRelativeUrl(url);
            clientContext.Load(file, x => x.Exists, x => x.CheckOutType);
            clientContext.ExecuteQuery();

            if (file.Exists)
            {
                if (file.CheckOutType != CheckOutType.None)
                {
                    file.CheckIn(comment, checkinType);
                    clientContext.ExecuteQuery();
                }
            }
        }


        [Obsolete("Use CSOM")]
        public static void ApplyTheme(string colorPaletteUrl, string fontSchemeUrl, string backgroundImageUrl, bool shareGenerated, Web web, ClientContext clientContext)
        {
            web.ApplyTheme(colorPaletteUrl, fontSchemeUrl, backgroundImageUrl, shareGenerated);
            clientContext.ExecuteQuery();
        }

        [Obsolete("Use AllProperties property on Web object")]
        public static Dictionary<string, object> GetPropertyBag(Web web, ClientContext clientContext)
        {
            PropertyValues values = web.AllProperties;
            clientContext.Load(values);
            clientContext.ExecuteQuery();
            return values.FieldValues;
        }

        [Obsolete("Use SetPropertyBagValue in OfficeDev/PnP.Core")]
        public static void SetPropertyBagValue(string key, object value, Web web, ClientContext clientContext)
        {
            clientContext.Load(web.AllProperties);
            web.AllProperties[key] = value;
            web.Update();
            clientContext.ExecuteQuery();
        }

        [Obsolete("Use RemovePropertyBagValue in OfficeDev/PnP.Core")]
        public static void RemovePropertyBagEntry(string key, Web web, ClientContext clientContext)
        {

            web.AllProperties[key] = null;
            web.AllProperties.FieldValues.Remove(key);

            web.Update();
            clientContext.Load(web, w => w.AllProperties);
            clientContext.ExecuteQuery();
        }

        [Obsolete("Use AddIndexedPropertyBagKey() RemoveIndexedPropertyBagKey() in OfficeDev/PnP.Core")]
        public static void SetIndexedPropertyKeys(List<string> keys, Web web, ClientContext clientContext)
        {
            clientContext.Load(web.AllProperties);

            clientContext.ExecuteQuery();

            web.AllProperties["vti_indexedpropertykeys"] = GetEncodedValueForSearchIndexProperty(keys);
            web.Update();
            clientContext.ExecuteQuery();

        }

        [Obsolete("Use Web.ReindexSite extension in OfficeDevPnP.Core")]
        public static void ReIndex(Web web, ClientContext clientContext)
        {
            web.ReIndexSite();
        }

        /// <summary>
        /// Used to convert the list of property keys is required format for listing keys to be index
        /// </summary>
        /// <param name="keys">list of keys to set to be searchable</param>
        /// <returns>string formatted list of keys in proper format</returns>
        internal static string GetEncodedValueForSearchIndexProperty(List<string> keys)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string current in keys)
            {
                stringBuilder.Append(Convert.ToBase64String(Encoding.Unicode.GetBytes(current)));
                stringBuilder.Append('|');
            }
            return stringBuilder.ToString();
        }

        [Obsolete("Use Web.AddCustomAction() in OfficeDev/PnP.Core")]
        public static UserCustomAction AddCustomAction(Web web, string title, string group, string location, string name, int sequence, string url, BasePermissions rights, ClientContext clientContext)
        {
            UserCustomAction customAction = null;

            // Add site settings link, if it doesn't already exist
            if (!CustomActionAlreadyExists(clientContext, name))
            {
                // Add site settings link
                customAction = clientContext.Web.UserCustomActions.Add();
                customAction.Group = group;
                customAction.Location = location;
                customAction.Name = name;
                customAction.Sequence = sequence;
                customAction.Url = url;
                customAction.Rights = rights;
                customAction.Title = title;
                customAction.Update();
                clientContext.Load(customAction);
                clientContext.ExecuteQuery();
            }
            return customAction;
        }

        [Obsolete("Use Web.GetCustomActions() in OfficeDev/PnP.Core")]
        public static List<UserCustomAction> GetCustomActions(Web web, ClientContext clientContext)
        {
            List<UserCustomAction> actions = new List<UserCustomAction>();

            clientContext.Load(web.UserCustomActions);
            clientContext.ExecuteQuery();

            foreach (UserCustomAction uca in web.UserCustomActions)
            {
                actions.Add(uca);
            }
            return actions;
        }

        [Obsolete("Use Web.DeleteCustomAction() in OfficeDev/PnP.Core")]
        public static void DeleteCustomAction(Guid id, Web web, ClientContext clientContext)
        {
            clientContext.Load(web.UserCustomActions);
            clientContext.ExecuteQuery();

            foreach (UserCustomAction action in web.UserCustomActions)
            {
                if (action.Id == id)
                {
                    action.DeleteObject();
                    clientContext.ExecuteQuery();
                    break;
                }
            }

        }

        /// <summary>
        /// Utility method to check particular custom action already exists on the web
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="name">Name of the custom action</param>
        /// <returns></returns>
        private static bool CustomActionAlreadyExists(ClientContext clientContext, string name)
        {
            clientContext.Load(clientContext.Web.UserCustomActions);
            clientContext.ExecuteQuery();
            for (int i = 0; i < clientContext.Web.UserCustomActions.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(clientContext.Web.UserCustomActions[i].Name) &&
                        clientContext.Web.UserCustomActions[i].Name.ToLowerInvariant() == name.ToLowerInvariant())
                {
                    return true;
                }
            }
            return false;
        }

        [Obsolete("Use FindFiles() in OfficeWeb/PnP.Core")]
        public static List<Microsoft.SharePoint.Client.File> FindFiles(Web web, string match, ClientContext clientContext)
        {
            Folder rootFolder = web.RootFolder;
            match = WildcardToRegex(match);
            List<Microsoft.SharePoint.Client.File> files = new List<Microsoft.SharePoint.Client.File>();

            ParseFiles(rootFolder, match, clientContext, ref files);

            return files;
        }


        private static void ParseFiles(Folder folder, string match, ClientContext context, ref List<Microsoft.SharePoint.Client.File> foundFiles)
        {

            FileCollection files = folder.Files;
            context.Load(files, fs => fs.Include(f => f.ServerRelativeUrl, f => f.Name, f => f.Title, f => f.TimeCreated, f => f.TimeLastModified));
            context.Load(folder.Folders);
            context.ExecuteQuery();
            foreach (Microsoft.SharePoint.Client.File file in files)
            {
                if (Regex.IsMatch(file.Name, match, RegexOptions.IgnoreCase))
                {

                    foundFiles.Add(file);
                }
            }
            foreach (Folder subfolder in folder.Folders)
            {
                ParseFiles(subfolder, match, context, ref foundFiles);
            }
        }

        private static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
                               Replace(@"\*", ".*").
                               Replace(@"\?", ".") + "$";
        }


        [Obsolete("Use CSOM")]
        public static Web GetWeb(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            clientContext.Load(web);
            clientContext.ExecuteQuery();

            return web;

        }

        [Obsolete("Use CSOM")]
        public static Web GetWebById(Guid guid, ClientContext clientContext)
        {
            Site site = clientContext.Site;
            Web web = site.OpenWebById(guid);
            clientContext.Load(web);
            clientContext.ExecuteQuery();

            return web;
        }

        [Obsolete("Use CSOM")]
        public static Web GetWebByUrl(string url, ClientContext clientContext)
        {
            Site site = clientContext.Site;
            Web web = site.OpenWeb(url);
            clientContext.Load(web);
            clientContext.ExecuteQuery();

            return web;
        }

        [Obsolete("Use CSOM")]
        public static List<Web> GetSubWebs(Web web, ClientContext clientContext)
        {
            List<Web> webs = new List<Web>();
            clientContext.Load(web.Webs);

            clientContext.ExecuteQuery();
            foreach (var w in web.Webs)
            {
                webs.Add(w);
            }
            return webs;
        }

        [Obsolete("Use OfficeDev/PnP.Core Web.AddNavigationNode()")]
        public static void AddNavigationLink(Web web, NavigationNodeType nodeType, string title, string url, bool asLast, string header, string previous, ClientContext clientContext)
        {
            var nodes = (nodeType == NavigationNodeType.QuickLaunch) ? web.Navigation.QuickLaunch : web.Navigation.TopNavigationBar;
            clientContext.Load(nodes, n => n.IncludeWithDefaultProperties(c => c.Children));
            clientContext.ExecuteQuery();
            if (header != null)
            {
                var headerNode = nodes.Where(x => x.Title == header).FirstOrDefault();
                if (headerNode != null)
                {
                    NavigationNodeCreationInformation ciNode = new NavigationNodeCreationInformation();
                    if (previous != null)
                    {
                        var children = headerNode.Children;
                        clientContext.Load(children, n => n.IncludeWithDefaultProperties(c => c.Title));
                        var previousNode = children.Where(x => x.Title == previous).FirstOrDefault();
                        if (previousNode != null)
                        {
                            ciNode.PreviousNode = previousNode;
                        }
                        else
                        {
                            throw new Exception("Previous Node with title " + previous + " not found.");
                        }
                    }
                    ciNode.AsLastNode = asLast;
                    ciNode.Title = title;
                    ciNode.Url = url;
                    NavigationNode node = headerNode.Children.Add(ciNode);
                    clientContext.ExecuteQuery();
                }
                else
                {
                    throw new Exception("Header Node with title " + header + " not found");
                }
            }
            else
            {
                NavigationNodeCreationInformation ciNode = new NavigationNodeCreationInformation();
                ciNode.AsLastNode = asLast;
                ciNode.Title = title;
                ciNode.Url = url;
                NavigationNode node = nodes.Add(ciNode);
                clientContext.ExecuteQuery();
            }
        }

        [Obsolete()]
        public enum NavigationNodeType
        {
            Top,
            QuickLaunch
        }

    }



}
