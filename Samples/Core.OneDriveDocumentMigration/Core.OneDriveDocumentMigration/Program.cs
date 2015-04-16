using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System.IO;
using System.Net;

namespace Contoso.Core.OneDriveDocumentMigration
{
    /// <summary>
    /// This program fetches On-Prem mysite folder structure below Personal Documents,
    /// Shared Documents and Shared Pictures, creates respective folder structure to 
    /// SharePoint Online. After that it gets the files from On-Prem and uploads a 
    /// copy of those to SharePoint Online.
    /// 
    /// User information is provided by csv file. Csv structure is:
    /// SpoOneDriveUserName;SpoOneDriveUserEmail;OnPremUserName
    /// 
    /// First line (header) is removed.
    /// </summary>
    class Program
    {
        private static string PersonalDocumentsName = "Personal Documents";
        private static string SharedDocumentsName = "Shared Documents";
        private static string SPODocumentsName = "Documents";
        private static string SPOSharedDocumentsName = "Shared with Everyone";
        private static string PicturesName = "Shared Pictures";

        /// <summary>
        /// User class
        /// Used when users are fetched from csv file.
        /// </summary>
        private class User
        {
            public string SpoOneDriveUserName;
            public string SpoOneDriveUserEmail;
            public string OnPremUserName;
        }

        /// <summary>
        /// Main function
        /// 
        /// Parameters
        /// 0 - Sharepoint Online Admin Url ("https://poc-admin.sharepoint.com")
        /// 1 - Sharepoint Online Onedrive url with placeholder ("https://poc-my.sharepoint.com/personal/{0}_poc_onmicrosoft_com")
        /// 2 - Sharepoint Online Admin name ("admin@poc.onmicrosoft.com")
        /// 3 - Sharepoint Online Admin password ("pass@word1")
        /// 4 - path to CSV File (C:\temp\users.csv)
        /// 5 - Onprem mysite url with placeholder ("http://mysite/personal/{0}")
        /// 6 - Onprem admin name (admin)
        /// 7 - Onprem admin password (pass@word1)
        /// 8 - Overwrite files in SPO
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string spoSiteUrl = string.Empty;
            string spoMysiteUrl = string.Empty;
            string adminUserName = string.Empty;
            SecureString adminPwd = ToSecureString(string.Empty);
            string csvLocation = string.Empty;
            string onPremMysiteUrl = string.Empty;
            string onPremAdmin = string.Empty;
            SecureString onPremAdminPWD = ToSecureString(string.Empty);
            bool overwrite = true;

            try
            {
                spoSiteUrl = args[0];
                spoMysiteUrl = args[1];
                adminUserName = args[2];
                adminPwd = ToSecureString(args[3]);
                csvLocation = args[4];
                onPremMysiteUrl = args[5];
                onPremAdmin = args[6];
                onPremAdminPWD = ToSecureString(args[7]);
                overwrite = Convert.ToBoolean(args[8]);
            }
            catch (Exception)
            {
                Console.WriteLine("Something was wrong with the parameters!");
                Console.WriteLine();
                Console.WriteLine("Parameters:");
                Console.WriteLine(@"0 - Sharepoint Online Admin Url ('https://poc-admin.sharepoint.com')");
                Console.WriteLine(@"1 - Sharepoint Online Onedrive url with placeholder ('https://poc-my.sharepoint.com/personal/{0}_poc_onmicrosoft_com')");
                Console.WriteLine(@"2 - Sharepoint Online Admin name ('admin@poc.onmicrosoft.com')");
                Console.WriteLine(@"3 - Sharepoint Online Admin password ('pass@word1')");
                Console.WriteLine(@"4 - path to CSV File ('C:\temp\users.csv')");
                Console.WriteLine(@"5 - Onprem mysite url with placeholder ('http://mysite/personal/{0}')");
                Console.WriteLine(@"6 - Onprem admin name ('admin')");
                Console.WriteLine(@"7 - Onprem admin password ('password')");
                Console.WriteLine(@"8 - Overwrite files in SPO (true/false)");

                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.Read();
                return;
            }

            List<User> users = ReadUsersFromCSV(csvLocation);
            foreach (User user in users)
            {
                string spoMysiteUrlNew = string.Format(spoMysiteUrl, user.SpoOneDriveUserName);

                List<string> folders = new List<string>();
                SharePointOnlineCredentials creds = new SharePointOnlineCredentials(adminUserName, adminPwd);
                NetworkCredential onpremCreds = new NetworkCredential(onPremAdmin, onPremAdminPWD);

                folders = GetOnPremFolderStructure(user.OnPremUserName, PersonalDocumentsName, onPremMysiteUrl, onpremCreds);
                UploadDocuments(creds, spoMysiteUrlNew, folders, user.OnPremUserName, PersonalDocumentsName, onPremMysiteUrl, onpremCreds, overwrite);

                folders = GetOnPremFolderStructure(user.OnPremUserName, PicturesName, onPremMysiteUrl, onpremCreds);
                UploadDocuments(creds, spoMysiteUrlNew, folders, user.OnPremUserName, PicturesName, onPremMysiteUrl, onpremCreds, overwrite);

                folders = GetOnPremFolderStructure(user.OnPremUserName, SharedDocumentsName, onPremMysiteUrl, onpremCreds);
                UploadDocuments(creds, spoMysiteUrlNew, folders, user.OnPremUserName, SharedDocumentsName, onPremMysiteUrl, onpremCreds, overwrite);
            }
        }

