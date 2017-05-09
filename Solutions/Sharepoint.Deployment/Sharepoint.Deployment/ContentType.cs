using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Deployment {
    public class ContentType : Deployable {
        #region Children
        public List<ListField> Fields { get; set; }
        #endregion

        internal string GetSchemaXml() {
            throw new NotImplementedException();
        }

        protected override void OnInit() {
            throw new NotImplementedException();
        }

        internal override void OnInvalidate() {
            throw new NotImplementedException();
        }

        protected override bool GetDeployed() {
            throw new NotImplementedException();
        }

        public override void ForEachChild(Action<Deployable> action) {
            throw new NotImplementedException();
        }

        internal override void OnCreate() {
            throw new NotImplementedException();
        }

        internal override void OnDelete() {
            throw new NotImplementedException();
        }
    }
}
