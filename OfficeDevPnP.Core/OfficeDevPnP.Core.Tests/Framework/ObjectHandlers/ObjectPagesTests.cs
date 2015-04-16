using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;
using WebPart = OfficeDevPnP.Core.Framework.Provisioning.Model.WebPart;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectPagesTests
    {
        private string webpartcontents = @"<webParts><webPart xmlns=""http://schemas.microsoft.com/WebPart/v3""><metaData><type name=""Microsoft.SharePoint.WebPartPages.ScriptEditorWebPart, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" /><importErrorMessage>Cannot import this Web Part.</importErrorMessage>
    </metaData>
    <data>
      <properties>
        <property name=""ExportMode"" type=""exportmode"">All</property>
        <property name=""HelpUrl"" type=""string"" />
        <property name=""Hidden"" type=""bool"">False</property>
        <property name=""Description"" type=""string"">Allows authors to insert HTML snippets or scripts.</property>
        <property name=""Content"" type=""string"">&lt;script type=""text/javascript""&gt;
alert(""Hello!"");
&lt;/script&gt;</property>
        <property name=""CatalogIconImageUrl"" type=""string"" />
        <property name=""Title"" type=""string"">Script Editor</property>
        <property name=""AllowHide"" type=""bool"">True</property>
        <property name=""AllowMinimize"" type=""bool"">True</property>
        <property name=""AllowZoneChange"" type=""bool"">True</property>
        <property name=""TitleUrl"" type=""string"" />
        <property name=""ChromeType"" type=""chrometype"">None</property>
        <property name=""AllowConnect"" type=""bool"">True</property>
        <property name=""Width"" type=""unit"" />
        <property name=""Height"" type=""unit"" />
        <property name=""HelpMode"" type=""helpmode"">Navigate</property>
        <property name=""AllowEdit"" type=""bool"">True</property>
        <property name=""TitleIconImageUrl"" type=""string"" />
        <property name=""Direction"" type=""direction"">NotSet</property>
        <property name=""AllowClose"" type=""bool"">True</property>
        <property name=""ChromeState"" type=""chromestate"">Normal</property>
      </properties>
    </data>
  </webPart>
</webParts>";

        [TestCleanup]
        public void Cleanup()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
                ctx.ExecuteQueryRetry();


                var file = ctx.Web.GetFileByServerRelativeUrl(UrlUtility.Combine(ctx.Web.ServerRelativeUrl, "/SitePages/pagetest.aspx"));
                ctx.Load(file, f => f.Exists);
                ctx.ExecuteQueryRetry();

                if (file.Exists)
                {
                    file.DeleteObject();
                    ctx.ExecuteQueryRetry();
                }
            }
        }
        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();
            
            Page page = new Page();
            page.Layout = WikiPageLayout.TwoColumns;
            page.Overwrite = true;
            page.Url = "~site/sitepages/pagetest.aspx";

           
            var webPart = new WebPart();
            webPart.Column = 1;
            webPart.Row = 1;
            webPart.Contents = webpartcontents;
            webPart.Title = "Script Test";

            page.WebParts.Add(webPart);


            template.Pages.Add(page);

            using (var ctx = TestCommon.CreateClientContext())
            {
                TokenParser parser = new TokenParser(ctx.Web, template);
                new ObjectPages().ProvisionObjects(ctx.Web, template, parser);

                ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
                ctx.ExecuteQueryRetry();

                var file = ctx.Web.GetFileByServerRelativeUrl(UrlUtility.Combine(ctx.Web.ServerRelativeUrl, "/SitePages/pagetest.aspx"));
                ctx.Load(file, f => f.Exists);
                ctx.ExecuteQueryRetry();

                Assert.IsTrue(file.Exists);
                var wps = ctx.Web.GetWebParts(UrlUtility.Combine(ctx.Web.ServerRelativeUrl, "/SitePages/pagetest.aspx"));
                Assert.IsTrue(wps.Any());
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
                template = new ObjectPages().CreateEntities(ctx.Web, template, creationInfo);

                Assert.IsInstanceOfType(template.Pages, typeof(List<Core.Framework.Provisioning.Model.Page>));
            }
        }
    }
}
