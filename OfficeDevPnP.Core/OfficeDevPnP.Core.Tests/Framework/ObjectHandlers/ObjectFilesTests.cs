using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectFilesTests
    {
        private string resourceFolder;
        private const string fileName = "ProvisioningTemplate-2015-03-Sample-01.xml";
        private string folder;

        [TestInitialize]
        public void Initialize()
        {
            resourceFolder = string.Format(@"{0}\..\..\Resources\Templates",
                AppDomain.CurrentDomain.BaseDirectory);

            
            folder = string.Format("test{0}", DateTime.Now.Ticks);
        }

        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                if (!ctx.Web.IsPropertyAvailable("ServerRelativeUrl"))
                {
                    ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
                    ctx.ExecuteQueryRetry();
                }

                var file = ctx.Web.GetFileByServerRelativeUrl(UrlUtility.Combine(ctx.Web.ServerRelativeUrl, "test/" + fileName));
                ctx.Load(file, f => f.Exists);
                ctx.ExecuteQueryRetry();
                if (file.Exists)
                {
                    file.DeleteObject();
                    ctx.ExecuteQueryRetry();
                }
                if (ctx.Web.RootFolder.FolderExists(folder))
                {
                    var serverFolder = ctx.Web.GetFolderByServerRelativeUrl(UrlUtility.Combine(ctx.Web.ServerRelativeUrl, folder));
                    serverFolder.DeleteObject();
                    ctx.ExecuteQueryRetry();
                }
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();
            
            FileSystemConnector connector = new FileSystemConnector(resourceFolder,"");

            template.Connector = connector;

            template.Files.Add(new Core.Framework.Provisioning.Model.File() { Src = fileName, Folder = folder });

            using (var ctx = TestCommon.CreateClientContext())
            {
                new ObjectFiles().ProvisionObjects(ctx.Web, template);


                if (!ctx.Web.IsPropertyAvailable("ServerRelativeUrl"))
                {
                    ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
                    ctx.ExecuteQueryRetry();
                }

                var file = ctx.Web.GetFileByServerRelativeUrl(
                    UrlUtility.Combine(ctx.Web.ServerRelativeUrl,
                        UrlUtility.Combine(folder, fileName)));
                ctx.Load(file, f => f.Exists);
                ctx.ExecuteQueryRetry();
                Assert.IsTrue(file.Exists);
            }
        }

        [TestMethod]
        public void CanCreateEntities()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                // Load the base template which will be used for the comparison work
                var creationInfo = new ProvisioningTemplateCreationInformation(ctx.Web) { BaseTemplate = ctx.Web.GetBaseTemplate() };

                var template = new ProvisioningTemplate();
                template = new ObjectFiles().CreateEntities(ctx.Web, template, creationInfo);

                Assert.IsInstanceOfType(template.Files, typeof(List<Core.Framework.Provisioning.Model.File>));
            }
        }
    }
}
