using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SP = Microsoft.SharePoint.Client;

namespace SharePoint.Deployment {
    public abstract class Deployable {
        private bool? _isDeployed;
        public SharePointContext Context { get; protected set; }
        public Web ParentWeb { get { return (this.Parent == null) ? null : (this.Parent as Web) ?? this.Parent.ParentWeb; } }
        public Site ParentSite { get { return (this.Parent == null) ? null : (this.Parent as Site) ?? this.Parent.ParentSite; } }
        public Deployable Parent { get; set; }
        public virtual bool? IsDeployed {
            get {
                return this._isDeployed;
            }
            internal set {
                this._isDeployed = value;
                if (this._isDeployed.HasValue && !this.IsDeployed.Value) {
                    this.ForEachChild(i => i.IsDeployed = false);
                }
            }
        }
        public bool Initialized { get; protected set; }

        protected virtual void Init(SharePointContext context) {
            this.Init(context, false);
        }

        protected virtual void Init(SharePointContext context, bool force) {
            if (force || !this.Initialized) {
                this.Context = context;
                this.OnInit();
                this.IsDeployed = this.GetDeployed(force);
                this.Initialized = true;
            }

            this.ForEachChild(i => { i.Parent = this;
                                     i.Init(this.Context); });
        }

        protected virtual SP.Web GetParentSPWeb() {
            var returnValue = (this.ParentWeb != null) ? this.ParentWeb.SpWeb : null;
            if (returnValue == null) {
                var url = (this.ParentWeb != null) ? this.ParentWeb.SiteRelativeUrl : "/";
                returnValue = this.Context.OpenWeb(url);
            }

            return returnValue;
        }

        public bool GetDeployed(bool force) {
            if (force || !this.IsDeployed.HasValue) {
                this.IsDeployed = this.GetDeployed();
            }
            return this.IsDeployed.Value;
        }

        public void Delete() {
            if (this.IsDeployed.Value) {
                this.OnDelete();
                this.IsDeployed = false;
                this.Context.Flush();
                this.Invalidate();
                this.Context.Invalidate();
            }
        }

        public void Invalidate() {
            if (this.Parent != null) this.Parent.Invalidate();

            if (this.Initialized) {
                this.Initialized = false;
                this.OnInvalidate();
                this.ForEachChild(i => i.Invalidate());
            }
        }

        internal virtual void Create() {
            if (!this.IsDeployed.Value) {
                this.OnCreate();
            }
            this.ForEachChild(i => i.Create());
        }

        protected abstract void OnInit();
        internal abstract void OnInvalidate();
        internal abstract void OnCreate();
        internal abstract void OnDelete();
        protected abstract bool GetDeployed();
        public abstract void ForEachChild(Action<Deployable> action);
    }
}
