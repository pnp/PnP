using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace BusinessApps.RemoteCalendarAccess.Models.CalendarModel
{
    public class RecurrenceHelper
    {
        public string BuildRecurrence(string recurrenceData, DateTime endDate)
        {
            dynamic json = ConvertToDynamic(recurrenceData);

            dynamic repeat = json.recurrence.rule.repeat;

            StringBuilder builder = new StringBuilder();
            
            builder.Append("RRULE:FREQ=");

            if (repeat.daily != null)
                AppendDaily(builder, repeat.daily);
            else if (repeat.weekly != null)
                AppendWeekly(builder, repeat.weekly, json);
            else if (repeat.monthly != null)
                AppendMonthly(builder, repeat.monthly);
            else if (repeat.monthlyByDay != null)
                AppendMonthlyByDay(builder, repeat.monthlyByDay);
            else if (repeat.yearly != null)
                AppendYearly(builder, repeat.yearly);
            else if (repeat.yearlyByDay != null)
                AppendYearlyByDay(builder, repeat.yearlyByDay);

            if(repeat.repeatInstances != null)
                builder.Append(";COUNT=" + repeat.repeatInstances);

            if(json.recurrence.rule.windowEnd != null)
                builder.Append(";UNTIL=" + DateTime.Parse(json.recurrence.rule.windowEnd.ToString()).ToString("yyyyMMddTHHmmssZ"));
            else
                builder.Append(";UNTIL=" + endDate.ToString("yyyyMMddTHHmmssZ"));
            
            return builder.ToString();
        }


        private void AppendDaily(StringBuilder builder, dynamic daily)
        {
            builder.Append("DAILY;INTERVAL=" + daily.dayFrequency);
        }

        private void AppendWeekly(StringBuilder builder, dynamic weekly, dynamic json)
        {
            builder.Append("WEEKLY;INTERVAL=" + weekly.weekFrequency + ";WKST=" + json.recurrence.rule.firstDayOfWeek);

            if (weekly.su == "TRUE" || weekly.mo == "TRUE" || weekly.tu == "TRUE" || weekly.we == "TRUE" ||
                weekly.th == "TRUE" || weekly.fr == "TRUE" || weekly.sa == "TRUE")
            {
                builder.Append(";BYDAY=");

                if (weekly.su == "TRUE")
                    builder.Append("SU,");
                if (weekly.mo == "TRUE")
                    builder.Append("MO,");
                if (weekly.tu == "TRUE")
                    builder.Append("TU,");
                if (weekly.we == "TRUE")
                    builder.Append("WE,");
                if (weekly.th == "TRUE")
                    builder.Append("TH,");
                if (weekly.fr == "TRUE")
                    builder.Append("FR,");
                if (weekly.sa == "TRUE")
                    builder.Append("SA,");

                builder.Length = builder.Length - 1;  //  Remove the trailing comma from above.
            }
        }

        private void AppendMonthly(StringBuilder builder, dynamic monthly)
        {
            builder.Append("MONTHLY;BYMONTHDAY=" + monthly.day + ";INTERVAL=" + monthly.monthFrequency);
        }

        private void AppendMonthlyByDay(StringBuilder builder, dynamic monthlyByDay)
        {
            builder.Append("MONTHLY;INTERVAL=" + monthlyByDay.monthFrequency);

            if (monthlyByDay.su == "TRUE" || monthlyByDay.mo == "TRUE" || monthlyByDay.tu == "TRUE" || monthlyByDay.we == "TRUE" ||
                monthlyByDay.th == "TRUE" || monthlyByDay.fr == "TRUE" || monthlyByDay.sa == "TRUE")
            {
                builder.Append(";BYDAY=");
                
                if (monthlyByDay.su == "TRUE")
                    builder.Append("SU,");
                if (monthlyByDay.mo == "TRUE")
                    builder.Append("MO,");
                if (monthlyByDay.tu == "TRUE")
                    builder.Append("TU,");
                if (monthlyByDay.we == "TRUE")
                    builder.Append("WE,");
                if (monthlyByDay.th == "TRUE")
                    builder.Append("TH,");
                if (monthlyByDay.fr == "TRUE")
                    builder.Append("FR,");
                if (monthlyByDay.sa == "TRUE")
                    builder.Append("SA,");

                builder.Length = builder.Length - 1;  //  Remove the trailing comma from above.
            }

            if (monthlyByDay.weekday == "TRUE" || monthlyByDay.weekend_day == "TRUE")
            {
                if(monthlyByDay.weekday == "TRUE")
                    builder.Append(";BYDAY=MO,TU,WE,TH,FR");
                else
                    builder.Append(";BYDAY=SA,SU");
            }

            switch ((string)monthlyByDay.weekdayOfMonth)
            {
                case "first":
                    builder.Append(";BYSETPOS=1");
                    break;
                case "second":
                    builder.Append(";BYSETPOS=2");
                    break;
                case "third":
                    builder.Append(";BYSETPOS=3");
                    break;
                case "fourth":
                    builder.Append(";BYSETPOS=4");
                    break;
                case "last":
                    builder.Append(";BYSETPOS=-1");
                    break;
                default:
                    throw new Exception("Invalid weekdayOfMonth value");
            }
        }    

        private void AppendYearly(StringBuilder builder, dynamic yearly)
        {
            builder.Append("YEARLY;INTERVAL=" + yearly.yearFrequency + ";BYMONTH=" + yearly.month + ";BYMONTHDAY=" + yearly.day);
        }

        private void AppendYearlyByDay(StringBuilder builder, dynamic yearlyByDay)
        {
            builder.Append("YEARLY" + ";BYMONTH=" + yearlyByDay.month);

            if (yearlyByDay.su == "TRUE" || yearlyByDay.mo == "TRUE" || yearlyByDay.tu == "TRUE" || yearlyByDay.we == "TRUE" ||
                yearlyByDay.th == "TRUE" || yearlyByDay.fr == "TRUE" || yearlyByDay.sa == "TRUE")
            {
                builder.Append(";BYDAY=");

                if (yearlyByDay.su == "TRUE")
                    builder.Append(yearlyByDay.yearFrequency + "SU,");
                if (yearlyByDay.mo == "TRUE")
                    builder.Append(yearlyByDay.yearFrequency + "MO,");
                if (yearlyByDay.tu == "TRUE")
                    builder.Append(yearlyByDay.yearFrequency + "TU,");
                if (yearlyByDay.we == "TRUE")
                    builder.Append(yearlyByDay.yearFrequency + "WE,");
                if (yearlyByDay.th == "TRUE")
                    builder.Append(yearlyByDay.yearFrequency + "TH,");
                if (yearlyByDay.fr == "TRUE")
                    builder.Append(yearlyByDay.yearFrequency + "FR,");
                if (yearlyByDay.sa == "TRUE")
                    builder.Append(yearlyByDay.yearFrequency + "SA,");

                builder.Length = builder.Length - 1;  //  Remove the trailing comma from above.
            }

            if (yearlyByDay.day == "TRUE")
            {
                switch ((string)yearlyByDay.weekdayOfMonth)
                {
                    case "first":
                        builder.Append(";BYMONTHDAY=1");
                        break;
                    case "second":
                        builder.Append(";BYMONTHDAY=2");
                        break;
                    case "third":
                        builder.Append(";BYMONTHDAY=3");
                        break;
                    case "fourth":
                        builder.Append(";BYMONTHDAY=4");
                        break;
                    case "last":
                        builder.Append(";BYMONTHDAY=-1");
                        break;
                    default:
                        throw new Exception("Invalid weekdayOfMonth value");
                }
            }

            if (yearlyByDay.weekday == "TRUE" || yearlyByDay.weekend_day == "TRUE")
            {
                if (yearlyByDay.weekday == "TRUE")
                    builder.Append(";BYDAY=MO,TU,WE,TH,FR");
                else
                    builder.Append(";BYDAY=SA,SU");
                switch ((string)yearlyByDay.weekdayOfMonth)
                {
                    case "first":
                        builder.Append(";BYMONTHPOS=1");
                        break;
                    case "second":
                        builder.Append(";BYMONTHPOS=2");
                        break;
                    case "third":
                        builder.Append(";BYMONTHPOS=3");
                        break;
                    case "fourth":
                        builder.Append(";BYMONTHPOS=4");
                        break;
                    case "last":
                        builder.Append(";BYMONTHPOS=-1");
                        break;
                    default:
                        throw new Exception("Invalid weekdayOfMonth value");
                }
            }
        }

        private dynamic ConvertToDynamic(string xmlData)
        {
            StringReader reader = new StringReader(xmlData);

            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            string json = JsonConvert.SerializeXmlNode(doc);
            json = json.Replace("@", "");
            dynamic jsonData = JsonConvert.DeserializeObject(json);

            return jsonData;
        }
    }
}