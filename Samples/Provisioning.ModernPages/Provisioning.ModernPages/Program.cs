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
            //string siteUrl = "https://contoso.sharepoint.com/sites/modernpagedemo";
            //string userName = "pnp@contoso.onmicrosoft.com";
            string siteUrl = "https://bertonline.sharepoint.com/sites/bert1";
            string userName = "bert.jansen@bertonline.onmicrosoft.com";
            SecureString password = GetSecureString("Password");

            AuthenticationManager am = new AuthenticationManager();
            using (var cc = am.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, userName, password))
            {
                #region Previous demos
                /*
                                // Demo 1: Add empty page
                                var page = cc.Web.AddClientSidePage("PnPRocks.aspx", true);

                                // Demo 2: Read existing page and Text control + custom web part. 
                                // Important: manually create a sample modern page called "Templatepage" in your demo site first
                                ClientSidePage p = ClientSidePage.Load(cc, "Demo.aspx");
                                // Add text control on top
                                ClientSideText txt1 = new ClientSideText() { Text = "PnP Rocks" };
                                p.AddControl(txt1, -1);
                                // Find custom component and add as last control
                                // Important: this assumes you've a custom client side web part with name "HelloWorld" deployed to the test site collection. 
                                var components = p.AvailableClientSideComponents();
                                var myWebPart = components.Where(s => s.Name == "HelloWorld").FirstOrDefault();
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
                */
                #endregion
                
                //Demo 6: create a page with a flexible page layout
                var page2 = cc.Web.AddClientSidePage("PageWithSections.aspx", true);
                page2.AddZone(CanvasZoneTemplate.ThreeColumn, 5);
                page2.AddZone(CanvasZoneTemplate.TwoColumn, 10);

                var componentsToAdd = page2.AvailableClientSideComponents();
                var myHellowWorldWebPart = componentsToAdd.Where(s => s.Name == "HelloWorld").FirstOrDefault();
                if (myHellowWorldWebPart != null)
                {
                    ClientSideText text = new ClientSideText()
                    {
                        Text = "Text control in first zone, left column"
                    };
                    page2.AddControl(text, page2.Zones[0]);

                    ClientSideWebPart helloWp = new ClientSideWebPart(myHellowWorldWebPart) { Order = 10 };
                    helloWp.Properties["description"] = "Hello world from control 1!!";
                    helloWp.Properties["test3"] = false;
                    helloWp.Properties["test2"] = "1";
                    //description":"HelloWorld","test":"Multi-line text field","test1":true,"test2":"2","test3":true
                    page2.AddControl(helloWp, page2.Zones[0].Sections[2]);

                    ClientSideWebPart helloWp2 = new ClientSideWebPart(myHellowWorldWebPart) { Order = 10 };
                    helloWp2.Properties["description"] = "Hello world from control 2!!";
                    helloWp2.Properties["test3"] = true;
                    helloWp2.Properties["test2"] = "3";
                    //description":"HelloWorld","test":"Multi-line text field","test1":true,"test2":"2","test3":true
                    page2.AddControl(helloWp2, page2.Zones[1]);
                }
                page2.Save();

                // Demo 7: read created page and flip the order of the sections, move web parts around and turn off commenting
                var page3 = cc.Web.LoadClientSidePage("PageWithSections.aspx");
                // Move web part
                page3.Zones[0].Sections[0].Controls[0].Move(page3.Zones[0].Sections[2], 20);
                // Move zones
                page3.Zones[0].Order = 10;
                page3.Zones[1].Order = 5;
                ClientSideText text2 = new ClientSideText()
                {
                    Text = "Text control in first zone, middle column"
                };
                page3.AddControl(text2, page3.Zones[0].Sections[1]);
                page3.Save();
                // Disable comments for this page
                page3.DisableComments();

                // Demo 8: create a news page
                ClientSidePage page4 = new ClientSidePage(cc);
                page4.AddZone(CanvasZoneTemplate.TwoColumn, 10);
                ClientSideText text4 = new ClientSideText()
                {
                    Text = "Hello all, this is our first news page."
                };
                page4.AddControl(text4);

                var imageWebPart4 = page4.InstantiateDefaultWebPart(DefaultClientSideWebParts.Image);
                imageWebPart4.Properties["imageSourceType"] = 2;
                imageWebPart4.Properties["siteId"] = "c827cb03-d059-4956-83d0-cd60e02e3b41";
                imageWebPart4.Properties["webId"] = "9fafd7c0-e8c3-4a3c-9e87-4232c481ca26";
                imageWebPart4.Properties["listId"] = "78d1b1ac-7590-49e7-b812-55f37c018c4b";
                imageWebPart4.Properties["uniqueId"] = "3C27A419-66D0-4C36-BF24-BD6147719052";
                imageWebPart4.Properties["imgWidth"] = 500;
                imageWebPart4.Properties["imgHeight"] = 235;
                //imageWebPart.PropertiesJson = "{\"imageSourceType\":2,\"altText\":\"\",\"fileName\":\"\",\"siteId\":\"c827cb03-d059-4956-83d0-cd60e02e3b41\",\"webId\":\"9fafd7c0-e8c3-4a3c-9e87-4232c481ca26\",\"listId\":\"78d1b1ac-7590-49e7-b812-55f37c018c4b\",\"uniqueId\":\"{3C27A419-66D0-4C36-BF24-BD6147719052}\",\"imgWidth\":1002,\"imgHeight\":469}";
                page4.AddControl(imageWebPart4, page4.DefaultZone.Sections[1]);
                page4.Save("newspage.aspx");
                // promote as news
                page4.PromoteAsNewsArticle();

                // Demo 9: give the site a new home page
                var newHomePage = cc.Web.AddClientSidePage();
                newHomePage.LayoutType = ClientSidePageLayoutType.Home;
                newHomePage.AddZone(CanvasZoneTemplate.ThreeColumn, 10);
                newHomePage.AddControl(new ClientSideText() { Text = "Having a custom home page" }, newHomePage.DefaultZone);
                newHomePage.AddControl(new ClientSideText() { Text = "with multiple columns" }, newHomePage.DefaultZone.Sections[1]);
                newHomePage.AddControl(new ClientSideText() { Text = "is possible :-)" }, newHomePage.DefaultZone.Sections[2]);
                newHomePage.Save("Home_2.aspx");
                newHomePage.PromoteAsHomePage();

                // Demo 10: restore original home page
                cc.Web.LoadClientSidePage("Home.aspx").PromoteAsHomePage(); 
                
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
