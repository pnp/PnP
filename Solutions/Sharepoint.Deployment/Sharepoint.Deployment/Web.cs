using SharePoint.Deployment.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SP = Microsoft.SharePoint.Client;
using Xml = System.Xml;

namespace SharePoint.Deployment {
    public class Web : Deployable, IDisposable {
        internal SP.Web SpWeb { get; set; }

        public string SiteRelativeUrl {
            get {
                return UrlUtility.GetRelativeUrl(this.ParentSite.Url, this.FullUrl);
            }
        }

        public string ServerRelativeUrl {
            get {
                return new Uri(this.FullUrl).PathAndQuery;
            }
        }

        public string FullUrl {
            get {
                string returnValue;
                string root;
                if (this.ParentWeb == null) {
                    root = this.ParentSite.Url;
                } else {
                    root = this.ParentWeb.FullUrl;
                }
                returnValue = UrlUtility.JoinUrl(root, this.UrlSegment);
                return returnValue;
            }
        }

        public Web() {
            this.Features = new List<Guid>();
            this.Fields = new List<SiteField>();
            this.Webs = new List<Web>();
            this.Lists = new List<List>();
        }

        public void Deploy() {
            this.Create();
            this.Context.Flush();
        }

        public void Init(string userName, string password, bool force) {
            base.Init(new SharePointContext(this.FullUrl, userName, password), force);
        }

        public void Init(string userName, string password) {
            this.Init(userName, password, false);
        }

        public void Init(ICredentials credentials, bool force) {
            base.Init(new SharePointContext(this.FullUrl, credentials), force);
        }

        public void Init(ICredentials credentials) {
            this.Init(credentials, false);
        }

        protected override bool GetDeployed() {
            var returnValue = false;
            this.SpWeb = this.Context.OpenWeb(this.SiteRelativeUrl);
            this.IsDeployed = returnValue = this.Context.TryExecuteSync(this.SpWeb);
            if (!returnValue) {
                this.SpWeb = null;
            }
            return returnValue;
        }

        protected override void OnInit() { }

        internal override void OnInvalidate() {
            this.SpWeb = null;
        }

        internal override void OnCreate() {
            var creationInfo = new SP.WebCreationInformation {
                Title = this.Title,
                Url = this.UrlSegment,
                Language = this.Language,
                Description = this.Description,
                UseSamePermissionsAsParentSite = true,
                WebTemplate = this.WebTemplate
            };
            SP.Web parentSpWeb = this.GetParentSPWeb();
            this.Context.ExecuteAsync(() => this.SpWeb = parentSpWeb.Webs.Add(creationInfo), () => this.IsDeployed = this.GetDeployed(true));
        }

        internal override void OnDelete() {
            this.Webs.ForEach(i => i.OnDelete());
            if (this.SpWeb == null) this.GetDeployed();
            this.Context.ExecuteAsync(() => { this.SpWeb.DeleteObject(); }, () => { this.SpWeb = null; });
        }

        public void LoadFromXml(Xml.XmlReader reader) {
            throw new NotImplementedException();
        }

        public override void ForEachChild(Action<Deployable> action) {
            if (this.Fields != null) this.Fields.ForEach(action);
            if (this.Lists != null) this.Lists.ForEach(action);
            if (this.Webs != null) this.Webs.ForEach(action);
            if (this.Folders != null) this.Folders.ForEach(action);
        }

        #region Definition Info
        public string UrlSegment { get; set; }
        public string Title { get; set; }
        public string WebTemplate { get; set; }
        public string Description { get; set; }
        public int Language { get; set; }
        #endregion

        #region Children
        public List<Web> Webs { get; set; }
        public List<List> Lists { get; set; }
        public List<Guid> Features { get; set; }
        public List<SiteField> Fields { get; set; }
        public List<Folder> Folders { get; set; }
        #endregion

        public void Dispose() {
            if (this.Context != null) this.Context.Dispose();
            if (this.Webs != null) this.Webs.ForEach(i => i.Dispose());
        }

    }
}
