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
        private string builtInLook1 = "Sea Monster"; // oslo, palette005, image_bg005, fontscheme003
        private string builtInLook2 = "Blossom"; // seattle, palette002,image_bg002
        private string builtInMaster1 = "oslo.master";
        private string builtInPalette1 = "palette003.spcolor";
        private string builtInFont1 = "fontscheme002.spfont";

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


        [TestInitialize()]
        public void Initialize()
        {
            Console.WriteLine("BrandingExtensionsTests.Initialise");
            customColorFilePath = Path.Combine(Path.GetTempPath(), "custom.spcolor");
            System.IO.File.WriteAllBytes(customColorFilePath, OfficeDevPnP.Core.Tests.Properties.Resources.custom);
            customBackgroundFilePath = Path.Combine(Path.GetTempPath(), "custombg.jpg");
            Properties.Resources.custombg.Save(customBackgroundFilePath);
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
            var testLookName = string.Format("Test_{0:s}", DateTimeOffset.Now);

            using (var context = TestCommon.CreateClientContext())
            {
                context.Load(context.Web, w => w.ServerRelativeUrl);
                context.ExecuteQuery();
                var paletteServerRelativeUrl = context.Web.ServerRelativeUrl + "/_catalog/theme/15" + builtInPalette1;
                var masterServerRelativeUrl = context.Web.ServerRelativeUrl + "/_catalog/masterpage" + builtInMaster1;
                
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
                Assert.IsTrue(lookPaletteUrl.Url.Contains(builtInPalette1));
                var lookMasterUrl = item["MasterPageUrl"] as FieldUrlValue;
                Assert.IsTrue(lookMasterUrl.Url.Contains(builtInMaster1));
            }
        }

    }
}
