using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SP = Microsoft.SharePoint.Client;

namespace SharePoint.Deployment {
    public class ListView : Deployable {
        internal SP.View SpListView;

        public ListView(string name, params string[] viewFields) : this(ListViewType.HTML, name, viewFields) { }
        public ListView(ListViewType type, string name, params string[] viewFields) {
            if (name == null) throw new ArgumentException("Name cannot be null", "name");

            this.Type = type;
            this.Name = name;
            this.NameResource = NameResource;
            this.Query = Query;
            this.ViewFields = new List<string>(viewFields);
            this.Url = string.Concat(name.removeNonWordCharacters(), ".aspx");
            this.Query = "<Where />";
            this.ToolbarType = ListToolbarType.Standard;
            this.XslLink = "main.xsl";
            this.XslLinkDefault = true;
            this.JsLink = "clienttemplates.js";
            this.RowLimit = 30;
            this.Paged = true;
            this.SetupPath = @"pages\viewpage.aspx";
            this.WebPartZoneId = "Main";
            this.BaseViewId = 1;
        }

        public static ListView GetDefaultView() {
            return new ListView("All Items", "LinkTitle") { DefaultView = true, Url = "AllItems.aspx", NameResource = "$Resources:core,objectiv_schema_mwsidcamlidC24;" };
        }

        public static ListView GetViewAllFields(string name, List list) {
            return new ListView(name, list.Fields.Select(i => i.InternalName).ToArray());
        }

        public static XElement GetBasicSchemaXml(string query, int rowLimit, bool paged, string scope, params string[] fields) {
            XElement returnValue;
            var viewFields = new XElement("ViewFields");
            returnValue = new XElement("View", viewFields);
            fields.ForEach(i => viewFields.Add(new XElement("FieldRef", new XAttribute("Name", i), "")));

            if (rowLimit > 0)                        returnValue.Add(new XElement("RowLimit", new XAttribute("Paged", paged.ToString(Boolean.Case.Upper)), rowLimit));
            if (!string.IsNullOrEmpty(query))        returnValue.Add(new XElement("Query", XElement.Parse(query)));
            /* Add scope to view definition */

            return returnValue;
        }

        public XElement GetSchemaXml() {
            XElement returnValue = ListView.GetBasicSchemaXml(this.Query, this.RowLimit, this.Paged, this.Scope, this.ViewFields.ToArray());
            returnValue.Add(
                new XAttribute("BaseViewID", this.BaseViewId),
                new XAttribute("Type", this.Type.ToString()),
                new XAttribute("DisplayName", this.NameResource ?? this.Name),
                new XAttribute("SetupPath", this.SetupPath),
                new XAttribute("Url", this.Url),
                new XAttribute("WebPartZoneID", this.WebPartZoneId)
                );

            if (this.DefaultView)                         returnValue.Add(new XAttribute("DefaultView", this.DefaultView.ToString(Boolean.Case.Upper)));
            if (this.ToolbarType != ListToolbarType.None) returnValue.Add(new XElement("Toolbar", new XAttribute("Type", this.ToolbarType.ToString())));
            if (!string.IsNullOrEmpty(this.XslLink))      returnValue.Add(new XElement("XslLink", new XAttribute("Default", this.XslLinkDefault.ToString(Boolean.Case.Upper)), this.XslLink));
            if (!string.IsNullOrEmpty(this.JsLink))       returnValue.Add(new XElement("JSLink", this.JsLink));

            return returnValue;
        }

        protected override void OnInit() {
        }

        internal override void OnInvalidate() {
            this.SpListView = null;
        }

        protected override bool GetDeployed() {
            var returnValue = false;
            var list = (List)this.Parent;
            var view = list.SpList.Views.GetByTitle(this.Name);
            if (returnValue = this.Context.TryExecuteSync(view)) {
                this.SpListView = view;
            }
            return returnValue;
        }

        public override void ForEachChild(Action<Deployable> action) {
        }

        internal override void OnCreate() {
        }

        internal override void OnDelete() {
        }

        #region View properties
        public string XslLink { get; set; }
        public bool XslLinkDefault { get; set; }
        public string JsLink { get; set; }
        public ListToolbarType ToolbarType { get; set; }
        public ListViewType Type { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public int RowLimit { get; set; }
        public bool Paged { get; set; }
        public string Scope { get; set; }
        public List<string> ViewFields { get; set; }
        public bool DefaultView { get; set; }
        public string SetupPath { get; set; }
        public string Url { get; set; }
        public string WebPartZoneId { get; set; }
        public int BaseViewId { get; set; }
        public string NameResource { get; set; }
        #endregion
    }
}
