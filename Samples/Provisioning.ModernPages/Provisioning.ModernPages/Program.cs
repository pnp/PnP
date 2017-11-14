using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Pages;
using System;
using System.Linq;
using System.Security;

namespace Provisioning.ModernPages
{
    class Program
    {
        static void Main(string[] args)
        {

            // Update the below variables to use your tenant and account information
            string siteUrl = "https://officedevpnp.sharepoint.com/sites/spfx-paolopia/";
            string userName = "paolo@officedevpnp.onmicrosoft.com";
            SecureString password = GetSecureString("Password");

            AuthenticationManager am = new AuthenticationManager();
            using (var cc = am.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, userName, password))
            {
                // Demo 1: Add empty page
                var page = cc.Web.AddClientSidePage("PnPRocks.aspx", true);

                // Demo 2: Read existing page and Text control + custom web part. 
                // Important: manually create a sample modern page called "Templatepage" in your demo site first
                ClientSidePage p = ClientSidePage.Load(cc, "Templatepage.aspx");
                // Add text control on top
                ClientSideText txt1 = new ClientSideText() { Text = "PnP Rocks" };
                p.AddControl(txt1, -1);
                // Find custom component and add as last control
                // Important: this assumes you've a custom client side web part with name "HelloWorld" deployed to the test site collection. 
                var components = p.AvailableClientSideComponents();
                var myWebPart = components.Where(s => s.Name == "AzureCDNSample").FirstOrDefault();
                if (myWebPart != null)
                {
                    ClientSideWebPart helloWp = new ClientSideWebPart(myWebPart) { Order = 10 };
                    p.AddControl(helloWp);
                }
                // Save the page under a new name
                p.Save("pagewithcontrols.aspx");

                //Demo 3: Add OOB web part (Image)
                // Important: if you don't update the web part properties your client side impage web part will not be able to load the image
                ClientSidePage page5 = new ClientSidePage(cc);
                var imageWebPart = page5.InstantiateDefaultWebPart(DefaultClientSideWebParts.Image);
                imageWebPart.Properties["imageSourceType"] = 2;
                imageWebPart.Properties["siteId"] = "c827cb03-d059-4956-83d0-cd60e02e3b41";
                imageWebPart.Properties["webId"] = "9fafd7c0-e8c3-4a3c-9e87-4232c481ca26";
                imageWebPart.Properties["listId"] = "78d1b1ac-7590-49e7-b812-55f37c018c4b";
                imageWebPart.Properties["uniqueId"] = "3C27A419-66D0-4C36-BF24-BD6147719052";
                imageWebPart.Properties["imgWidth"] = 1002;
                imageWebPart.Properties["imgHeight"] = 469;
                //imageWebPart.PropertiesJson = "{\"imageSourceType\":2,\"altText\":\"\",\"fileName\":\"\",\"siteId\":\"c827cb03-d059-4956-83d0-cd60e02e3b41\",\"webId\":\"9fafd7c0-e8c3-4a3c-9e87-4232c481ca26\",\"listId\":\"78d1b1ac-7590-49e7-b812-55f37c018c4b\",\"uniqueId\":\"{3C27A419-66D0-4C36-BF24-BD6147719052}\",\"imgWidth\":1002,\"imgHeight\":469}";
                page5.AddControl(imageWebPart);
                page5.Save("pagewithimage.aspx");

                //Demo 4: delete page
                ClientSidePage p2 = ClientSidePage.Load(cc, "pagewithcontrols.aspx");
                p2.Delete();

                //Demo 5: delete control
                ClientSidePage deleteDemoPage = ClientSidePage.Load(cc, "pagewithimage.aspx");
                deleteDemoPage.Controls[0].Delete();
                deleteDemoPage.Save();
            }
        }

        #region Helper methods
        private static SecureString GetSecureString(string label)
        {
            SecureString sStrPwd = new SecureString();
            try
            {
                Console.Write(String.Format("{0}: ", label));

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

        private static String GetString(string label)
        {
            String sStrPwd = "";
            try
            {
                Console.Write(String.Format("{0}: ", label));

                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (sStrPwd.Length > 0)
                        {
                            //sStrPwd.RemoveAt(sStrPwd.Length - 1);
                            sStrPwd.Remove(sStrPwd.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        //sStrPwd.AppendChar(keyInfo.KeyChar);
                        sStrPwd = sStrPwd + keyInfo.KeyChar;
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
        #endregion
    }

}
