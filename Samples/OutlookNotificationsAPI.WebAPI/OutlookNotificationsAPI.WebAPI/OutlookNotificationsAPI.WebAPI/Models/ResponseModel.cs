using System.Collections.Generic;

namespace OutlookNotificationsAPI.Models
{
    public class ResponseModel<T>
    {
        public List<T> Value { get; set; }
    }
}
