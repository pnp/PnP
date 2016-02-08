using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class MSGraphAPIDemoContext
    {
        public static String CurrentUserUPN
        {
            get
            {
                String result = null;

                var upn = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(System.Security.Claims.ClaimTypes.Upn);
                if (upn != null)
                {
                    result = upn.Value;
                }

                return (result);
            }
        }

        public static String CurrentUserDisplayName
        {
            get
            {
                String result = null;

                var name = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("name");
                if (name != null)
                {
                    result = name.Value;
                }

                return (result);
            }
        }
    }
}