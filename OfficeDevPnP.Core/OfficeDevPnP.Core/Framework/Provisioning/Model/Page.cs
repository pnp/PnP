using System;
using System.Collections.Generic;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class Page : IEquatable<Page>
    {
        #region Private Members
        private List<WebPart> _webParts = new List<WebPart>();
        #endregion

        #region Properties
        public string Url { get; set; }

        public WikiPageLayout Layout { get; set; }

        public bool Overwrite { get; set; }
        public bool WelcomePage { get; set; }

        public List<WebPart> WebParts
        {
            get { return _webParts; }
            private set { _webParts = value; }
        }

        #endregion

        #region Constructors
        public Page() { }

        public Page(string url, bool overwrite, WikiPageLayout layout, IEnumerable<WebPart> webParts, bool welcomePage)
        {
            this.Url = url;
            this.Overwrite = overwrite;
            this.Layout = layout;
            this.WelcomePage = welcomePage;

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
                this.Url,
                this.Overwrite,
                this.Layout).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is File))
            {
                return (false);
            }
            return (Equals((File)obj));
        }

        public bool Equals(Page other)
        {
            return (this.Url == other.Url &&
                this.Overwrite == other.Overwrite &&
                this.Layout == other.Layout);
        }

        #endregion
    }
}
