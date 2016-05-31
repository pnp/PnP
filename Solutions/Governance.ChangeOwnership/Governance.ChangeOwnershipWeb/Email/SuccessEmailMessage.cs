using System.Collections.Generic;

namespace Contoso.Office365.common
{
    public class SuccessEmailMessage
    {
        #region Variables/Properties
        private List<string> _to = new List<string>();
        private List<string> _cc = new List<string>();

        public string Subject { get; set; }
        public string SiteUrl { get; set; }
        public string OldSiteOwner { get; set; }
        public string NewSiteOwner { get; set; }
        public int StorageLimit { get; set; }
        public List<string> To
        {
            get { return _to; }
            set { _to = value; }
        }
        public List<string> Cc
        {
            get { return _cc; }
            set { _cc = value; }
        }

        #endregion
    }
}
