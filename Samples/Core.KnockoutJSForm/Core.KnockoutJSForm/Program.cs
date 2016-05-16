using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;
using System.Security;

namespace Core.KnockoutJSForm
{
    class Program
    {
        // TODO: update values before running the sample or blank them which 
        // triggers a prompt to ask for the value
        static string siteUrl = "https://<tenant>.sharepoint.com/sites/dev";
        static string username = "<user>@<tenant>.onmicrosoft.com";
        static SecureString password = null;

        private static ClientContext CreateContext()
        {
            ClientContext ctx = new ClientContext(siteUrl);
            ctx.Credentials = new SharePointOnlineCredentials(username, password);
            ctx.ExecuteQuery();

            Console.WriteLine("Connected to {0}", siteUrl);
            Console.WriteLine("");

            return ctx;
        }
        private static void ProvisionAssets(ClientContext ctx)
        {
            Console.WriteLine("Provisioning assets:");

            string[] fileNames = {
                                     "jquery-1.11.2.min.js",
                                     "knockout-3.3.0.js",
                                     "event-registration-form.js",
                                     "event-registration-form-template.js"};
            
            List styleLibrary = ctx.Web.Lists.GetByTitle("Style Library");
            ctx.Load(styleLibrary, l => l.RootFolder);
            Folder pnpFolder = styleLibrary.RootFolder.EnsureFolder("OfficeDevPnP");
            foreach (string fileName in fileNames)
            {
                Console.WriteLine(fileName);

                File assetFile = pnpFolder.GetFile(fileName);
                if (assetFile != null)
                    assetFile.CheckOut();

                string localFilePath = "Assets/" + fileName;
                string newLocalFilePath = Utilities.ReplaceTokensInAssetFile(ctx, localFilePath);

                assetFile = pnpFolder.UploadFile(fileName, newLocalFilePath, true);
                assetFile.CheckIn("Uploaded by provisioning engine.", CheckinType.MajorCheckIn);
                ctx.ExecuteQuery();
                System.IO.File.Delete(newLocalFilePath);
            }
            Console.WriteLine("");
        }

        private static void ProvisionLists(ClientContext ctx)
        {
            Console.WriteLine("Provisioning lists:");
            Console.WriteLine("Events");
            List eventsList = ctx.Web.CreateList(ListTemplateType.Events, "Events", false, false, "Lists/Events", false);
            eventsList.CreateField(@"<Field Type=""Boolean"" DisplayName=""Registration Allowed"" ID=""{d395011d-07c9-40a5-99c2-cb4d4f209d13}"" Name=""OfficeDevPnPRegistrationAllowed""><Default>1</Default></Field>", false);
            ctx.Load(eventsList);
            ctx.ExecuteQueryRetry();

            Console.WriteLine("Event Registration");
            List regList = ctx.Web.CreateList(ListTemplateType.GenericList, "Event Registration", false, false, "Lists/Event Registration", false);
            Field field = regList.CreateField(@"<Field Type=""Lookup"" DisplayName=""Event"" ID=""{39e09239-3da4-455f-9f03-add53034de0a}"" Name=""OfficeDevPnPEventLookup"" />", false);
            ctx.Load(regList);
            ctx.Load(field);
            ctx.ExecuteQueryRetry();

            // configure event lookup field
            FieldLookup eventField = ctx.CastTo<FieldLookup>(field);
            eventField.LookupList = eventsList.Id.ToString();
            eventField.LookupField = "Title";
            eventField.Indexed = true;
            eventField.IsRelationship = true;
            eventField.RelationshipDeleteBehavior = RelationshipDeleteBehaviorType.Cascade;
            eventField.Update();
            ctx.ExecuteQueryRetry();
            // configure author field
            Field authorField = regList.Fields.GetFieldByName<Field>("Author");
            authorField.Indexed = true;
            authorField.Update();
            ctx.ExecuteQueryRetry();

            Console.WriteLine("");
        }

        private static void ProvisionWebPart(ClientContext ctx)
        {
            Console.WriteLine("Provisioning web part...");
            Web web = ctx.Web;
            ctx.Load(web);
            ctx.ExecuteQueryRetry();

            string pageUrl = web.ServerRelativeUrl + "/Lists/Events/DispForm.aspx";
            File webPartPage = web.GetFileByServerRelativeUrl(pageUrl);
            ctx.Load(webPartPage);
            ctx.ExecuteQueryRetry();

            string webPartXml = System.IO.File.ReadAllText(@"Assets\WebParts\EventRegistrationInformation.dwp");
            //replace tokens
            webPartXml = Utilities.ReplaceTokens(ctx, webPartXml);
            OfficeDevPnP.Core.Entities.WebPartEntity webPart = new OfficeDevPnP.Core.Entities.WebPartEntity()
            {
                WebPartZone = "Main",
                WebPartIndex = 20,
                WebPartTitle = "Event Registration Information",
                WebPartXml = webPartXml
            };
            Console.WriteLine("Adding event registration web part to " + pageUrl);
            web.AddWebPartToWebPartPage(pageUrl, webPart);
            Console.WriteLine("");
        }

        private static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();
            try
            {
                Console.Write("SharePoint Password : ");

                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (sStrPwd.Length > 0)
                        {
                            sStrPwd.RemoveAt(sStrPwd.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        sStrPwd.AppendChar(keyInfo.KeyChar);
                    }

                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                sStrPwd = null;
                Console.WriteLine(e.Message);
            }

            return sStrPwd;
        }

        private static string GetUserName()
        {
            string strUserName = string.Empty;
            try
            {
                Console.Write("SharePoint User Name : ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        private static string GetSite()
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write("Give Office365 site URL : ");
                siteUrl = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                siteUrl = string.Empty;
            }
            return siteUrl;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Provisioning OfficeDevPnPCore.KnockoutJSForm sample started...");

            // Request Office365 site from the user
            if (String.IsNullOrEmpty(siteUrl))
            {
                siteUrl = GetSite();
            }

            /* Prompt for Credentials */
            if (String.IsNullOrEmpty(username))
            {
                username = GetUserName();
            }

            if (password == null || password.Length == 0)
            {
                password = GetPassword();
            }

            ClientContext ctx = CreateContext();
            
            ProvisionAssets(ctx);
            ProvisionLists(ctx);
            ProvisionWebPart(ctx);

            Console.WriteLine("Provisioning completed.");
            Console.Read();
        }
    }
}
