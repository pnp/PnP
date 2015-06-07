using System;
using System.Collections.Generic;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class File : IEquatable<File>
    {
        #region Private Members
        private List<WebPart> _webParts = new List<WebPart>();
        private Dictionary<string, string> _properties = new Dictionary<string,string>();
        #endregion

        #region Properties
        public string Src { get; set; }

        public string Folder { get; set; }

        public bool Overwrite { get; set; }

        public List<WebPart> WebParts
        {
            get { return _webParts; }
            private set { _webParts = value; }
        }

        public Dictionary<string, string> Properties
        {
            get { return _properties; }
            private set { _properties = value; }
        }

        #endregion

        #region Constructors
        public File() { }

        public File(string src, string folder, bool overwrite, IEnumerable<WebPart> webParts, IDictionary<string,string> properties )
        {
            this.Src = src;
            this.Overwrite = overwrite;
            this.Folder = folder;
            if (webParts != null)
            {
                this.WebParts.AddRange(webParts);
            }
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    this.Properties.Add(property.Key,property.Value);
                }
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
