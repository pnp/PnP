using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class DateTimeUtil
    {
        public static System.DateTime Add(System.DateTime time, System.TimeSpan timespan)
        {
            if (timespan == System.TimeSpan.Zero)
            {
                return time;
            }
            if (timespan > System.TimeSpan.Zero && System.DateTime.MaxValue - time <= timespan)
            {
                return DateTimeUtil.GetMaxValue(time.Kind);
            }
            if (timespan < System.TimeSpan.Zero && System.DateTime.MinValue - time >= timespan)
            {
                return DateTimeUtil.GetMinValue(time.Kind);
            }
            return time + timespan;
        }

        public static System.DateTime AddNonNegative(System.DateTime time, System.TimeSpan timeSpan)
        {
            if (timeSpan < System.TimeSpan.Zero)
            {
                throw new System.ArgumentException("TimeSpan must be greater than or equal to TimeSpan.Zero.", "timeSpan");
            }
            return DateTimeUtil.Add(time, timeSpan);
        }

        public static System.DateTime GetMaxValue(System.DateTimeKind kind)
        {
            return new System.DateTime(System.DateTime.MaxValue.Ticks, kind);
        }

        public static System.DateTime GetMinValue(System.DateTimeKind kind)
        {
            return new System.DateTime(System.DateTime.MinValue.Ticks, kind);
        }

        public static System.DateTime? ToUniversalTime(System.DateTime? value)
        {
            if (!value.HasValue || value.Value.Kind == System.DateTimeKind.Utc)
            {
                return value;
            }
            return new System.DateTime?(DateTimeUtil.ToUniversalTime(value.Value));
        }

        public static System.DateTime ToUniversalTime(System.DateTime value)
        {
            if (value.Kind == System.DateTimeKind.Utc)
            {
                return value;
            }
            return value.ToUniversalTime();
        }
    }
}
