using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
/// <summary>
/// Defines the Recurrence for a series of events
/// </summary>
public class EventRecurrence
{
    /// <summary>
    /// The Recurrence Pattern
    /// </summary>
    public EventRecurrencePattern Pattern { get; set; }

    /// <summary>
    /// The Recurrence Range
    /// </summary>
    public EventRecurrenceRange Range { get; set; }
}
}