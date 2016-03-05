using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data
{
    internal static class SPDataConstants
    {
        #region SharePoint Metadata Repository
        public const string LIST_URL_SITECLASSIFICATION = "Lists/SiteClassifications";
        public const string LIST_URL_DIVISIONS = "Lists/Divisions";
        public const string LIST_URL_FUNCTIONS = "Lists/Functions";
        public const string LIST_URL_LANGUAGES = "Lists/Languages";
        public const string LIST_URL_REGIONS = "Lists/Regions";
        public const string LIST_URL_TIMEZONES = "Lists/TimeZone";
        public const string LIST_URL_APPSETTINGS = "Lists/AppSettings";

        public const string LIST_TITLE_SITECLASSIFICATION = "Site Classifications";
        public const string LIST_TITLE_DIVISIONS = "Divisions";
        public const string LIST_TITLE_FUNCTIONS = "Functions";
        public const string LIST_TITLE_LANGUAGES = "Languages";
        public const string LIST_TITLE_REGIONS   = "Regions";
        public const string LIST_TITLE_TIMEZONES = "TimeZone";
        public const string LIST_TITLE_APPSETTINGS = "AppSettings";

        public const int CSOM_WAIT_TIME = -1;
        #endregion
    }
}
