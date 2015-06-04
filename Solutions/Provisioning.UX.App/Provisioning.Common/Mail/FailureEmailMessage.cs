using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Mail
{
    public class FailureEmailMessage
    {
        #region Variables/Properties
        private List<string> _to = new List<string>();
        private List<string> _cc = new List<string>();

        public string Subject { get; set; }
        public string SiteUrl { get; set; }
        public string SiteOwner { get; set; }
        public string SiteAdmin { get; set; }
        public string ErrorMessage { get; set; }
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