        /// <summary>
        /// This method reads csv file and maps properties to user entity
        /// </summary>
        /// <param name="fileLocation">Location of the csv file</param>
        /// <returns>List of Users</returns>
        private static List<User> ReadUsersFromCSV(string fileLocation)
        {
            List<User> users = new List<User>();
            var rowsOriginal = System.IO.File.ReadAllLines(fileLocation);
            // Remove header row
            var rows = rowsOriginal.Skip(1);
            foreach (string s in rows)
            {
                string[] cols = s.Split(';');
                User user = new User();
                user.SpoOneDriveUserName = cols[0];
                user.SpoOneDriveUserEmail = cols[1];
                user.OnPremUserName = cols[2];
                users.Add(user);
            }

            return users;
        }

        /// <summary>
        /// Creates folder structure with full paths
        /// </summary>
        /// <param name="ctx">ClientContext</param>
        /// <param name="ParentFolder">Starting folder</param>
        /// <param name="FolderPath">Tail of the path</param>
        /// <returns>Folder</returns>
        private static Folder EnsureFolder(ClientContext ctx, Folder ParentFolder, string FolderPath)
        {
            //Split up the incoming path so we have the first element as the a new sub-folder name 
            //and add it to ParentFolder folders collection
            string[] PathElements = FolderPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (PathElements.Count() > 0)
            {
                string Head = PathElements[0];
                Folder NewFolder = ParentFolder.Folders.Add(Head);
                ctx.Load(NewFolder);
                ctx.ExecuteQuery();

                //If we have subfolders to create then the length of PathElements will be greater than 1
                if (PathElements.Length > 1)
                {
                    //If we have more nested folders to create then reassemble the folder path using what we have left i.e. the tail
                    string Tail = string.Empty;
                    for (int i = 1; i < PathElements.Length; i++)
                        Tail = Tail + "/" + PathElements[i];

                    //Then make a recursive call to create the next subfolder
                    return EnsureFolder(ctx, NewFolder, Tail);
                }
                else
                    //This ensures that the folder at the end of the chain gets returned
                    return NewFolder;
            }
            return null;
        }
        /// <summary>
        /// Fetches On-Prem documents and their folder structure 
        /// Uploads them to SharePoint Online to respective folders.
        /// </summary>
        /// <param name="spoCredentials">SharePointOnlineCredetials</param>
        /// <param name="tenantAdminUrl">Url to tenant admin site</param>
        /// <param name="folders">List of folders to be created</param>
        /// <param name="userName">On-prem username as in mysite url</param>
        /// <param name="sourceLibraryName">source library name</param>
        /// <param name="onPremMysiteUrl">Onpremises mysite url for the users</param>
        /// <param name="onpremCreds">Onprem credentials</param>
        /// <param name="overwrite">Overwrite existing files</param>
        public static void UploadDocuments(SharePointOnlineCredentials spoCredentials, string tenantAdminUrl, List<string> folders,
            string userName, string sourceLibraryName, string onPremMysiteUrl, NetworkCredential onpremCreds, bool overwrite)
        {
            using (ClientContext context = new ClientContext(tenantAdminUrl))
            {
                try
                {
                    context.AuthenticationMode = ClientAuthenticationMode.Default;
                    context.Credentials = spoCredentials;
                    context.ExecuteQuery();
                    List docs = context.Web.Lists.GetByTitle(SPODocumentsName);

                    context.ExecuteQuery();
                    var folder = docs.RootFolder;
                    context.Load(folder);
                    context.ExecuteQuery();

                    context.Load(context.Web);
                    context.ExecuteQuery();

                    string SPOUrl = context.Web.ServerRelativeUrl;

                    // Create onprem folder structure to SPO
                    foreach (string f in folders)
                    {
                        // Shared documents name is different in SPO
                        if (f.Contains(SharedDocumentsName))
                        {
                            EnsureFolder(context, folder, SPOSharedDocumentsName);
                        }
                        else
                        {
                            EnsureFolder(context, folder, f);
                        }
                    }

                    onPremMysiteUrl = string.Format(onPremMysiteUrl, userName);
                    using (ClientContext opContext = new ClientContext(onPremMysiteUrl))
                    {
                        opContext.Credentials = onpremCreds;
                        opContext.Load(opContext.Web);
                        opContext.ExecuteQuery();

                        List documentsList = opContext.Web.Lists.GetByTitle(sourceLibraryName);
                        opContext.Load(documentsList);
                        opContext.ExecuteQuery();

                        CamlQuery query = CamlQuery.CreateAllItemsQuery();
                        ListItemCollection items = documentsList.GetItems(query);
                        opContext.Load(items, items2 => items2.IncludeWithDefaultProperties
                            (item => item.DisplayName, item => item.File));
                        opContext.ExecuteQuery();
                        foreach (ListItem item in items)
                        {
                            if (item.FileSystemObjectType != FileSystemObjectType.Folder)
                            {
                                string itemUrl = item.File.ServerRelativeUrl;
                                itemUrl = itemUrl.Substring(itemUrl.IndexOf(sourceLibraryName));

                                // Library names are different in SPO (2013) than in 2010 mysite
                                if (sourceLibraryName == PicturesName)
                                {
                                    itemUrl = "/" + itemUrl;
                                }
                                else if (sourceLibraryName == SharedDocumentsName)
                                {
                                    itemUrl = "/" + itemUrl.Replace(SharedDocumentsName, SPOSharedDocumentsName);
                                }
                                else
                                {
                                    itemUrl = itemUrl.Substring(itemUrl.IndexOf("/"));
                                }
                                try
                                {
                                    // Get the file from mysite and upload the file to spo
                                    string SPOLocation = SPOUrl + "/" + SPODocumentsName + itemUrl;
                                    FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(opContext, item.File.ServerRelativeUrl);
                                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, SPOLocation, fileInfo.Stream, overwrite);

                                }
                                catch (ClientRequestException crex)
                                {
                                    if (crex.Message.ToLower().Contains("exists"))
                                    {
                                        Console.WriteLine(crex.Message);
                                    }
                                    else
                                        throw crex;
                                }
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Opps, something went wrong and we need to work on it. The error message is {0}", ex.Message));
                }
            }
        }

        /// <summary>
        /// Gets the on-premises mysite folder structure
        /// </summary>
        /// <param name="userName">username as in mysite url</param>
        /// <param name="docLibname">Starting library name like "Personal Documents"</param>
        /// <param name="onPremMysiteUrl">Url to mysite</param>
        /// <param name="onpremCreds">Credentials for onprem</param>
        /// <returns>List of folder Paths</returns>
        private static List<string> GetOnPremFolderStructure(string userName, string docLibname, string onPremMysiteUrl, NetworkCredential onpremCreds)
        {
            List<string> folders = new List<string>();

            onPremMysiteUrl = string.Format(onPremMysiteUrl, userName);
            using (ClientContext context = new ClientContext(onPremMysiteUrl))
            {
                try
                {
                    context.Credentials = onpremCreds;
                    context.AuthenticationMode = ClientAuthenticationMode.Default;
                    Web web = context.Web;
                    context.Load(web);
                    context.ExecuteQuery();

                    ListCollection lists = web.Lists;

                    List docs = context.Web.Lists.GetByTitle(docLibname);
                    context.Load(docs);
                    context.ExecuteQuery();

                    var rootFolder = docs.RootFolder;
                    context.Load(rootFolder);
                    context.ExecuteQuery();

                    string url = string.Empty;
                    // Add the root folder url
                    if (docLibname == PicturesName)
                    {
                        url = PicturesName + "/";
                    }
                    else if (docLibname == SharedDocumentsName)
                    {
                        url = SharedDocumentsName + "/";
                    }
                    folders.Add(url);

                    FolderCollection collFolder = docs.RootFolder.Folders;
                    context.Load(collFolder);
                    context.ExecuteQuery();


                    ParseFolderStructure(folders, context, collFolder, docLibname);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Opps, something went wrong and we need to work on it. The error message is {0}", ex.Message));
                    throw;
                }
            }

            return folders;
        }

        /// <summary>
        /// Recursively goes through folder structure and adds the path to list
        /// </summary>
        /// <param name="folders">list of paths</param>
        /// <param name="context">ClientContext</param>
        /// <param name="collFolder">Collection of folders</param>
        /// <param name="folderName">Name of the starting folder</param>
        private static void ParseFolderStructure(List<string> folders, ClientContext context, FolderCollection collFolder, string folderName)
        {
            if (collFolder.Count > 0)
            {
                foreach (Folder folder in collFolder)
                {
                    context.Load(folder);
                    context.ExecuteQuery();

                    // Do not add forms folders or pictures thumbnail folders
                    if (!folder.Name.ToLower().Contains("forms") && !folder.Name.StartsWith("_"))
                    {
                        // strip away the beginning of the folder structure
                        string url = folder.ServerRelativeUrl;
                        url = url.Substring(url.IndexOf(folderName));
                        if (folderName != PicturesName)
                        {
                            url = url.Substring(url.IndexOf("/"));
                        }
                        folders.Add(url);
                        context.Load(folder.Folders);
                        context.ExecuteQuery();

                        if (folder.Folders.Count > 0)
                        {
                            ParseFolderStructure(folders, context, folder.Folders, folderName);
                        }
                    }
                }
            }
        }

        public static SecureString StringToSecure(string nonSecureString)
        {
            SecureString _secureString = new SecureString();
            foreach (char _c in nonSecureString)
                _secureString.AppendChar(_c);
            return _secureString;
        }

        public static SecureString ToSecureString(string Source)
        {
            if (string.IsNullOrWhiteSpace(Source))
                return null;
            else
            {
                SecureString Result = new SecureString();
                foreach (char c in Source.ToCharArray())
                    Result.AppendChar(c);
                return Result;
            }
        }
    }
}
