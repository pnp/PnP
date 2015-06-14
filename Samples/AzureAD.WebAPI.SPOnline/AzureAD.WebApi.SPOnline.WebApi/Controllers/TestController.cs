using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AzureAD.WebApi.SPOnline.WebApi.Controllers
{
    [Authorize]
    public class TestController : ApiController
    {
        [HttpGet]
        public string Test()
        {
            string sharePointUrl = ConfigurationManager.AppSettings["SharePointURL"];
            string newToken = GetSharePointAccessToken(sharePointUrl, this.Request.Headers.Authorization.Parameter);

            using (ClientContext cli = new ClientContext(sharePointUrl))
            {

                /// Adding authorization header 
                cli.ExecutingWebRequest += (s, e) => e.WebRequestExecutor.WebRequest.Headers.Add("Authorization", "Bearer " + newToken);
            
                var web = cli.Web;
                cli.Load(web);
                cli.ExecuteQuery();
                return web.Title;
            }
        }

        internal static string GetSharePointAccessToken(string url, string accessToken)
        {
            string clientID = ConfigurationManager.AppSettings["ClientID"];
            string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];

            var appCred = new ClientCredential(clientID, clientSecret);
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext("https://login.windows.net/common");

            AuthenticationResult authResult = authContext.AcquireToken(new Uri(url).GetLeftPart(UriPartial.Authority), appCred, new UserAssertion(accessToken));
            return authResult.AccessToken;
        }

     
    }
}
