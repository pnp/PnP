using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S
{
    public class EpochTime
    {
        public static readonly System.DateTime UnixEpoch = 
            new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        private long _secondsSinceUnixEpoch;

        public long SecondsSinceUnixEpoch
        {
            get
            {
                return this._secondsSinceUnixEpoch;
            }
        }

        public System.DateTime DateTime
        {
            get
            {
                System.TimeSpan timeSpan = System.TimeSpan.FromSeconds((double)this._secondsSinceUnixEpoch);
                return DateTimeUtil.AddNonNegative(EpochTime.UnixEpoch, timeSpan);
            }
        }

        public EpochTime(string secondsSinceUnixEpochString)
        {
            long secondsSinceUnixEpoch;
            if (!long.TryParse(secondsSinceUnixEpochString, out secondsSinceUnixEpoch))
            {
                throw new System.ArgumentException("Invalid date time string format.", "secondsSinceUnixEpochString");
            }
            this._secondsSinceUnixEpoch = secondsSinceUnixEpoch;
        }

        public EpochTime(long secondsSinceUnixEpoch)
        {
            if (secondsSinceUnixEpoch < 0L)
            {
                throw new System.ArgumentException("secondsSinceUnixEpoch must be greater than or equal to zero.", "secondsSinceUnixEpoch");
            }
            this._secondsSinceUnixEpoch = secondsSinceUnixEpoch;
        }

        public EpochTime(System.DateTime dateTime)
        {
            if (dateTime < EpochTime.UnixEpoch)
            {
                string message = string.Format(System.Globalization.CultureInfo.InvariantCulture, "DateTime must be greater than or equal to {0}", new object[]
                {
                    EpochTime.UnixEpoch.ToString()
                });
                throw new System.ArgumentOutOfRangeException("dateTime", message);
            }
            this._secondsSinceUnixEpoch = (long)(dateTime - EpochTime.UnixEpoch).TotalSeconds;
        }
    }
}
