using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Deployment.utility {
    public static class EnumUtility {
        public static T Parse<T>(string value) {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}
