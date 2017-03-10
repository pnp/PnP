# Sharing a SharePoint Online calendar via iCalendar #

### Summary ###
This sample shows a method of sharing a SharePoint site calendar with any calendar client via iCalendar.  The sample reads events from a SharePoint calendar and converts those events
into a standard iCalendar format utilizing SharePoint CSOM, Azure Active Directory Graph Client Library, SQL Azure via Entity Framework,
and Azure Web Apps.  


### Full walkthrough ###

A full walkthrough of the development process can be found at - 
[http://blog.jonathanhuss.com/sharing-a-sharepoint-online-calendar-via-icalendar/](http://blog.jonathanhuss.com/sharing-a-sharepoint-online-calendar-via-icalendar/)

### Applies to ###
- Office 365 / SharePoint Online

### Prerequisites ###
N/A

### Solution ###
Solution | Author(s)
---------|----------
BusinessApps.RemoteCalendarAccess | Jonathan Huss (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.0 | July 20th, 2015 | Initial Release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

### Creating the iCalendar ###

The basic flow in this sample is as follows:

1.  Calendar client requests calendar via GUID based URL from Azure Web App.
2.  Application retrieves user and SharePoint calendar connected to GUID from SQL Azure Database using Entity Framework.
3.  Application looks up user in Azure Active Directory and confirms user account exists and is active.
4.  Application gathers events from SharePoint calendar via CSOM and converts them to iCalendar format.
5.  iCalendar file is returned to calendar client.

The MVC action that does the bulk of the work looks like this:

```
public FileResult Index(Guid? Id)
{
    if (Id == null)
        return AccessDenied();

    RemoteCalendarAccessManager manager = new RemoteCalendarAccessManager();
    BLM.RemoteCalendarAccess remoteCalendarAccess = manager.GetRemoteCalendarAccess(Id.Value);

    if (remoteCalendarAccess == null)
        return AccessDenied();

    AzureActiveDirectory azureAD = new AzureActiveDirectory();

    IUser user = null;
    try
    {
        user = azureAD.GetUser(remoteCalendarAccess.UserId).Result;
    }
    catch (AggregateException e)
    {
        if (!e.InnerExceptions.Any(i => i.Message == "User " + remoteCalendarAccess.UserId + " not found."))
            throw;
    }

    if (user == null || user.AccountEnabled == false)
        return AccessDenied();

    manager.UpdateLastAccessTime(remoteCalendarAccess);

    Uri uri = new Uri(remoteCalendarAccess.SiteAddress);
    string realm = TokenHelper.GetRealmFromTargetUrl(uri);
    var token = TokenHelper.GetAppOnlyAccessToken("00000003-0000-0ff1-ce00-000000000000", uri.Authority, realm);
    ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(uri.ToString(), token.AccessToken);

    clientContext.Load(clientContext.Web.Lists);
    clientContext.ExecuteQuery();

    List list = clientContext.Web.Lists.Where(l => l.Id == remoteCalendarAccess.CalendarId).First();

    if (list == null)
        return AccessDenied();

    ListItemCollection items = list.GetItems(CamlQuery.CreateAllItemsQuery());
    clientContext.Load(items);

    clientContext.Load(clientContext.Web);
    clientContext.Load(clientContext.Web.RegionalSettings);
    clientContext.Load(clientContext.Web.RegionalSettings.TimeZone);
    clientContext.Load(clientContext.Web, w => w.Title);

    clientContext.ExecuteQuery();

    Calendar calendar = new Calendar();
    calendar.Title = clientContext.Web.Title + " - " + list.Title;
    calendar.Timezone = Timezone.Parse(clientContext.Web.RegionalSettings.TimeZone.Description);
    calendar.Events = items.Select(i => Event.Parse(i)).ToList<Event>();

    FileContentResult result = File(System.Text.Encoding.Default.GetBytes(calendar.ToString()), "text/calendar", "calendar.ics");

    return result;
}
```

### Events ###

Converting SharePoint events to iCalendar events is really fairly straightfoward.  It's just a matter of taking the SharePoint event data and injecting it into the iCalendar format.  The iCalendar
RFC can be found here:  [http://tools.ietf.org/html/rfc5545](http://tools.ietf.org/html/rfc5545).  The process for doing so looks like this:

```
public string ToString(List<Event> Events)
{
    StringBuilder builder = new StringBuilder();
 
    builder.AppendLine("BEGIN:VEVENT");
    builder.AppendLine("SUMMARY:" + CleanText(Title));
    builder.AppendLine("DTSTAMP:" + Created.ToString("yyyyMMddTHHmmssZ"));
    builder.AppendLine("DESCRIPTION:" + CleanText(Description));
    builder.AppendLine("LOCATION:" + CleanText(Location));
    builder.AppendLine("CATEGORIES:" + CleanText(Category));
    builder.AppendLine("UID:" + UID);
    builder.AppendLine("STATUS:CONFIRMED");
    builder.AppendLine("LAST-MODIFIED:" + LastModified.ToString("yyyyMMddTHHmmssZ"));
 
    if(AllDayEvent)
    {
        builder.AppendLine("DTSTART;VALUE=DATE:" + EventDate.ToString("yyyyMMdd"));
 
        double days = Math.Round(((Double)Duration / (double)(60 * 60 * 24)));
        builder.AppendLine("DTEND;VALUE=DATE:" + EventDate.AddDays(days).ToString("yyyyMMdd"));
    }
    else
    {
        builder.AppendLine("DTSTART:" + EventDate.ToString("yyyyMMddTHHmmssZ"));
        builder.AppendLine("DTEND:" + EventDate.AddSeconds(Duration).ToString("yyyyMMddTHHmmssZ"));
    }
 
    IEnumerable<Event> deletedEvents = Events.Where(e => e.MasterSeriesItemID == ID && e.EventType == EventType.Deleted);
    foreach(Event deletedEvent in deletedEvents)
    {
        if(AllDayEvent)
            builder.AppendLine("EXDATE;VALUE=DATE:" + deletedEvent.RecurrenceID.ToString("yyyyMMdd"));
        else
            builder.AppendLine("EXDATE:" + deletedEvent.RecurrenceID.ToString("yyyyMMddTHHmmssZ"));
    }
 
    if(RecurrenceID != DateTime.MinValue && EventType == EventType.Exception) //  Event is exception to recurring item
    {
        if(AllDayEvent)
            builder.AppendLine("RECURRENCE-ID;VALUE=DATE:" + RecurrenceID.ToString("yyyyMMdd"));
        else
            builder.AppendLine("RECURRENCE-ID:" + RecurrenceID.ToString("yyyyMMddTHHmmssZ"));
    }
    else if (Recurrence && !RecurrenceData.Contains("V3RecurrencePattern"))
    {
        RecurrenceHelper recurrenceHelper = new RecurrenceHelper();
        builder.AppendLine(recurrenceHelper.BuildRecurrence(RecurrenceData, EndDate));
    }
 
    if(EventType == CalendarModel.EventType.Exception)
    {
        List<Event> exceptions = Events.Where(e => e.MasterSeriesItemID == MasterSeriesItemID).OrderBy(e => e.Created).ToList<Event>();
        builder.AppendLine("SEQUENCE:" + (exceptions.IndexOf(this) + 1));
    }
    else
        builder.AppendLine("SEQUENCE:0");
 
    builder.AppendLine("BEGIN:VALARM");
    builder.AppendLine("ACTION:DISPLAY");
    builder.AppendLine("TRIGGER:-PT10M");
    builder.AppendLine("DESCRIPTION:Reminder");
    builder.AppendLine("END:VALARM");
 
    builder.AppendLine("END:VEVENT");
 
    return builder.ToString();
}
 
private string CleanText(string text)
{
    if (text != null)
        text = text.Replace(""", "DQUOTE")
                    .Replace("\", "\\")
                    .Replace(",", "\,")
                    .Replace(":", "":"")
                    .Replace(";", "\;")
                    .Replace("rn", "\n")
                    .Replace("n", "\n");
 
    return text;
}
```

### Using the iCalendar ###

The system generates a unique URL for the particular user and particular calendar.  Once the user has that URL, 
they can then load it into OWA.  The following image shows a SharePoint calendar.  It has a single event
 on the 9th, a weekly recurring event that occurs every Tuesday, and an exception to the recurring event
 on the 29th.

![Image of SharePoint calendar](http://blog.jonathanhuss.com/wp-content/uploads/2015/07/image14.png)

Using the iCalendar application, that same calendar loaded into OWA looks like this:

![Image of SharePoint calendar above in OWA](http://blog.jonathanhuss.com/wp-content/uploads/2015/07/image19.png)

### Bonus! ###

If the user syncs their Office 365 e-mail to their mobile devices, their OWA calendar can be automatically
synced as well.  This means that they can easily add any SharePoint calendar to their mobile device with very
little effort.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/BusinessApps.remoteCalendarAccess" />