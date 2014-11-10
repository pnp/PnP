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
        private string customColorFilePath = string.Empty;
        private string customBackgroundFilePath = string.Empty;
        private const string THEME_NAME = "Test Theme";

        private static string htmlPublishingPageWithoutExtension = "TestHtmlPublishingPageLayout";
        private static string publishingPageWithoutExtension = "TestPublishingPageLayout";
        private string htmlPublishingPagePath = string.Format("../../Resources/{0}.html", htmlPublishingPageWithoutExtension);
        private string publishingPagePath = string.Format("../../Resources/{0}.aspx", publishingPageWithoutExtension);
        private string pageLayoutTitle = "CustomHtmlPageLayout";

        private string welcomePageContentTypeId =
            "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB064584E219954237AF390064DEA0F50FC8C147B0B6EA0636C4A7D4";

        private Guid publishingSiteFeatureId = new Guid("f6924d36-2fa8-4f0b-b16d-06b7250180fa");
        private Guid publishingWebFeatureId = new Guid("94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb");

        bool deactivateSiteFeatureOnTeardown = false;
        bool deactivateWebFeatureOnTeardown = false;


        [TestInitialize()]
        public void Initialize()
        {
            customColorFilePath = Path.Combine(Path.GetTempPath(), "custom.spcolor");
            System.IO.File.WriteAllBytes(customColorFilePath, OfficeDevPnP.Core.Tests.Properties.Resources.custom);
            customBackgroundFilePath = Path.Combine(Path.GetTempPath(), "custombg.jpg");
            Properties.Resources.custombg.Save(customBackgroundFilePath);
        }

        [TestCleanup()]
        public void CleanUp()
        {
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
                                    <Eq>
                                        <FieldRef Name='Name' />
                                        <Value Type='Text'>{0}</Value>
                                    </Eq>
                                </Where>
                             </Query>
                        </View>";
                    // Let's update the theme name accordingly
                    camlString = string.Format(camlString, THEME_NAME);
                    query.ViewXml = camlString;
                    var found = themeGallery.GetItems(query);
                    web.Context.Load(found);
                    web.Context.ExecuteQuery();
                    if (found.Count > 0)
                    {
                        var themeItem = found[0];
                        themeItem.DeleteObject();
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
                context.Web.DeployThemeToWeb("Test Theme", customColorFilePath, null, customBackgroundFilePath, null);
                Assert.IsTrue(context.Web.ThemeEntryExists("Test Theme"));
            }
        }

        [TestMethod()]
        public void ThemeEntryExistsTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                context.Web.DeployThemeToWeb("Test Theme", customColorFilePath, null, customBackgroundFilePath, null);
                Assert.IsTrue(context.Web.ThemeEntryExists("Test Theme"));
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
    }
}
