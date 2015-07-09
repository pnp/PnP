using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.WorkflowTemplate
{
    public static class SharePointContextExtensions
    {

        //used in console apps
        /// <summary>
        /// Creates an ClientContext for the SharePoint Uri based on Authentication USer information
        /// </summary>
        /// <returns>A ClientContext instance</returns>
        public static ClientContext CreateSharePointContext(this SharePointContextProvider context,
            Uri webUrl, SharePointAuthenticationInfo authenticationInfo)
        {
            var clientContext = new ClientContext(webUrl);
            if (authenticationInfo.mode == SharePointMode.OnPremise)
            {
                NetworkCredential credentials = new NetworkCredential(authenticationInfo.userName, authenticationInfo.password);

                clientContext.Credentials = credentials;
            }
            else if (authenticationInfo.mode == SharePointMode.Cloud)
            {
                SecureString passWord = new SecureString();

                foreach (char c in authenticationInfo.password.ToCharArray()) passWord.AppendChar(c);

                clientContext.Credentials = new SharePointOnlineCredentials(authenticationInfo.userName, passWord);
            }
            else
            {
                throw new ArgumentException("SharePoint authentication information is invalid!");
                
            }
            return clientContext;
        }

    }
}
