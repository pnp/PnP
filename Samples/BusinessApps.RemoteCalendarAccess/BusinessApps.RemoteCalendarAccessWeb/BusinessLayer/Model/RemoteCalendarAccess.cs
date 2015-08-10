using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLM = BusinessApps.RemoteCalendarAccessWeb.DataLayer.Model;

namespace BusinessApps.RemoteCalendarAccessWeb.BusinessLayer.Model
{
    public class RemoteCalendarAccess
    {
        public Guid ID { get; set; }
        public Guid CalendarId { get; set; }
        public string SiteAddress { get; set; }
        public string UserId { get; set; }
        public DateTime LastAccess { get; set; }

        public RemoteCalendarAccess(Guid calendarId, string siteAddress, string userId)
        {
            ID = Guid.NewGuid();
            CalendarId = calendarId;
            SiteAddress = siteAddress;
            UserId = userId;
            LastAccess = DateTime.UtcNow;
        }

        public RemoteCalendarAccess(DLM.RemoteCalendarAccess remoteCalendarAccess)
        {
            ID = remoteCalendarAccess.ID;
            CalendarId = remoteCalendarAccess.CalendarId;
            SiteAddress = remoteCalendarAccess.SiteAddress;
            UserId = remoteCalendarAccess.UserId;
            LastAccess = remoteCalendarAccess.LastAccess;
        }

        public T ToDataModel<T>() where T: class
        {
            if(typeof(T) == typeof(DLM.RemoteCalendarAccess))
            {
                DLM.RemoteCalendarAccess remoteCalendarAccess = new DLM.RemoteCalendarAccess();
                remoteCalendarAccess.ID = ID;
                remoteCalendarAccess.CalendarId = CalendarId;
                remoteCalendarAccess.SiteAddress = SiteAddress;
                remoteCalendarAccess.UserId = UserId;
                remoteCalendarAccess.LastAccess = LastAccess;

                return remoteCalendarAccess as T;
            }

            return default(T);
        }
    }
}
