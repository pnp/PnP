using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using UPSPackageTrackerWeb.Models;
using System.Configuration;

namespace UPSPackageTrackerWeb.Controllers
{
    public class UPSTrackingController : ApiController
    {
        // GET: api/UPSTracking/5
        public UPS.TrackResponse Get(string id)
        {
            //HACK hard code local storage for simplicity
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UPSTracking");
            table.CreateIfNotExists();

            PackageTrackerDetails ptd = null;
            TableOperation retrieveOperation = TableOperation.Retrieve<PackageTrackerDetails>(PackageTrackerDetails.GetPartitionKey(id), id);
            TableResult retrievedResult = table.Execute(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                ptd = (PackageTrackerDetails)retrievedResult.Result;
            }
            else
            {
                UPS.TrackResponse response = null;
                using (var tracker = new UPS.TrackPortTypeClient())
                {
                    response = tracker.ProcessTrack(
                        new UPS.UPSSecurity
                        {
                            UsernameToken = new UPS.UPSSecurityUsernameToken
                            {
                                Username = ConfigurationManager.AppSettings["UPSSecurityUsernameToken.Username"],
                                Password = ConfigurationManager.AppSettings["UPSSecurityUsernameToken.Password"]
                            },
                            ServiceAccessToken = new UPS.UPSSecurityServiceAccessToken
                            {
                                AccessLicenseNumber = ConfigurationManager.AppSettings["UPSSecurityServiceAccessToken.AccessLicenseNumber"]
                            }
                        },
                        new UPS.TrackRequest { InquiryNumber = id, Request = new UPS.RequestType { RequestOption = new string[] {"1"} }  }
                    );


                    ptd = new PackageTrackerDetails(id, null, response);
                    TableOperation insertOperation = TableOperation.Insert(ptd);
                    table.Execute(insertOperation);
                }

            }


            return ptd.UPSResponse;
        }
    }
}
