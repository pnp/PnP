using Newtonsoft.Json;
using System;

namespace OutlookNotificationsAPI.Models
{
    public class PushSubscriptionModel
    {
        [JsonProperty("@odata.type")]
        public string Type
        {
            get
            {
                return "#Microsoft.OutlookServices.PushSubscription";
            }
        }

        public string Resource { get; set; }
        public string NotificationURL { get; set; }
        public string ChangeType { get; set; }
        public Guid ClientState { get; set; }
    }
}
