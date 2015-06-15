using SP = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharePoint.Deployment.Utilities;
using System.IO;

namespace SharePoint.Deployment {
    public class Folder : Deployable {
        public SP.Folder SpFolder { get; set; }
        public Folder ParentFolder { get; set; }

        public Folder(string webRelativeUrl) {
            this.WebRelativeUrl = webRelativeUrl;
        }

        protected override void OnInit() {
            this.Folders.ForEach(i => i.ParentFolder = this);
            this.Files.ForEach(i => i.ParentFolder = this);
        }

        internal override void OnInvalidate() {
            this.SpFolder = null;
            this.IsDeployed = null;
        }

        internal override void OnCreate() {
            this.Context.ExecuteAsync(() => this.GetParentSPWeb().Folders.Add(this.WebRelativeUrl), () => this.IsDeployed = this.GetDeployed(true));
        }

        internal override void OnDelete() {
            this.Context.ExecuteAsync(() => this.SpFolder.DeleteObject(), () => { this.SpFolder = null; });
        }

        protected override bool GetDeployed() {
            var returnValue = false;
            this.SpFolder = this.GetParentSPWeb().Folders.GetByUrl(this.WebRelativeUrl);
            returnValue = this.Context.TryExecuteSync(this.SpFolder);
            if (!returnValue) {
                this.SpFolder = null;
            }
            return returnValue;
        }

        public override void ForEachChild(Action<Deployable> action) {
            if (this.Files != null) this.Files.ForEach(action);
            if (this.Folders != null) this.Folders.ForEach(action);
        }

        public string ServerRelativeUrl {
            get {
                return UrlUtility.JoinUrl(this.ParentWeb.ServerRelativeUrl, this.WebRelativeUrl);
            }
        }

        #region creation properties
        public string WebRelativeUrl { get; set; }
        #endregion

        #region children
        public List<Folder> Folders { get; set; }
        public List<File> Files { get; set; }
        #endregion

        public static Folder FromFileSystem(string localPath, string targetPath) {
            Folder returnValue;
            var folder = new DirectoryInfo(localPath);
            returnValue = new Folder(targetPath) {
                Folders = folder.GetDirectories().Select(i => Folder.FromFileSystem(i.FullName, UrlUtility.JoinUrl(targetPath, i.Name))).ToList(),
                Files = folder.GetFiles().Select(i => new File(i.FullName)).ToList()
            };

            return returnValue;
        }
    }
}
