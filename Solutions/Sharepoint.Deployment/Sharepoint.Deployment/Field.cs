using SP = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace SharePoint.Deployment {
    public abstract class Field : Deployable {
        internal SP.Field SpField { get; set; }

        protected Field() { }

        public Field(SP.FieldType type, string name) {
            this.FieldType = type;
            this.InternalName = name;
            this.Title = name;

            this.Choices = new List<string>();
            this.Options = new Dictionary<string,string>();
        }

        protected override void OnInit() {
            if (string.IsNullOrEmpty(this.InternalName)) {
                this.InternalName = this.Title.removeNonWordCharacters();
            }
        }

        internal override void OnInvalidate() {
            this.SpField = null;
        }

        public override void ForEachChild(Action<Deployable> action) { }

        internal virtual XElement GetSchemaXml() {
            XElement returnValue = new XElement("Field");
            
            if (this.Id.HasValue) returnValue.Add(new XAttribute("ID", this.Id.Value.ToString("B")));

            returnValue.Add(
                new XAttribute("Type", this.FieldType.ToString()),
                new XAttribute("Name", this.InternalName),
                new XAttribute("DisplayName", this.Title),
                new XAttribute("Required", this.Required.ToString(Boolean.Case.Upper))
                );

            if (!string.IsNullOrEmpty(this.DefaultValue)) {
                returnValue.Add(new XAttribute("Default", this.DefaultValue));
            }

            foreach (string key in this.Options.Keys) {
                returnValue.Add(new XAttribute(key, this.Options[key]));
            }

            if (this.FieldType == SP.FieldType.Choice) {
                XElement choices = new XElement("Choices");
                this.Choices.ForEach(i => choices.Add(new XAttribute("Choice", i)));
                returnValue.Add(choices);
            }

            return returnValue;
        }

        public virtual SP.FieldType FieldType { get; set; }
        public virtual Guid? Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string InternalName { get; set; }
        public virtual string DefaultValue { get; set; }
        public virtual bool Required { get; set; }
        public virtual List<string> Choices { get; set; }
        public virtual Dictionary<string,string> Options { get; set; }
    }
}
