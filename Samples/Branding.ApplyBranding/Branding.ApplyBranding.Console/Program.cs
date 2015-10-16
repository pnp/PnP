using Microsoft.SharePoint.Client;
using System;
using System.Security;
using System.Xml.Linq;

namespace Contoso.Branding.ApplyBranding
{
    enum Mode { activate, deactivate, activateIncremental, debug, invalid }

    class Program {
        internal static char[] trimChars = new char[] { '/' };

        static void Main(string[] args) {
            //check to ensure there's at least one argument
            var mode = GetMode(args);
            if (mode == Mode.invalid || args.Length > 2) {
                DisplayUsage();
                return;
            }

            var isOnline = false;
            SharePointOnlineCredentials credentials = null;
            var lastTimeRun = DateTime.MinValue;

            if (mode == Mode.debug) {
                //if we're in debug, to to the project directory and read the Branding files and settings.xml from there
                var dir = System.IO.Directory.GetCurrentDirectory();
                dir = dir.Substring(0, dir.IndexOf("\\bin"));
                System.IO.Directory.SetCurrentDirectory(dir);
            }

            if (mode == Mode.debug || mode == Mode.activateIncremental) {
                lastTimeRun = GetLastRun();
            }

            if (args.Length > 1) {
                //assuming online
                if (args[1] == "online") {
                    isOnline = true;
                    //only relevant if SharePoint Online
                    var username = GetUserName();
                    var password = GetPassword();
                    credentials = new SharePointOnlineCredentials(username, password);
                }
            }

            //activate or deactivate the branding
            var branding = XDocument.Load("settings.xml").Element("branding");
            var url = branding.Attribute("url").Value;

            foreach (var site in branding.Element("sites").Descendants("site")) {
                var siteUrl = url.TrimEnd(trimChars) + "/" + site.Attribute("url").Value.TrimEnd(trimChars);
                using (ClientContext clientContext = new ClientContext(siteUrl)) {
                    if (isOnline) {
                        clientContext.Credentials = credentials;
                    }

                    clientContext.Load(clientContext.Web);
                    clientContext.ExecuteQuery();
                    switch (mode) {
                        case Mode.activate:
                        case Mode.activateIncremental:
                        case Mode.debug:
                            UploadFiles(clientContext, branding, lastTimeRun);
                            UploadMasterPages(clientContext, branding, lastTimeRun);
                            UploadPageLayouts(clientContext, branding, lastTimeRun);
                            SaveTimeStamp();
                            break;
                        case Mode.deactivate:
                            RemoveFiles(clientContext, branding);
                            RemoveMasterPages(clientContext, branding);
                            RemovePageLayouts(clientContext, branding);
                            break;
                    }
                }
            }

            Console.WriteLine("Done!");

            if (mode != Mode.debug) {
                Console.ReadLine();
            }
            else {
                Console.WriteLine("Closing in 2 seconds...");
                System.Threading.Thread.Sleep(2000);
            }

        }

        #region "activate branding functions"

        private static void UploadFiles(ClientContext clientContext, XElement branding) {
            UploadFiles(clientContext, branding, DateTime.MinValue);
        }

        private static void UploadFiles(ClientContext clientContext, XElement branding, DateTime lastRun) {
            foreach (var file in branding.Element("files").Descendants("file")) {
                var name = file.Attribute("name").Value;
                var folder = file.Attribute("folder").Value.TrimEnd(trimChars);
                var path = file.Attribute("path").Value.TrimEnd(trimChars);

                //get the last modified time of the file
                var fileLastUpdated = System.IO.File.GetLastWriteTime(System.IO.Path.Combine("Branding\\Files\\", name));

                if (fileLastUpdated > lastRun) {
                    BrandingHelper.UploadFile(clientContext, name, folder, path);
                }

            }
        }

        private static void UploadMasterPages(ClientContext clientContext, XElement branding) {
            UploadMasterPages(clientContext, branding, DateTime.MinValue);
        }

        private static void UploadMasterPages(ClientContext clientContext, XElement branding, DateTime lastRun) {
            foreach (var masterpage in branding.Element("masterpages").Descendants("masterpage")) {
                var name = masterpage.Attribute("name").Value;
                var folder = masterpage.Attribute("folder").Value.TrimEnd(new char[] { '/' });

                //get the last modified time of the file
                var fileLastUpdated = System.IO.File.GetLastWriteTime(System.IO.Path.Combine("Branding\\MasterPages\\", name));

                if (fileLastUpdated > lastRun) {
                    BrandingHelper.UploadMasterPage(clientContext, name, folder);
                }
            }
        }

        private static void UploadPageLayouts(ClientContext clientContext, XElement branding, DateTime lastRun) {
            foreach (var pagelayout in branding.Element("pagelayouts").Descendants("pagelayout")) {
                var name = pagelayout.Attribute("name").Value;
                var folder = pagelayout.Attribute("folder").Value.TrimEnd(trimChars);
                var publishingAssociatedContentType = pagelayout.Attribute("publishingAssociatedContentType").Value;
                var title = pagelayout.Attribute("title").Value;

                //get the last modified time of the file
                var fileLastUpdated = System.IO.File.GetLastWriteTime(System.IO.Path.Combine("Branding\\PageLayouts\\", name));

                if (fileLastUpdated > lastRun) {
                    BrandingHelper.UploadPageLayout(clientContext, name, folder, title, publishingAssociatedContentType);
                }
            }
        }

