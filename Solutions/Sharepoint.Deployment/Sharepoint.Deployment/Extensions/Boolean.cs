using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Deployment {
    public static class Boolean {
        public enum Case {
            Lower,
            Upper,
            Title
        }
        public static string ToString(this bool value, Case c) {
            switch (c) {
                case Case.Lower:
                    return CultureInfo.CurrentCulture.TextInfo.ToLower(value.ToString());
                case Case.Upper:
                    return CultureInfo.CurrentCulture.TextInfo.ToUpper(value.ToString());
                case Case.Title:
                    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToString());
                default:
                    throw new ArgumentException("Invalid case value provided.");
            }
        }
    }
}
