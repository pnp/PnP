using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.CreateContentTypes
{
    class Program
    {
        /// <summary>
        /// Assumptions -   Site has Finnish and Swedish language enabled in the cloud.
        ///                 Translated fields are available if the user selectes Finnish or Swedish as the default language in user profile (OneDrive site)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            string siteUrl = GetSite();

            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", siteUrl);

            string userName =  GetUserName();
            SecureString pwd = GetPassword();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null))
                return;

            // Open connection to Office365 tenant
            ClientContext cc = new ClientContext(siteUrl);
            cc.AuthenticationMode = ClientAuthenticationMode.Default;
            cc.Credentials = new SharePointOnlineCredentials(userName, pwd);

            // Load reference to content type collection
            Web web = cc.Web;

            //
            // Ensure that we have the initial config available.
            //
            CreateContentTypeIfDoesNotExist(cc, web);
            CreateSiteColumn(cc, web);
            AddSiteColumnToContentType(cc, web);
            CreateCustomList(cc, web);

            //
            // Add localization for English, Finnish and Swedish
            //
            LocalizeSiteAndList(cc, web);
            LocalizeContentTypeAndField(cc, web);
            
        }

        private static void LocalizeSiteAndList(ClientContext cc, Web web)
        {
            // Localize site title
            web.TitleResource.SetValueForUICulture("en-US", "Localize Me");
            web.TitleResource.SetValueForUICulture("fi-FI", "Kielikäännä minut");
            web.TitleResource.SetValueForUICulture("fr-FR", "Localize Me to French");
            // Site description
            web.DescriptionResource.SetValueForUICulture("en-US", "Localize Me site sample");
            web.DescriptionResource.SetValueForUICulture("fi-FI", "Kielikäännetty saitti");
            web.DescriptionResource.SetValueForUICulture("fr-FR", "Localize to French in description");
            web.Update();
            cc.ExecuteQuery();

            // Localize custom list which was created previously
            List list = cc.Web.Lists.GetByTitle("LocalizeMe");
            cc.Load(list);
            cc.ExecuteQuery();
            list.TitleResource.SetValueForUICulture("en-US", "Localize Me");
            list.TitleResource.SetValueForUICulture("fi-FI", "Kielikäännä minut");
            list.TitleResource.SetValueForUICulture("fr-FR", "French text for title");
            // Description
            list.DescriptionResource.SetValueForUICulture("en-US", "This is localization CSOM usage example list.");
            list.DescriptionResource.SetValueForUICulture("fi-FI", "Tämä esimerkki näyttää miten voit kielikääntää listoja.");
            list.DescriptionResource.SetValueForUICulture("fr-FR", "I have no idea how to translate this to French.");
            list.Update();
            cc.ExecuteQuery();
        }

        /// <summary>
        /// Used to create custom list to demonstrate the multi-lingual capabilities with the list title and decription.
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="web"></param>
        private static void CreateCustomList(ClientContext cc, Web web)
        {
            ListCollection listCollection = cc.Web.Lists;
            cc.Load(listCollection, lists => lists.Include(list => list.Title).Where(list => list.Title == "LocalizeMe"));
            cc.ExecuteQuery();
            // Create the list, if it's not there...
            if (listCollection.Count == 0)
            {
                ListCreationInformation newList = new ListCreationInformation();
                newList.Title = "LocalizeMe";
                newList.QuickLaunchOption = QuickLaunchOptions.On;
                newList.TemplateType = (int)ListTemplateType.GenericList;
                newList.Description = "LocalizeMe sample list";
                List list = web.Lists.Add(newList);
                cc.ExecuteQuery();
            }
        }

        private static void LocalizeContentTypeAndField(ClientContext cc, Web web)
        {
            ContentTypeCollection contentTypes = web.ContentTypes;
            ContentType myContentType = contentTypes.GetById("0x0101009189AB5D3D2647B580F011DA2F356FB2");
            cc.Load(contentTypes);
            cc.Load(myContentType);
            cc.ExecuteQuery();
            // Title of the content type
            myContentType.NameResource.SetValueForUICulture("en-US", "Contoso Document");
            myContentType.NameResource.SetValueForUICulture("fi-FI", "Contoso Dokumentti");
            myContentType.NameResource.SetValueForUICulture("fr-FR", "Contoso Document (FR)");
            // Description of the content type
            myContentType.DescriptionResource.SetValueForUICulture("en-US", "This is the Contoso Document.");
            myContentType.DescriptionResource.SetValueForUICulture("fi-FI", "Tämä on geneerinen Contoso dokumentti.");
            myContentType.DescriptionResource.SetValueForUICulture("fr-FR", "French Contoso document.");
            myContentType.Update(true);
            cc.ExecuteQuery();

            // Do localization also for the site column
            FieldCollection fields = web.Fields;
            Field fld = fields.GetByInternalNameOrTitle("ContosoString");
            fld.TitleResource.SetValueForUICulture("en-US", "Contoso String");
            fld.TitleResource.SetValueForUICulture("fi-FI", "Contoso Teksti");
            fld.TitleResource.SetValueForUICulture("fr-FR", "Contoso French String");
            // Description entry
            fld.DescriptionResource.SetValueForUICulture("en-US", "Used to store Contoso specific metadata.");
            fld.DescriptionResource.SetValueForUICulture("fi-FI", "Tää on niiku Contoso metadatalle.");
            fld.DescriptionResource.SetValueForUICulture("fr-FR", "French Description Goes here");
            fld.UpdateAndPushChanges(true);
            cc.ExecuteQuery();
        }

        

        private static void CreateContentTypeIfDoesNotExist(ClientContext cc, Web web)
        {
            ContentTypeCollection contentTypes = web.ContentTypes;
            cc.Load(contentTypes);
            cc.ExecuteQuery();

            foreach (var item in contentTypes)
            {
                if (item.StringId == "0x0101009189AB5D3D2647B580F011DA2F356FB2")
                    return;
            }

            // Create a Content Type Information object
            ContentTypeCreationInformation newCt = new ContentTypeCreationInformation();
            // Set the name for the content type
            newCt.Name = "Contoso Document";
            //Inherit from oob document - 0x0101 and assign 
            newCt.Id = "0x0101009189AB5D3D2647B580F011DA2F356FB2";
            // Set content type to be avaialble from specific group
            newCt.Group = "Contoso Content Types";
            // Create the content type
            ContentType myContentType = contentTypes.Add(newCt);
            cc.ExecuteQuery();
        }


        private static void CreateSiteColumn(ClientContext cc, Web web)
        {
            // Add site column to the content type if it's not there...
            FieldCollection fields = web.Fields;
            cc.Load(fields);
            cc.ExecuteQuery();

            foreach (var item in fields)
            {
                if (item.InternalName == "ContosoString")
                    return;
            }

            string FieldAsXML = @"<Field ID='{4F34B2ED-9CFF-4900-B091-4C0033F89944}' 
                                            Name='ContosoString' 
                                            DisplayName='Contoso String' 
                                            Type='Text' 
                                            Hidden='False' 
                                            Group='Contoso Site Columns' 
                                            Description='Contoso Text Field' />";
            Field fld = fields.AddFieldAsXml(FieldAsXML, true, AddFieldOptions.DefaultValue);
            cc.Load(fields);
            cc.Load(fld);
            cc.ExecuteQuery();
        }

        private static void AddSiteColumnToContentType(ClientContext cc, Web web)
        {
            ContentTypeCollection contentTypes = web.ContentTypes;
            cc.Load(contentTypes);
            cc.ExecuteQuery();
            ContentType myContentType = contentTypes.GetById("0x0101009189AB5D3D2647B580F011DA2F356FB2");
            cc.Load(myContentType);
            cc.ExecuteQuery();

            FieldCollection fields = web.Fields;
            Field fld = fields.GetByInternalNameOrTitle("ContosoString");
            cc.Load(fields);
            cc.Load(fld);
            cc.ExecuteQuery();

            FieldLinkCollection refFields = myContentType.FieldLinks;
            cc.Load(refFields);
            cc.ExecuteQuery();

            foreach (var item in refFields)
            {
                if (item.Name == "ContosoString")
                    return;
            }

            // ref does nt
            FieldLinkCreationInformation link = new FieldLinkCreationInformation();
            link.Field = fld;
            myContentType.FieldLinks.Add(link);
            myContentType.Update(true);
            cc.ExecuteQuery();
        }

        #region CONNECTIVITY METHODS

        static SecureString GetPassword()
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

        static string GetUserName()
        {
            string strUserName = string.Empty;
            try
            {
                Console.Write("SharePoint Username : ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        static string GetSite()
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write("Give Office365 site URL: ");
                siteUrl = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                siteUrl = string.Empty;
            }
            return siteUrl;
        }

        private static string URLCombine(string baseUrl, string relativeUrl)
        {
            if (baseUrl.Length == 0)
                return relativeUrl;
            if (relativeUrl.Length == 0)
                return baseUrl;
            return string.Format("{0}/{1}", baseUrl.TrimEnd(new char[] { '/', '\\' }), relativeUrl.TrimStart(new char[] { '/', '\\' }));
        }
        #endregion
    }
}
