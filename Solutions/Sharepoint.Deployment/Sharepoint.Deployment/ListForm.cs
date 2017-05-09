using SharePoint.Deployment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SP = Microsoft.SharePoint.Client;

namespace SharePoint.Deployment {
    public class ListForm {
        public SP.PageType Type { get; set; }
        public string Url { get; set; }
        public string SetupPath { get; set; }
        public string WebPartZoneId { get; set; }
        public ListForm(SP.PageType type) {
            this.Type = type;
            this.Url = (type == SP.PageType.DisplayForm) ? "DispForm.aspx" : string.Concat(type.ToString(), ".aspx");
            this.SetupPath = @"pages\form.aspx";
            this.WebPartZoneId = "Main";
        }

        public XElement getSchemaXml() {
            return new XElement("Form",
                new XAttribute("Type", this.Type.ToString()),
                new XAttribute("Url", this.Url),
                new XAttribute("SetupPath", this.SetupPath),
                new XAttribute("WebPartZoneID", this.WebPartZoneId)
                );
        }
    }
}
