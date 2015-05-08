using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x = System.Xml.Linq;

namespace SharePoint.Deployment {
    public static class XElementExtensions {
        public static void setNamespaceOnDecendants(this x.XElement element, x.XNamespace name) {
            element.Name = name + element.Name.LocalName;
            element.Elements().ForEach(i => i.setNamespaceOnDecendants(name));
        }
    }
}
