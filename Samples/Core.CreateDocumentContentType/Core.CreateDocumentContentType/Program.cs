using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;


namespace Core.CreateDocumentContentType
{
    class Program
    {
        /// <summary>
        /// Assumption
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string siteUrl = GetSite();

            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", siteUrl);

            string userName = GetUserName();
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
            CreateContentTypeIfDoesNotExist(cc, web);
            SetDocumentAsTemplate(cc, web);

            // Just to pause
            Console.WriteLine("Modification completed successfully, press enter to continue");
            Console.ReadLine();
        }

        private static void CreateContentTypeIfDoesNotExist(ClientContext cc, Web web)
        {
            ContentTypeCollection contentTypes = web.ContentTypes;
            cc.Load(contentTypes);
            cc.ExecuteQuery();

            foreach (var item in contentTypes)
            {
                if (item.StringId == "0x0101009189AB5D3D2647B580F011DA2F356FB3")
                    return;
            }

            // Create a Content Type Information object
            ContentTypeCreationInformation newCt = new ContentTypeCreationInformation();
            // Set the name for the content type
            newCt.Name = "Contoso Sample Document";
            //Inherit from oob document - 0x0101 and assign 
            newCt.Id = "0x0101009189AB5D3D2647B580F011DA2F356FB3";
            // Set content type to be avaialble from specific group
            newCt.Group = "Contoso Content Types";
            // Create the content type
            ContentType myContentType = contentTypes.Add(newCt);
            cc.ExecuteQuery();

            Console.WriteLine("Content type created.");
        }

        private static void SetDocumentAsTemplate(ClientContext cc, Web web)
        {
            ContentType ct = web.ContentTypes.GetById("0x0101009189AB5D3D2647B580F011DA2F356FB3");
            cc.Load(ct); 
            cc.ExecuteQuery();

            // Get instance to the _cts folder created for the each of the content types
            string ctFolderServerRelativeURL = "_cts/" + ct.Name;
            Folder ctFolder = web.GetFolderByServerRelativeUrl(ctFolderServerRelativeURL);
            cc.Load(ctFolder);
            cc.ExecuteQuery();

            // Load the local template document
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.docx");
            string fileName = System.IO.Path.GetFileName(path);
            byte[] filecontent = System.IO.File.ReadAllBytes(path);

            // Uplaod file to the Office365
            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = filecontent;
                newFile.Url = ctFolderServerRelativeURL + "/" + fileName;

                Microsoft.SharePoint.Client.File uploadedFile = ctFolder.Files.Add(newFile);
                cc.Load(uploadedFile);
                cc.ExecuteQuery();
            }


            ct.DocumentTemplate = fileName;
            ct.Update(true);
            cc.ExecuteQuery();
            Console.WriteLine("Document template uploaded and set to the content type.");
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


        #endregion

    }
}
