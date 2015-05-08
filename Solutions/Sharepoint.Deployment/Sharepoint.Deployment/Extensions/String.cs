using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharePoint.Deployment {
    public static class String {
        private static Regex normalizationRegex = new Regex(@"[^\w]");
        public static string removeNonWordCharacters(this string value) {
            return normalizationRegex.Replace(value, "");
        }
    }
}
