using SP = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Specialized;

namespace SharePoint.Deployment {
    public class SiteField : Field {
        public SiteField(Guid id, SP.FieldType type, string name) : base(type, name) {
            this.Id = id;
        }

        protected override bool GetDeployed() {
            bool returnValue = false;
            this.SpField = this.ParentWeb.SpWeb.Fields.GetById(this.Id.Value);
            this.IsDeployed = returnValue = this.Context.TryExecuteSync(this.SpField);
            return returnValue;
        }

        internal override void OnCreate() {
            this.Context.ExecuteAsync(
                () => this.SpField = this.GetParentSPWeb().Fields.AddFieldAsXml(this.GetSchemaXml().ToString(), false, SP.AddFieldOptions.AddFieldInternalNameHint),
                () => this.IsDeployed = this.GetDeployed()
                );
        }

        internal override void OnDelete() {
            throw new NotImplementedException();
        }
    }
}
