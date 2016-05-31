namespace Core.JavaScript.Embedder
{
    using Microsoft.SharePoint.Client;
    using System;
    using System.Net;
    using System.Security;

    public static class ContextManager
    {
        #region WithContext

        internal static void WithContext(Action<ClientContext> a)
        {
            // you could make this as complicated as you want (load from config, etc)
            // for testing simple is easier
            var mySite = "{server url}";
            var myLogin = "{login}";
            var myPassword = "{password}";
            var isOnline = true;
            var hasPartnerAccess = false;
            var useDefault = true;


            var context = new ClientContext(mySite);

            if (useDefault)
            {
                // we will use the current user's credentials
                context.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
            else if (isOnline)
            {
                var password = new SecureString();
                foreach (char c in myPassword.ToCharArray())
                {
                    password.AppendChar(c);
                }

                context.Credentials = new SharePointOnlineCredentials(myLogin, password);
            }
            else
            {
                context.Credentials = new NetworkCredential(myLogin, myPassword);
            }

            if (hasPartnerAccess)
            {
                // needed if partner access is enabled in SPO-D
                context.ExecutingWebRequest += (sender, e) =>
                {
                    e.WebRequestExecutor.WebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                    e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                };
            }

            a(context);
        }

        #endregion
    }
}
