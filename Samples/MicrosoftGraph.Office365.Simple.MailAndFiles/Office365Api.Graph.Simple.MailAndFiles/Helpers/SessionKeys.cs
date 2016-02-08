using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office365Api.Graph.Simple.MailAndFiles.Helpers
{
    /// <summary>
    /// Keys used to store session values.
    /// </summary>
    public static class SessionKeys
    {
        public static class Login
        {

            public static string AccessToken = nameof(AccessToken);
            public static string UserInfo = nameof(UserInfo);
        }
    }
}