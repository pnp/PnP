using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Deployment.Utilities {
    public static class UrlUtility {
        public static string JoinUrl(params string[] args) {
            var result = from s in args
                         where !string.IsNullOrEmpty(s)
                         select s.Trim().Trim('/');
            return string.Join("/", result);
        }

        public static string GetRelativeUrl(string baseUrl, string fullUrl) {
            string returnValue;
            if (fullUrl.StartsWith(baseUrl, StringComparison.CurrentCultureIgnoreCase)) {
                returnValue = fullUrl.Replace(baseUrl, "");
            } else {
                throw new ArgumentException(string.Format("fullUrl '{0}' is not an extension of baseUrl '{1}'", fullUrl, baseUrl));
            }
            return returnValue;
        }
    }
}
