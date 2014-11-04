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
    }
}
