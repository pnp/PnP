using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UPSPackageTrackerWeb.Models
{
    public class PackageTrackerDetails : TableEntity
    {
        public PackageTrackerDetails()
        {

        }

        public PackageTrackerDetails(string referenceNumber, DateTime? cacheExpirationDate, UPS.TrackResponse upsResponse)
        {
            this.ReferenceNumber = referenceNumber;
            this.PartitionKey = GetPartitionKey(referenceNumber);
            this.RowKey = referenceNumber;
            this.CacheExpirationDate = cacheExpirationDate;
            this.UPSResponse = upsResponse;
        }
        public string ReferenceNumber { get; set; }
        public DateTime? CacheExpirationDate { get; set; }
        public string TrackResponseString
        {
            get
            {
                if (UPSResponse != null)
                    return Newtonsoft.Json.JsonConvert.SerializeObject(UPSResponse);
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    UPSResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UPS.TrackResponse>(value);
                } else
                {
                    UPSResponse = null;
                }
            }
        }
        public UPS.TrackResponse UPSResponse { get; set; }

        public static string GetPartitionKey(string referenceNumber)
        {
            return referenceNumber.Substring(0, 5);
        }
    }
}