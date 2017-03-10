using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Web
{
    public static class OAuth2ProtectedResourceUtility
    {
        public static string ReadToken(string authorizationHeader)
        {
            string text = authorizationHeader.Trim();
            if (text.StartsWith("Bearer", true, System.Globalization.CultureInfo.InvariantCulture))
            {
                string[] array = authorizationHeader.Split(new char[]
                {
                    ' '
                });
                if (array.Length == 2 && array[0].Equals("Bearer", System.StringComparison.OrdinalIgnoreCase))
                {
                    return array[1];
                }
            }
            return null;
        }

        public static string WriteAuthorizationHeader(string token)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1}", new object[]
            {
                "Bearer",
                token
            });
        }
    }
}
