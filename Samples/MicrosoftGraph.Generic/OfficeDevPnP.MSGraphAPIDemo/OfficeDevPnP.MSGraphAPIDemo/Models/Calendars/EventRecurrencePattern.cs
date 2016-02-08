using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines the Recurrence Pattern for a series of events
    /// </summary>
    public class EventRecurrencePattern
    {
        /// <summary>
        /// The day of the month for the recurrence
        /// </summary>
        public Int32 DayOfMonth { get; set; }

        /// <summary>
        /// The days of the week for the recurrence
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public DayOfWeek[] DaysOfWeek { get; set; }

        /// <summary>
        /// The first day of the week
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DayOfWeek FirstDayOfWeek { get; set; }

        /// <summary>
        /// The week of the month
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public WeekIndex Index { get; set; }

        /// <summary>
        /// The interval for repeating occurrences
        /// </summary>
        public Int32 Interval { get; set; }

        /// <summary>
        /// The month for the recurrence
        /// </summary>
        public Int32 Month { get; set; }

        /// <summary>
        /// The type of recurrence
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RecurrenceType Type { get; set; }
    }

    /// <summary>
    /// Index of the week in the month
    /// </summary>
    public enum WeekIndex
    {
        /// <summary>
        /// First week
        /// </summary>
        First,
        /// <summary>
        /// Second week
        /// </summary>
        Second,
        /// <summary>
        /// Third week
        /// </summary>
        Third,
        /// <summary>
        /// Fourth week
        /// </summary>
        Fourth,
        /// <summary>
        /// Last week
        /// </summary>
        Last,
    }

    /// <summary>
    /// The type of recurrence
    /// </summary>
    public enum RecurrenceType
    {
        /// <summary>
        /// Daily
        /// </summary>
        Daily,
        /// <summary>
        /// Weekly
        /// </summary>
        Weekly,
        /// <summary>
        /// Absolute every month
        /// </summary>
        AbsoluteMonthly,
        /// <summary>
        /// Relative every month
        /// </summary>
        RelativeMonthly,
        /// <summary>
        /// Absolute every year
        /// </summary>
        AbsoluteYearly,
        /// <summary>
        /// Relative every year
        /// </summary>
        RelativeYearly,
    }
}