using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines the Recurrence Range for a series of events
    /// </summary>
    public class EventRecurrenceRange
    {
        /// <summary>
        /// The Start Date of the recurrence
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The End Date of the recurrence
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The number of occurrences
        /// </summary>
        public Int32 NumberOfOccurrences { get; set; }

        /// <summary>
        /// The reference TimeZone for the recurrence
        /// </summary>
        public String RecurrenceTimeZone { get; set; }

        /// <summary>
        /// The type of recurrence
        /// </summary>
        public RecurrenceRangeType Type { get; set; }
    }

    public enum RecurrenceRangeType
    {
        /// <summary>
        /// The recurrence will end at the end date
        /// </summary>
        EndDate,
        /// <summary>
        /// The recurrence will never end
        /// </summary>
        NoEnd,
        /// <summary>
        /// The recurrence will end after a number of occurrences
        /// </summary>
        Numbered,
    }
}