using System.Text;

namespace System {
    [Obsolete("Method deprecated")]
    public static class DateTimeExtensions {
        #region [ AsTimeAgoString ]
        /// <summary>
        /// Gets time since an item may have been modified as a nicely formatted string (e.g. yesterday, a month ago, 7 weeks ago, etc.).
        /// </summary>
        /// <param name="modified">Last modified date.</param>
        /// <returns></returns>
        public static string AsTimeAgoString(this DateTimeOffset modified) {
            TimeSpan diff = DateTimeOffset.Now - modified;
            string dateString = string.Empty;
            bool isFullWord = false;

            if (diff.Days > 0) {
                dateString = diff.Days + " day";
                if (diff.Days == 1) {
                    isFullWord = true;
                    dateString = "yesterday";
                }
                else if (diff.Days > 1) {
                    dateString += "s";

                    // handle months
                    if (diff.Days < 7) {
                        isFullWord = true;
                        dateString = modified.DayOfWeek.ToString();
                    }
                    else if (diff.Days >= 7 && diff.Days < 30) {
                        double weekDiff = diff.Days / 7;
                        int weeks = (int)Math.Round(weekDiff);
                        dateString = weeks == 1 ? "a week" : weeks + " weeks";
                    }
                    else if (diff.Days >= 30) {
                        double monthDiff = diff.Days / 30;
                        int months = (int)Math.Round(monthDiff);
                        if (months == 1) {
                            //isFullWord = true;
                            dateString = "a month";
                        }
                        else if (monthDiff > 1) {
                            dateString = months + " months";
                        }
                    }
                }
            }
            else if (diff.Hours > 0) {
                dateString = diff.Hours + " hour";
                if (diff.Hours == 1)
                    dateString = "an hour";
                else if (diff.Hours > 1)
                    dateString += "s";
            }
            else if (diff.Minutes > 0) {
                dateString = diff.Minutes + " minute";
                if (diff.Minutes == 1)
                    dateString = "a minute";
                else if (diff.Minutes > 1)
                    dateString += "s";
            }
            else if (diff.Seconds > 0) {
                dateString = diff.Seconds + " second";
                if (diff.Seconds > 1)
                    dateString += "s";
            }
            else {
                dateString = "just a moment";
            }

            if (!isFullWord)
                dateString += " ago";

            return dateString;
        }

        /// <summary>
        /// Gets time since an item may have been modified as a nicely formatted string (e.g. yesterday, a month ago, 7 weeks ago, etc.).
        /// </summary>
        /// <param name="modified">Last modified date.</param>
        /// <returns></returns>
        [Obsolete("Method deprecated")]
        public static string AsTimeAgoString(this DateTime modified) {
            return AsTimeAgoString(modified, false);
        }

        /// <summary>
        /// Gets time since an item may have been modified as a nicely formatted string (e.g. yesterday, a month ago, 7 weeks ago, etc.).
        /// </summary>
        /// <param name="modified">Last modified date.</param>
        /// <param name="useLocalTime">Use local time or server time.</param>
        /// <returns></returns>
        [Obsolete("Method deprecated")]
        public static string AsTimeAgoString(this DateTime modified, bool useLocalTime) {
            TimeSpan diff = useLocalTime ? DateTime.Now.ToLocalTime() - modified.ToLocalTime() : DateTime.Now - modified;
            string dateString = string.Empty;
            bool isFullWord = false;

            if (diff.Days > 0) {
                dateString = diff.Days + " day";
                if (diff.Days == 1) {
                    isFullWord = true;
                    dateString = "yesterday";
                }
                else if (diff.Days > 1) {
                    dateString += "s";

                    // handle months
                    if (diff.Days < 7) {
                        isFullWord = true;
                        dateString = modified.DayOfWeek.ToString();
                    }
                    else if (diff.Days >= 7 && diff.Days < 30) {
                        double weekDiff = diff.Days / 7;
                        int weeks = (int)Math.Round(weekDiff);
                        dateString = weeks == 1 ? "a week" : weeks + " weeks";
                    }
                    else if (diff.Days >= 30) {
                        double monthDiff = diff.Days / 30;
                        int months = (int)Math.Round(monthDiff);
                        if (months == 1) {
                            //isFullWord = true;
                            dateString = "a month";
                        }
                        else if (monthDiff > 1) {
                            dateString = months + " months";
                        }
                    }
                }
            }
            else if (diff.Hours > 0) {
                dateString = diff.Hours + " hour";
                if (diff.Hours == 1)
                    dateString = "an hour";
                else if (diff.Hours > 1)
                    dateString += "s";
            }
            else if (diff.Minutes > 0) {
                dateString = diff.Minutes + " minute";
                if (diff.Minutes == 1)
                    dateString = "a minute";
                else if (diff.Minutes > 1)
                    dateString += "s";
            }
            else if (diff.Seconds > 0) {
                dateString = diff.Seconds + " second";
                if (diff.Seconds > 1)
                    dateString += "s";
            }
            else {
                dateString = "just a moment";
            }

            if (!isFullWord)
                dateString += " ago";

            return dateString;
        }
        #endregion

        #region [ GetTimeZoneAbbreviation ]
        [Obsolete("Method deprecated")]
        private static string GetTimeZoneAbbreviation()
        {
            StringBuilder abbreviations = new StringBuilder();

            string[] timeZoneWords = TimeZoneInfo.Local.StandardName.ToString().Split(' ');

            foreach (string word in timeZoneWords)
            {
                if ((!string.IsNullOrEmpty(word)) && (word.Length > 1))
                {
                    abbreviations.Append(word.Substring(0, 1));
                }
            }
            return abbreviations.ToString().ToUpper();
        }
        #endregion
    }
}