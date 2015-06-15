using SP = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Specialized;

namespace SharePoint.Deployment {
    public class ListField : Field {
        public List ParentList { get; set; }
        public SiteField SiteField { get; protected set; }
        public bool IsSiteField { get; protected set; }

        public ListField(SP.FieldType type, string name) : base(type, name) { }

        protected ListField(Guid id) {
            this.IsSiteField = true;
            this.Id = id;
        }

        protected override bool GetDeployed() {
            bool returnValue = false;
            if (this.Id.HasValue) {
                this.SpField = this.ParentWeb.SpWeb.Fields.GetById(this.Id.Value);
            } else {
                this.SpField = this.ParentWeb.SpWeb.Fields.GetByInternalNameOrTitle(this.InternalName);
            }
            this.IsDeployed = returnValue = this.Context.TryExecuteSync(this.SpField);

            if (!returnValue) {
                this.SpField = null;
            }

            return returnValue;
        }

        protected override void OnInit() {
            if (this.IsSiteField) {
                var web = this.ParentWeb;
                while (this.SiteField == null && web != null) {
                    if (web.Fields != null) this.SiteField = web.Fields.FirstOrDefault(i => i.Id == this.Id);
                    web = web.ParentWeb;
                }
                if (this.SiteField == null) throw new InvalidOperationException(string.Format("Can not find site field with id {0}", this.Id));
            } else {
                base.OnInit();
            }
        }

        internal override void OnCreate() {
            if (this.IsSiteField) {
                this.Context.ExecuteSync(() => { this.SpField = this.ParentList.SpList.Fields.Add(this.SiteField.SpField); } );
                this.IsDeployed = this.GetDeployed(true);
            } else {
                this.Context.ExecuteAsync(
                    () => this.SpField = this.ParentList.SpList.Fields.AddFieldAsXml(this.GetSchemaXml().ToString(), this.AddToDefaultView, this.FieldOptions),
                    () => this.IsDeployed = this.GetDeployed(true));
            }
        }

        internal override void OnDelete() {
            throw new NotImplementedException();
        }

        internal override XElement GetSchemaXml() {
            return (this.IsSiteField) ? this.SiteField.GetSchemaXml() : base.GetSchemaXml();
        }

        public bool AddToDefaultView { get; set; }
        public SP.AddFieldOptions FieldOptions { get; set; }

        public static ListField FromSiteField(SiteField field) {
            return ListField.FromSiteField(field.Id.Value);
        }

        public static ListField FromSiteField(Guid id) {
            return new ListField(id);
        }

        public static ListField TitleFieldDefinition {
            get {
                return new ListField(SP.FieldType.Text, "$Resources:core,Title;") {
                    Id = new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247"),
                    Required = true,
                    InternalName = "Title",
                    Options = { { "SourceID", "http://schemas.microsoft.com/sharepoint/v3" }, 
                                { "StaticName", "Title" }, 
                                { "MaxLength", "255" } }
                };
            }
        }
    }
}
