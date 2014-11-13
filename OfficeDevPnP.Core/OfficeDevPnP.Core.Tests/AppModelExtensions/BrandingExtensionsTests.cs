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
        private static string htmlPublishingPageWithoutExtension = "TestHtmlPublishingPageLayout";
        private static string publishingPageWithoutExtension = "TestPublishingPageLayout";
        private string htmlPublishingPagePath = string.Format("../../Resources/{0}.html", htmlPublishingPageWithoutExtension);
        private string publishingPagePath = string.Format("../../Resources/{0}.aspx", publishingPageWithoutExtension);
        private string pageLayoutTitle = "CustomHtmlPageLayout";

        private string welcomePageContentTypeId =
            "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB064584E219954237AF390064DEA0F50FC8C147B0B6EA0636C4A7D4";

        private Guid publishingSiteFeatureId = new Guid("f6924d36-2fa8-4f0b-b16d-06b7250180fa");
        private Guid publishingWebFeatureId = new Guid("94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb");

        private string testWebName;

        bool deactivateSiteFeatureOnTeardown = false;
        bool deactivateWebFeatureOnTeardown = false;


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

                // Remove composed looks from server
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
                Console.WriteLine("{0} matching looks found to delete", found.Count);
                var looksToDelete = found.ToList();
                foreach (var item in looksToDelete)
                {
                    Console.WriteLine("Delete look item '{0}'", item["Name"]);
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

                try
                {
                    Microsoft.SharePoint.Client.File customColorFile = folder15.Files.GetByUrl("custom.spcolor");
                    customColorFile.DeleteObject();
                    context.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception cleaning up: {0}", ex);
                }

                try
                {
                    Microsoft.SharePoint.Client.File customBackgroundFile = folder15.Files.GetByUrl("custombg.jpg");
                    customBackgroundFile.DeleteObject();
                    context.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception cleaning up: {0}", ex);
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

        private Web Setup()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                ExceptionHandlingScope scope = new ExceptionHandlingScope(ctx);

                Web web;
                Site site;
                site = ctx.Site;
                web = ctx.Site.RootWeb;
                if (!site.IsFeatureActive(publishingSiteFeatureId))
                {
                    site.ActivateFeature(publishingSiteFeatureId);
                    deactivateSiteFeatureOnTeardown = true;
                }
                if (!web.IsFeatureActive(publishingWebFeatureId))
                {
                    site.RootWeb.ActivateFeature(publishingWebFeatureId);
                    deactivateWebFeatureOnTeardown = true;
                }
                return web;

            }
        }

        private void Teardown()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                if (deactivateSiteFeatureOnTeardown)
                {
                    ctx.Site.DeactivateFeature(publishingSiteFeatureId);
                }
                if (deactivateWebFeatureOnTeardown)
                {
                    ctx.Web.DeactivateFeature(publishingWebFeatureId);
                }
            }
        }

        [TestMethod]
        public void CanUploadHtmlPageLayoutAndConvertItToAspxVersion()
        {
            var web = Setup();
            web.Context.Load(web);
            web.DeployHtmlPageLayout(htmlPublishingPagePath, pageLayoutTitle, "", welcomePageContentTypeId);
            web.Context.Load(web, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();
            var item = web.GetPageLayoutListItemByName(htmlPublishingPageWithoutExtension);
            Assert.AreNotEqual(null, item);
            Teardown();
        }

        [TestMethod]
        public void CanUploadPageLayout()
        {
            var web = Setup();
            web.Context.Load(web);
            web.DeployPageLayout(publishingPagePath, pageLayoutTitle, "", welcomePageContentTypeId);
            web.Context.Load(web, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();
            var item = web.GetPageLayoutListItemByName(publishingPageWithoutExtension);
            Assert.AreNotEqual(null, item);
            Teardown();
        }

        [TestMethod()]
        public void DeployThemeToWebTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                context.Web.DeployThemeToWeb("Test_Theme", customColorFilePath, null, customBackgroundFilePath, null);
                Assert.IsTrue(context.Web.ThemeEntryExists("Test_Theme"));
            }
        }

        [TestMethod()]
        public void ThemeEntryExistsTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                // context.Web.DeployThemeToWeb("Test_Theme", customColorFilePath, null, customBackgroundFilePath, null);
                //Assert.IsTrue(context.Web.ThemeEntryExists("Test_Theme"));
                Assert.IsTrue(context.Web.ThemeEntryExists("Office"));
                Assert.IsFalse(context.Web.ThemeEntryExists("Dummy Test Theme That Should Not Exist"));
            }
        }

        [TestMethod()]
        public void AddNewThemeOptionToSubWebTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                context.Web.DeployThemeToWeb("Test_Theme", customColorFilePath, null, customBackgroundFilePath, null);
                Assert.IsTrue(context.Web.ThemeEntryExists("Test_Theme"));
            }
        }

        [TestMethod()]
        public void GetCurrentLookTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                context.Web.SetComposedLookByUrl(builtInLookSeaMonster);
            }

            using (var context = TestCommon.CreateClientContext())
            {
                var theme = context.Web.GetCurrentLook();
                Assert.IsTrue(theme != null);
                Assert.IsTrue(theme.BackgroundImage.EndsWith("image_bg005.jpg"));
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
                var lookDisplayOrder = item["DisplayOrder"].ToString();
                Assert.AreEqual("5", lookDisplayOrder);
            }
        }

        [TestMethod()]
        public void CreateComposedLookByNameShouldWork()
        {
            var testLookName = string.Format("Test_CL{0:yyyyMMddTHHmmss}", DateTimeOffset.Now);

            using (var context = TestCommon.CreateClientContext())
            {
                // Act
                context.Web.CreateComposedLookByName(testLookName, builtInPalette003, null, null, builtInMasterOslo, 5);
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

                // Act
                webToChangeA.SetComposedLookByUrl(builtInLookBlossom);
                webToChange1.SetComposedLookByUrl(builtInLookSeaMonster, resetSubsitesToInherit: false);
            }

            using (var context = TestCommon.CreateClientContext())
            {
                var webCollection = context.Web.Webs;
                context.Load(webCollection, wc => wc.Include(w => w.Title));
                context.ExecuteQuery();
                var webToCheck1 = webCollection.First(w => w.Title == testWebName);

                var webCollection2 = webToCheck1.Webs;
                context.Load(webCollection2, wc => wc.Include(w => w.Title, w => w.MasterUrl, w => w.CustomMasterUrl));
                context.ExecuteQuery();

                var webToCheckB = webCollection2.First(w => w.Title == "B");
                var webToCheckA = webCollection2.First(w => w.Title == "A");
                var accentTextB = webToCheckB.ThemeInfo.GetThemeShadeByName("AccentText");
                var accentTextA = webToCheckA.ThemeInfo.GetThemeShadeByName("AccentText");
                context.ExecuteQuery();

                // Assert: B will have new style, A will have Inherit = false and not get the new style

                // Sea Monster oslo, palette005, image_bg005, fontscheme003
                Assert.IsTrue(webToCheckB.MasterUrl.Contains(builtInMasterOslo));
                Assert.AreEqual("FFF07200", accentTextB.Value);

                // Blossom seattle, palette002, image_bg002
                Assert.IsTrue(webToCheckA.MasterUrl.Contains(builtInMasterSeattle));
                Assert.AreEqual("FFD55881", accentTextA.Value);
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

                // Act
                webToChangeA.SetComposedLookByUrl(builtInLookBlossom);
                webToChange1.SetComposedLookByUrl(builtInLookSeaMonster, resetSubsitesToInherit: true);
            }

            using (var context = TestCommon.CreateClientContext())
            {
                var webCollection = context.Web.Webs;
                context.Load(webCollection, wc => wc.Include(w => w.Title));
                context.ExecuteQuery();
                var webToCheck1 = webCollection.First(w => w.Title == testWebName);

                var webCollection2 = webToCheck1.Webs;
                context.Load(webCollection2, wc => wc.Include(w => w.Title, w => w.MasterUrl, w => w.CustomMasterUrl));
                context.ExecuteQuery();
                var webToCheckA = webCollection2.First(w => w.Title == "A");
                var accentA = webToCheckA.ThemeInfo.GetThemeShadeByName("AccentText");
                context.ExecuteQuery();

                // Assert: B will have Inherit = false and not get the new style, A will hav new style

                // Sea Monster oslo, palette005, image_bg005, fontscheme003
                Assert.IsTrue(webToCheckA.MasterUrl.Contains(builtInMasterOslo));
                Assert.AreEqual("FFF07200", accentA.Value);
            }
        }

    }
}
