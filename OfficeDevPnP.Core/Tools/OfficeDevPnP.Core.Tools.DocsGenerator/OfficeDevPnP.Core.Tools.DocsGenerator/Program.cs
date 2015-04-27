using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace OfficeDevPnP.Core.Tools.DocsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateMDFromPnPSchema();
        }

        static void GenerateMDFromPnPSchema()
        {
            XDocument xsd = XDocument.Load(@"..\..\..\..\..\OfficeDevPnP.Core\Framework\Provisioning\Providers\Xml\ProvisioningSchema-2015-05.xsd");
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(@"..\..\XSD2MD.xslt");

            XsltArgumentList xsltArgs = new XsltArgumentList();
            xsltArgs.AddParam("now", String.Empty, DateTime.Now.ToShortDateString());

            using (FileStream fs = new FileStream(@"..\..\..\..\..\ProvisioningSchema-2015-05.md", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                xslt.Transform(xsd.CreateNavigator(), xsltArgs, fs);
            }
        }
    }
}
