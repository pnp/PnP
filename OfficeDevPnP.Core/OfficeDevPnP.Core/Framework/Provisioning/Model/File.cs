using System;
using System.Collections.Generic;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class File : IEquatable<File>
    {
        #region Private Members
        private List<WebPart> _webParts = new List<WebPart>();
        #endregion

        #region Properties
        public string Src { get; set; }

        public string Folder { get; set; }

        public bool Overwrite { get; set; }

        public bool Create { get; set; }

        public List<WebPart> WebParts
        {
            get { return _webParts; }
            private set { _webParts = value; }
        }

        #endregion

        #region Constructors
        public File() { }

        public File(string src, string folder, bool overwrite, bool create, IEnumerable<WebPart> webParts)
        {
            this.Src = src;
            this.Overwrite = overwrite;
            this.Folder = folder;
            this.Create = create;
            if (webParts != null)
            {
                this.WebParts.AddRange(webParts);
            }
        }


        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}",
                this.Folder,
                this.Overwrite,
                this.Src).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is File))
            {
                return (false);
            }
            return (Equals((File)obj));
        }

        public bool Equals(File other)
        {
            return (this.Folder == other.Folder &&
                this.Overwrite == other.Overwrite &&
                this.Src == other.Src);
        }

        #endregion
    }
}
