using Microsoft.Office365.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Overview
{
    static class DiscoveryAPISample
    {
        // Discovery service supports MyFiles, Mail, Contacts and Calendar
        public static async Task<AuthenticationInfo> DiscoverMyFiles()
        {
            Authenticator authenticator = new Authenticator();
            AuthenticationInfo authInfo = await authenticator.AuthenticateAsync("MyFiles", ServiceIdentifierKind.Capability);
            return authInfo;
        }

        public static async Task<AuthenticationInfo> DiscoverMail()
        {
            Authenticator authenticator = new Authenticator();
            AuthenticationInfo authInfo = await authenticator.AuthenticateAsync("Mail", ServiceIdentifierKind.Capability);
            return authInfo;
        }
    }
}
