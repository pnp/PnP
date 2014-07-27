using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Core.LargeFileUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            // Request Office365 site from the user
            string siteUrl = GetSite();

            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", siteUrl);

            string userName = GetUserName();
            SecureString pwd = GetPassword();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null))
                return;

            ClientContext ctx = new ClientContext(siteUrl);
            ctx.AuthenticationMode = ClientAuthenticationMode.Default;
            ctx.Credentials = new SharePointOnlineCredentials(userName, pwd);

            // First the failing part
            try
            {
                // Works for smaller files and will cause an exception now
                new FileUploadService().UploadDocumentContent(ctx, "Docs", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SP2013_LargeFile1.pptx"));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Exception while uploading file to the target site {0}.", ex.ToString()));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press enter to continue.");
                Console.Read();
                
            }

            // These should both work as expected.
            try
            {
                // Alternate 1 for uploading large files 
                new FileUploadService().SaveBinaryDirect(ctx, "Docs", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SP2013_LargeFile1.pptx"));
                // Alternate 2 for uploading large files
                new FileUploadService().UploadDocumentContentStream(ctx, "Docs", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SP2013_LargeFile2.pptx"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Exception while uploading files to the target site: {0}.", ex.ToString()));
                Console.WriteLine("Press enter to continue.");
                Console.Read();
            }
            // Just to see what we have in console
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("All good with two big files uploaded to the Office365, press enter to continue.");
            Console.Read();
        }


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
    }
   
}
