using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.RemoteCalendarAccess.Models.CalendarModel
{
    public enum EventType
    {
        NonRecurring = 0,
        Recurring = 1,
        Deleted = 3,
        Exception = 4
    }
}