        private static void UploadPageLayouts(ClientContext clientContext, XElement branding) {
            UploadPageLayouts(clientContext, branding, DateTime.MinValue);
        }

        #endregion

        #region "deactivate branding functions"

        private static void RemoveFiles(ClientContext clientContext, XElement branding) {
            var name = "";
            var folder = "";
            var path = "";
            foreach (var file in branding.Element("files").Descendants("file")) {
                name = file.Attribute("name").Value;
                folder = file.Attribute("folder").Value.TrimEnd(trimChars);
                path = file.Attribute("path").Value.TrimEnd(trimChars);

                BrandingHelper.RemoveFile(clientContext, name, folder, path);
            }
            BrandingHelper.RemoveFolder(clientContext, folder, path);
        }

        private static void RemoveMasterPages(ClientContext clientContext, XElement branding) {
            var name = "";
            var folder = "";
            foreach (var masterpage in branding.Element("masterpages").Descendants("masterpage")) {
                name = masterpage.Attribute("name").Value;
                folder = masterpage.Attribute("folder").Value.TrimEnd(new char[] { '/' });

                BrandingHelper.RemoveMasterPage(clientContext, name, folder);
            }
            BrandingHelper.RemoveFolder(clientContext, folder, "_catalogs/masterpage");
        }

        private static void RemovePageLayouts(ClientContext clientContext, XElement branding) {
            foreach (var pagelayout in branding.Element("pagelayouts").Descendants("pagelayout")) {
                var name = pagelayout.Attribute("name").Value;
                var folder = pagelayout.Attribute("folder").Value.TrimEnd(trimChars);
                var publishingAssociatedContentType = pagelayout.Attribute("publishingAssociatedContentType").Value;
                var title = pagelayout.Attribute("title").Value;

                BrandingHelper.RemovePageLayout(clientContext, name, folder);
            }
        }

        #endregion

        #region "helper functions"

        static SecureString GetPassword() {
            SecureString sStrPwd = new SecureString();

            try {
                Console.Write("SharePoint Password: ");

                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true)) {
                    if (keyInfo.Key == ConsoleKey.Backspace) {
                        if (sStrPwd.Length > 0) {
                            sStrPwd.RemoveAt(sStrPwd.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter) {
                        Console.Write("*");
                        sStrPwd.AppendChar(keyInfo.KeyChar);
                    }

                }
                Console.WriteLine("");
            }
            catch (Exception e) {
                sStrPwd = null;
                Console.WriteLine(e.Message);
            }

            return sStrPwd;
        }

        static string GetUserName() {
            string strUserName = string.Empty;
            try {
                Console.Write("SharePoint Username: ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        static void DisplayUsage() {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please specify 'activate' or 'deactivate' and optionally 'online'");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Example 1 (SharePoint Online): \n Contoso.Branding.ApplyBranding.Console.exe activate online");
            Console.WriteLine("Example 2 (SharePoint Online):  \n Contoso.Branding.ApplyBranding.Console.exe deactivate online");
            Console.WriteLine("Example 3 (SharePoint On-premises):  \n Contoso.Branding.ApplyBranding.Console.exe activate");
            Console.WriteLine("Example 4 (SharePoint On-premises):  \n Contoso.Branding.ApplyBranding.Console.exe deactivate");
            Console.WriteLine("Example 5 (SharePoint Online):  \n Contoso.Branding.ApplyBranding.Console.exe activateIncremental online");
            Console.WriteLine("Example 6 (SharePoint On-premises):  \n Contoso.Branding.ApplyBranding.Console.exe activateIncremental");
            Console.ResetColor();
            Console.ReadLine();
        }

        static void SaveTimeStamp() {
            try {
                var timeStampFile = "lastrun.log";
                if (!System.IO.File.Exists(timeStampFile)) {
                    using (System.IO.File.Create(timeStampFile)) { }
                }

                System.IO.File.WriteAllLines(timeStampFile, new string[] { DateTime.Now.ToString() });
            }
            catch (Exception ex) {

            }
        }

        static DateTime GetLastRun() {
            var value = DateTime.MinValue;

            var timeStampFile = "lastrun.log";
            if (System.IO.File.Exists(timeStampFile)) {
                var lines = System.IO.File.ReadAllLines(timeStampFile);
                if (lines.Length > 0) {
                    var line1 = lines[0];

                    DateTime.TryParse(line1, out value);
                }
            }

            return value;
        }

        static Mode GetMode(string[] args) {
            if (args.Length > 0) {
                try {
                    var mode = args[0];
                    return (Mode)Enum.Parse(typeof(Mode), mode);
                }
                catch (Exception) {
                }
            }

            return Mode.invalid;
        }

        #endregion
    }
}
