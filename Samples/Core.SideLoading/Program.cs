//*********************************************************
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.SideLoading
{

    class Program
    {
        static void Main(string[] args)
        {
            Guid _sideloadingFeature = new Guid("AE3A1339-61F5-4f8f-81A7-ABD2DA956A7D");

            string _url = GetUserInput("Please Supply the SharePoint Online Site Collection URL: ");
            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", _url);

            string _userName = GetUserInput("SharePoint Username: ");
            SecureString _pwd = GetPassword();
            ClientContext _ctx = new ClientContext(_url);
            _ctx.ApplicationName = "AMS SIDELOADING SAMPLE";
            _ctx.AuthenticationMode = ClientAuthenticationMode.Default;
            //For SharePoint Online
            _ctx.Credentials = new SharePointOnlineCredentials(_userName, _pwd);

            string _path = GetUserInput("Please supply path to your app package:");

            Site _site = _ctx.Site;
            Web _web = _ctx.Web;

            try
            {
                _ctx.Load(_web);
                _ctx.ExecuteQuery();

                //Make sure we have side loading enabled. You must be a tenant admin to activate or you will get an exception! The ProcessFeature is an extension method,
                _site.ProcessFeature(_sideloadingFeature, true);
                try
                {
                    var _appstream = System.IO.File.OpenRead(_path);
                    AppInstance _app = _web.LoadAndInstallApp(_appstream);
                    _ctx.Load(_app);
                    _ctx.ExecuteQuery();
                }
                catch
                {
                    throw;
                }

                //we should ensure that the side loading feature is disable when we are done or if an exception occurs 
                _site.ProcessFeature(_sideloadingFeature, false);

            }
            catch (Exception _ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Exception!"), _ex.ToString());
                Console.WriteLine("Press any key to continue.");
                Console.Read();
            }
        }


        /// <summary>
        /// Helper to get User Input from the console
        /// </summary>
        /// <returns></returns>
        public static string GetUserInput(string message)
        {
            string _path = string.Empty;
            Console.Write(message);
            _path = Console.ReadLine();
           
            return _path;
        }
        /// <summary>
        /// Helper to return the password
        /// </summary>
        /// <returns>SecureString representing the password</returns>
        public static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();

            try
            {
                Console.Write("SharePoint Password: ");

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


    }
}
