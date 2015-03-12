using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
//The assembly for this is in Program Files\SharePoint Client Components\Assemblies
using Microsoft.Online.SharePoint.TenantAdministration;

namespace Provisioning.Cloud.WorkflowWeb.Services
{
    public class CreateSite : IRemoteEventService
    {
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            return result;
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {

            string SHAREPOINT_PID = "00000003-0000-0ff1-ce00-000000000000";  //This is hard-coded for SharePoint Online (ie - all tenants) 
            //The app must have tenant-level permissions and can be installed on any site in the tenancy. You must use the tenant
            //admin site url to get client context.
            Uri sharePointUrl = new Uri("https://<your-domain>-admin.sharepoint.com");
            string myRealm = TokenHelper.GetRealmFromTargetUrl(sharePointUrl);
            try
            {
                string accessToken = TokenHelper.GetAppOnlyAccessToken(SHAREPOINT_PID, sharePointUrl.Authority, myRealm).AccessToken;


                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(sharePointUrl.ToString(), accessToken))
                {
                    if (clientContext != null)
                    {

                        var requestTitle = properties.ItemEventProperties.AfterProperties["Title"];

                        var tenant = new Tenant(clientContext);
                        var newSite = new SiteCreationProperties()
                        {
                            Url = "https://<your domain>.sharepoint.com/sites/" + requestTitle,
                            Owner = "administrator@<your domain>.onmicrosoft.com",
                            Template = "STS#0",
                            Title = "Workflow provisioning test site two",
                            StorageMaximumLevel = 1000,
                            StorageWarningLevel = 500,
                            TimeZoneId = 7,
                            UserCodeMaximumLevel = 7,
                            UserCodeWarningLevel = 1,
                        };

                        var spoOperation = tenant.CreateSite(newSite);
                        clientContext.Load(spoOperation);
                        clientContext.ExecuteQuery();

                        while (!spoOperation.IsComplete)
                        {
                            System.Threading.Thread.Sleep(2000);
                            clientContext.Load(spoOperation);
                            clientContext.ExecuteQuery();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = ex;


            }
        }


    }
}




