using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectComposedLookTests
    {

        [TestMethod]
        public void CanCreateComposedLooks()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var template = new ProvisioningTemplate();
                template = new ObjectComposedLook().CreateEntities(ctx.Web, template, null);
                Assert.IsInstanceOfType(template.ComposedLook, typeof(Core.Framework.Provisioning.Model.ComposedLook));
            }
        }

        [TestMethod]
        public void CanProvisionComposed1Looks()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                using (ClientContext cc = ctx.Clone("https://bertonline.sharepoint.com/sites/temp2"))
                {
                    var template = new ProvisioningTemplate();
                    template = new ObjectComposedLook().CreateEntities(cc.Web, template, null);

                    template.ComposedLook.Name = "Green";
                    template.ID = "bertdemo";

                    cc.Web.ApplyProvisioningTemplate(template);
                }
            }
        }

        [TestMethod]
        public void CanProvisionComposed2Looks()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                using (ClientContext cc = ctx.Clone("https://bertonline.sharepoint.com/sites/4f020b2a38344bc79fd431759272b48d"))
                {
                    var template = new ProvisioningTemplate();
                    template = new ObjectComposedLook().CreateEntities(cc.Web, template, null);

                    template.ID = "bertdemo";
                    template.Connector = new FileSystemConnector("./Resources", "");

                    using (ClientContext cc2 = ctx.Clone("https://bertonline.sharepoint.com/sites/temp2/s1"))
                    {
                        cc2.Web.ApplyProvisioningTemplate(template);
                    }
                }
            }
        }

        [TestMethod]
        public void CanGetComposedLook()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                using (ClientContext cc = ctx.Clone("https://bertonline.sharepoint.com/sites/temp2/s1"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }

                using (var cc = ctx.Clone("https://bertonline.sharepoint.com/sites/4f020b2a38344bc79fd431759272b48d"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }
                using (var cc = ctx.Clone("https://bertonline.sharepoint.com/sites/130020"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }

                using (var cc = ctx.Clone("https://bertonline.sharepoint.com/sites/130020"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }

                using (var cc = ctx.Clone("https://bertonline.sharepoint.com/sites/20140020"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }

                using (var cc = ctx.Clone("https://bertonline.sharepoint.com/sites/dev4"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }

                using (var cc = ctx.Clone("https://bertonline.sharepoint.com/sites/temp3"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }

                using (var cc = ctx.Clone("https://bertonline.sharepoint.com/sites/temp3/demo1"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }

                using (var cc = ctx.Clone("https://bertonline.sharepoint.com/sites/temp2"))
                {
                    ThemeEntity t = cc.Web.GetCurrentComposedLook();
                }

            }
        }


    }
}
