using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BusinessApps.RemoteCalendarAccess.Models.CalendarModel
{
    public class Timezone
    {
        private TimeZoneInfo _timeZoneInfo { get; set; }

        public static Timezone Parse(string timeZoneInfoDescription)
        {
            Timezone tz = new Timezone();
            tz._timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().Where(t => t.DisplayName.Replace("&", "and") == timeZoneInfoDescription).First();
            return tz;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("BEGIN:VTIMEZONE");
            builder.AppendLine("TZID:" + _timeZoneInfo.DisplayName);
            builder.AppendLine("LAST-MODIFIED:19870101T000000Z");

            if (!_timeZoneInfo.SupportsDaylightSavingTime)
            {
                builder.AppendLine("BEGIN:STANDARD");
                builder.AppendLine("DTSTART:19000101T000000Z");
                builder.AppendLine("TZOFFSETFROM:" + FormatTimeZoneOffset(_timeZoneInfo.BaseUtcOffset));
                builder.AppendLine("TZOFFSETTO:" + FormatTimeZoneOffset(_timeZoneInfo.BaseUtcOffset));
                builder.AppendLine("TZNAME:" + _timeZoneInfo.StandardName);
                builder.AppendLine("END:STANDARD");
            }
            else
            {
                foreach (System.TimeZoneInfo.AdjustmentRule rule in _timeZoneInfo.GetAdjustmentRules())
                {
                    DateTime dateStart = rule.DateStart.Date == DateTime.MinValue.Date ? new DateTime(1900, 1, 1) : rule.DateStart;
                    DateTime dateEnd = rule.DateEnd.Date == DateTime.MaxValue.Date ? new DateTime(2100, 1, 1) : rule.DateEnd;

                    builder.AppendLine("BEGIN:STANDARD");
                    builder.AppendLine("DTSTART:" + dateStart.ToString("yyyyMMddT") + rule.DaylightTransitionEnd.TimeOfDay.ToString("HHmmssZ"));
                    builder.AppendLine("DTEND:" + dateEnd.ToString("yyyyMMddT") + rule.DaylightTransitionEnd.TimeOfDay.ToString("HHmmssZ"));
                    builder.AppendLine("TZOFFSETFROM:" + FormatTimeZoneOffset(_timeZoneInfo.BaseUtcOffset.Add(rule.DaylightDelta)));
                    builder.AppendLine("TZOFFSETTO:" + FormatTimeZoneOffset(_timeZoneInfo.BaseUtcOffset));
                    builder.AppendLine("TZNAME:" + _timeZoneInfo.StandardName);
                    builder.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=" + rule.DaylightTransitionEnd.Month +
                                                        ";BYWEEK=" + rule.DaylightTransitionEnd.Week +
                                                        ";BYDAY=" + rule.DaylightTransitionEnd.Day +
                                                            GetDayOfWeekCode(rule.DaylightTransitionEnd.DayOfWeek));
                    builder.AppendLine("END:STANDARD");
                    
                    builder.AppendLine("BEGIN:DAYLIGHT");
                    builder.AppendLine("DTSTART:" + dateStart.ToString("yyyyMMddT") + rule.DaylightTransitionStart.TimeOfDay.ToString("HHmmssZ"));
                    builder.AppendLine("DTEND:" + dateEnd.ToString("yyyyMMddT") + rule.DaylightTransitionStart.TimeOfDay.ToString("HHmmssZ"));
                    builder.AppendLine("TZOFFSETFROM:" + FormatTimeZoneOffset(_timeZoneInfo.BaseUtcOffset));
                    builder.AppendLine("TZOFFSETTO:" + FormatTimeZoneOffset(_timeZoneInfo.BaseUtcOffset.Add(rule.DaylightDelta)));
                    builder.AppendLine("TZNAME:" + _timeZoneInfo.DaylightName);
                    builder.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=" + rule.DaylightTransitionStart.Month +
                                                        ";BYWEEK=" + rule.DaylightTransitionStart.Week +
                                                        ";BYDAY=" + rule.DaylightTransitionStart.Day +
                                                            GetDayOfWeekCode(rule.DaylightTransitionStart.DayOfWeek));
                    builder.AppendLine("END:DAYLIGHT");
                }
            }

            builder.AppendLine("END:VTIMEZONE");

            return builder.ToString();
        }

        private string FormatTimeZoneOffset(TimeSpan offset)
        {
            if (offset.TotalSeconds > 0)
                return offset.ToString("'+'hhmm");
            else
                return offset.ToString("'-'hhmm");
        }

        private string GetDayOfWeekCode(DayOfWeek dayOfWeek)
        {
            switch(dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "SU";
                case DayOfWeek.Monday:
                    return "MO";
                case DayOfWeek.Tuesday:
                    return "TU";
                case DayOfWeek.Wednesday:
                    return "WE";
                case DayOfWeek.Thursday:
                    return "TH";
                case DayOfWeek.Friday:
                    return "FR";
                case DayOfWeek.Saturday:
                    return "SA";
                default:
                    throw new Exception("Day of week value not supported.");
            }
        }
    }
}