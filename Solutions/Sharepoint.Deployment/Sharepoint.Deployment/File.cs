using SP = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Deployment.Utilities;
using System.IO;

namespace SharePoint.Deployment {
    public class File : Deployable {
        public SP.File SpFile { get; set; }
        public Folder ParentFolder { get; set; }

        public File(string fileName) {
            this.FileName = Path.GetFileName(FileName);
            this.FilePath = fileName;
        }

        protected override void OnInit() { }

        internal override void OnInvalidate() {
            this.SpFile = null;
            this.IsDeployed = null;
        }

        internal override void OnCreate() {
            using (var content = new FileInfo(this.FilePath).Open(FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var creationInfo = new SP.FileCreationInformation() { Url = this.ServerRelativeUrl, 
                                                                      ContentStream = content, 
                                                                      Overwrite = true };
                this.Context.ExecuteAsync(() => this.ParentFolder.SpFolder.Files.Add(creationInfo), () => this.IsDeployed = this.GetDeployed(true));
            }
        }

        internal override void OnDelete() {
            this.Context.ExecuteAsync(() => this.SpFile.DeleteObject(), () => { this.SpFile = null; });
        }

        protected override bool GetDeployed() {
            var returnValue = false;
            this.SpFile = this.GetParentSPWeb().GetFileByServerRelativeUrl(this.ServerRelativeUrl);
            this.IsDeployed = returnValue = this.Context.TryExecuteSync(this.SpFile);
            if (!returnValue) {
                this.SpFile = null;
            }
            return returnValue;
        }

        public override void ForEachChild(Action<Deployable> action) { }

        public string ServerRelativeUrl {
            get {
                return UrlUtility.JoinUrl(this.ParentFolder.ServerRelativeUrl, this.FileName);
            }
        }

        public string WebRelativeUrl {
            get {
                return UrlUtility.JoinUrl(this.ParentFolder.WebRelativeUrl, this.FileName);
            }
        }

        #region file properties
        public string FileName { get; set; }
        public string FilePath { get; set; }
        #endregion
    }
}
