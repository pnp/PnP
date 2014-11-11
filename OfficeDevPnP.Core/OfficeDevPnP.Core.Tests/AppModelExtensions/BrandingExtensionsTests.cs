using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.IO;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass()]
    public class BrandingExtensionsTests
    {
        private string builtInLookSeaMonster = "Sea Monster"; // oslo, palette005, image_bg005, fontscheme003
        private string builtInLookBlossom = "Blossom"; // seattle, palette002,image_bg002
        private string builtInMasterOslo = "oslo.master";
        private string builtInMasterSeattle = "seattle.master";
        private string builtInPalette003 = "palette003.spcolor";
        private string builtInFont002 = "fontscheme002.spfont";

        private string customColorFilePath = string.Empty;
        private string customBackgroundFilePath = string.Empty;
        private const string THEME_NAME = "Test_Theme";

        const string CAML_QUERY_FIND_BY_FILENAME = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Name' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";

        private string testWebName;


        [TestInitialize()]
        public void Initialize()
        {
            Console.WriteLine("BrandingExtensionsTests.Initialise");
            customColorFilePath = Path.Combine(Path.GetTempPath(), "custom.spcolor");
            System.IO.File.WriteAllBytes(customColorFilePath, OfficeDevPnP.Core.Tests.Properties.Resources.custom);
            customBackgroundFilePath = Path.Combine(Path.GetTempPath(), "custombg.jpg");
            Properties.Resources.custombg.Save(customBackgroundFilePath);

            testWebName = string.Format("Test_CL{0:yyyyMMddTHHmmss}", DateTimeOffset.Now);
            using (var context = TestCommon.CreateClientContext())
            {
                var wci1 = new WebCreationInformation();
                wci1.Url = testWebName;
                wci1.Title = testWebName;
                wci1.WebTemplate = "CMSPUBLISHING#0";
                var web1 = context.Web.Webs.Add(wci1);
                context.ExecuteQuery();

                var wci2 = new WebCreationInformation();
                wci2.Url = "a";
                wci2.Title = "A";
                wci2.WebTemplate = "CMSPUBLISHING#0";
                var webA = web1.Webs.Add(wci2);
                context.ExecuteQuery();

                var wci3 = new WebCreationInformation();
                wci3.Url = "b";
                wci3.Title = "B";
                wci3.WebTemplate = "CMSPUBLISHING#0";
                var webB = web1.Webs.Add(wci3);
                context.ExecuteQuery();
            }

        }

        [TestCleanup()]
        public void CleanUp()
        {
            Console.WriteLine("BrandingExtensionsTests.CleanUp");
            if (System.IO.File.Exists(customColorFilePath))
            {
                System.IO.File.Delete(customColorFilePath);
            }
            if (System.IO.File.Exists(customBackgroundFilePath))
            {
                System.IO.File.Delete(customBackgroundFilePath);
            }

            using (var context = TestCommon.CreateClientContext())
            {
                var web = context.Web;
                if (web.ThemeEntryExists(THEME_NAME))
                {

                    // Remove theme from server
                    List themeGallery = web.GetCatalog((int)ListTemplateType.DesignCatalog);
                    CamlQuery query = new CamlQuery();
                    string camlString = @"
                        <View>
                            <Query>                
                                <Where>
                                    <Contains>
                                        <FieldRef Name='Name' />
                                        <Value Type='Text'>Test_</Value>
                                    </Contains>
                                </Where>
                             </Query>
                        </View>";
                    query.ViewXml = camlString;
                    var found = themeGallery.GetItems(query);
                    web.Context.Load(found);
                    web.Context.ExecuteQuery();
                    foreach (var item in found)
                    {
                        item.DeleteObject();
                        context.ExecuteQuery();
                    }

                    // Remove Theme Files
                    List themesList = web.GetCatalog((int)ListTemplateType.ThemeCatalog);
                    Folder rootFolder = themesList.RootFolder;
                    FolderCollection rootFolders = rootFolder.Folders;
                    web.Context.Load(rootFolder);
                    web.Context.Load(rootFolders, f => f.Where(folder => folder.Name == "15"));
                    web.Context.ExecuteQuery();

                    Folder folder15 = rootFolders.FirstOrDefault();

                    Microsoft.SharePoint.Client.File customColorFile = folder15.Files.GetByUrl("custom.spcolor");
                    Microsoft.SharePoint.Client.File customBackgroundFile = folder15.Files.GetByUrl("custombg.jpg");

                    customColorFile.DeleteObject();
                    customBackgroundFile.DeleteObject();
                    context.ExecuteQuery();
                }

                var webCollection1 = web.Webs;
                context.Load(webCollection1, wc => wc.Include(w => w.Title, w => w.ServerRelativeUrl));
                context.ExecuteQuery();
                var websToDelete = new List<Web>();
                foreach (var web1 in webCollection1)
                {
                    if (web1.Title.StartsWith("Test_"))
                    {
                        var webCollection2 = web1.Webs;
                        context.Load(webCollection2, wc => wc.Include(w => w.Title, w => w.ServerRelativeUrl));
                        context.ExecuteQuery();
                        var childrenToDelete = new List<Web>(webCollection2);
                        foreach (var web2 in childrenToDelete)
                        {
                            Console.WriteLine("Deleting site {0}", web2.ServerRelativeUrl);
                            web2.DeleteObject();
                            context.ExecuteQuery();
                        }
                        websToDelete.Add(web1);
                    }
                }

                foreach (var web1 in websToDelete)
                {
                    Console.WriteLine("Deleting site {0}", web1.ServerRelativeUrl);
                    web1.DeleteObject();
                    try
                    {
                        context.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception cleaning up: {0}", ex);
                    }
                }
            }
        }


        [TestMethod()]
        public void DeployThemeToWebTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                context.Web.DeployThemeToWeb("Test Theme", customColorFilePath, null, customBackgroundFilePath, null);
                Assert.IsTrue(context.Web.ThemeEntryExists("Test Theme"));
            }
        }

        [TestMethod()]
        public void ThemeEntryExistsTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                Assert.IsTrue(context.Web.ThemeEntryExists("Office"));
                Assert.IsFalse(context.Web.ThemeEntryExists("Dummy Test Theme That Should Not Exist"));
            }
        }

        [TestMethod()]
        public void AddNewThemeOptionToSubWebTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                context.Web.DeployThemeToWeb("Test Theme", customColorFilePath, null, customBackgroundFilePath, null);
                Assert.IsTrue(context.Web.ThemeEntryExists("Test Theme"));
            }
        }

        [TestMethod()]
        public void GetCurrentThemeTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                context.Web.DeployThemeToWeb("Test Theme", customColorFilePath, null, customBackgroundFilePath, null);
                context.Web.SetThemeToWeb("Test Theme");

                var theme = context.Web.GetCurrentTheme();
                Assert.IsTrue(theme != null);
                Assert.IsTrue(theme.BackgroundImage.EndsWith("custombg.jpg"));
            }
        }

        [TestMethod()]
        public void CreateComposedLookShouldWork()
        {
            var testLookName = string.Format("Test_CL{0:yyyyMMddTHHmmss}", DateTimeOffset.Now);

            using (var context = TestCommon.CreateClientContext())
            {
                context.Load(context.Web, w => w.ServerRelativeUrl);
                context.ExecuteQuery();
                var paletteServerRelativeUrl = context.Web.ServerRelativeUrl + "/_catalog/theme/15" + builtInPalette003;
                var masterServerRelativeUrl = context.Web.ServerRelativeUrl + "/_catalog/masterpage" + builtInMasterOslo;
                
                context.Web.CreateComposedLookByUrl(testLookName, paletteServerRelativeUrl, null, null, masterServerRelativeUrl, 5);
            }

            using (var context = TestCommon.CreateClientContext())
            {
                var composedLooksList = context.Web.GetCatalog((int)ListTemplateType.DesignCatalog);
                CamlQuery query = new CamlQuery();
                query.ViewXml = string.Format(CAML_QUERY_FIND_BY_FILENAME, testLookName);
                var existingCollection = composedLooksList.GetItems(query);
                context.Load(existingCollection);
                context.ExecuteQuery();
                var item = existingCollection.FirstOrDefault();

                var lookPaletteUrl = item["ThemeUrl"] as FieldUrlValue;
                Assert.IsTrue(lookPaletteUrl.Url.Contains(builtInPalette003));
                var lookMasterUrl = item["MasterPageUrl"] as FieldUrlValue;
                Assert.IsTrue(lookMasterUrl.Url.Contains(builtInMasterOslo));
            }
        }

        [TestMethod()]
        public void SetComposedLookInherits()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                var webCollection = context.Web.Webs;
                context.Load(webCollection, wc => wc.Include(w => w.Title));
                context.ExecuteQuery();
                var webToChange1 = webCollection.First(w => w.Title == testWebName);

                var webCollection2 = webToChange1.Webs;
                context.Load(webCollection2, wc => wc.Include(w => w.Title));
                context.ExecuteQuery();
                var webToChangeA = webCollection2.First(w => w.Title == "A");

                webToChangeA.SetComposedLookByUrl(builtInLookBlossom);
                webToChange1.SetComposedLookByUrl(builtInLookSeaMonster);
            }

            using (var context = TestCommon.CreateClientContext())
            {
                var webCollection = context.Web.Webs;
                context.Load(webCollection, wc => wc.Include(w => w.Title));
                context.ExecuteQuery();
                var webToCheck1 = webCollection.First(w => w.Title == testWebName);

                var webCollection2 = webToCheck1.Webs;
                //context.Load(webCollection2, wc => wc.Include(w => w.Title, w => w.MasterUrl, w => w.CustomMasterUrl, w => w.ThemeInfo));
                context.Load(webCollection2, wc => wc.Include(w => w.Title, w => w.MasterUrl, w => w.CustomMasterUrl));
                context.ExecuteQuery();

                var webToCheckB = webCollection2.First(w => w.Title == "B");
                var webToCheckA = webCollection2.First(w => w.Title == "A");

                // Sea Monster oslo, palette005, image_bg005, fontscheme003
                Assert.IsTrue(webToCheckB.MasterUrl.Contains(builtInMasterOslo));
                // Blossom seattle, palette002, image_bg002
                Assert.IsTrue(webToCheckA.MasterUrl.Contains(builtInMasterSeattle));
                
                // TODO: A way to check the theme has been applied. web.ThemeInfo isn't working

                //var themeInfoB = webToCheckB.ThemeInfo;
                //Console.WriteLine("Load themeInfoB");
                //context.Load(themeInfoB);
                //context.ExecuteQuery(); // Fails with "The object id "ThemeInfo-..." is invalid"
                //Console.WriteLine("Use themeInfoB");
                //var accentTextB = themeInfoB.GetThemeShadeByName("AccentText");
                //context.ExecuteQuery();
                //var themeInfoA = webToCheckA.ThemeInfo;
                //context.Load(themeInfoA);
                //context.ExecuteQuery();
                //var accentTextA = themeInfoA.GetThemeShadeByName("AccentText");
                //context.ExecuteQuery();

                //Assert.AreEqual("F07200", accentTextB.Value);
                //Assert.AreEqual("D55881", accentTextA.Value);
            }
        }

        [TestMethod()]
        public void SetComposedLookResetInheritance()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                var webCollection = context.Web.Webs;
                context.Load(webCollection, wc => wc.Include(w => w.Title));
                context.ExecuteQuery();
                var webToChange1 = webCollection.First(w => w.Title == testWebName);

                var webCollection2 = webToChange1.Webs;
                context.Load(webCollection2, wc => wc.Include(w => w.Title));
                context.ExecuteQuery();
                var webToChangeA = webCollection2.First(w => w.Title == "A");

                webToChangeA.SetComposedLookByUrl(builtInLookBlossom);
                webToChange1.SetComposedLookByUrl(builtInLookSeaMonster, resetSubsitesToInherit:true);
            }

            using (var context = TestCommon.CreateClientContext())
            {
                var webCollection = context.Web.Webs;
                context.Load(webCollection, wc => wc.Include(w => w.Title));
                context.ExecuteQuery();
                var webToCheck1 = webCollection.First(w => w.Title == testWebName);

                var webCollection2 = webToCheck1.Webs;
                context.Load(webCollection2, wc => wc.Include(w => w.Title, w => w.MasterUrl, w => w.CustomMasterUrl, w => w.ThemeInfo));
                context.ExecuteQuery();
                var webToCheckA = webCollection2.First(w => w.Title == "A");

                // Sea Monster oslo, palette005, image_bg005, fontscheme003
                Assert.IsTrue(webToCheckA.MasterUrl.Contains(builtInMasterOslo));

                //var themeInfoA = webToCheckA.ThemeInfo;
                
                //Assert.AreEqual("F07200", themeInfoA.GetThemeShadeByName("AccentText"));
            }
        }

    }
}
