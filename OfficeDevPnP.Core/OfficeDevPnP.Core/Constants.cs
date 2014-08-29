using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class Constants
    {
        public static readonly Guid APPSIDELOADINGFEATUREID = new Guid("AE3A1339-61F5-4f8f-81A7-ABD2DA956A7D");
        public static readonly Guid MINIMALDOWNLOADSTRATEGYFEATUREID = new Guid("87294c72-f260-42f3-a41b-981a2ffce37a");

        internal const string EXCEPTION_MSG_INVALID_ARG = "{The argument {0}, is invalid or not supplied.";
        internal const string FIELD_XML_FORMAT = @"<Field Type=""{0}"" Name=""{1}"" DisplayName=""{2}"" ID=""{3}"" Group=""{4}"" {5}/>";
        internal const string THEMES_DIRECTORY = "/_catalogs/theme/15/{0}";
        internal const string MASTERPAGE_SEATTLE = "/_catalogs/masterpage/seattle.master";
        internal const string MASTERPAGE_DIRECTORY = "/_catalogs/masterpage/{0}";
        internal const string MASTERPAGE_CONTENT_TYPE = "0x01010500B45822D4B60B7B40A2BFCC0995839404";
        internal const string PAGE_LAYOUT_CONTENT_TYPE = "0x01010007FF3E057FA8AB4AA42FCB67B453FFC100E214EEE741181F4E9F7ACC43278EE811";
    }
}
