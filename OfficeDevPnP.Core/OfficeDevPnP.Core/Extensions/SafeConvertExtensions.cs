using System;

namespace System {
    /// <summary>
    /// Safely convert strings to specified types.
    /// </summary>
    public static class SafeConvertExtensions {
        #region [ ToBoolean ]
        /// <summary>
        /// Converts the input string to a boolean and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="defaultValue">A default value to return for a null input value.</param>
        public static bool ToBoolean(this string input, bool defaultValue) {
            try {
                return Convert.ToBoolean(input);
            }
            catch {
                return defaultValue;
            }
        }
        /// <summary>
        /// Converts the input string to a boolean and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        public static bool ToBoolean(this string input) {
            return ToBoolean(input, false);
        }
        #endregion

        #region [ ToInt32 ]
        /// <summary>
        /// Converts the input string to a Int32 and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="defaultValue">A default value to return for a null input value.</param>
        public static int ToInt32(this string input, int defaultValue) {
            try {
                return Convert.ToInt32(input);
            }
            catch {
                return defaultValue;
            }
        }
        /// <summary>
        /// Converts the input string to a Int64 and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        public static int ToInt32(this string input) {
            return ToInt32(input, 0);
        }
        #endregion

        #region [ ToInt64 ]
        /// <summary>
        /// Converts the input string to a Int32 and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="defaultValue">A default value to return for a null input value.</param>
        public static long ToInt64(this string input, int defaultValue) {
            try {
                return Convert.ToInt64(input);
            }
            catch {
                return defaultValue;
            }
        }
        /// <summary>
        /// Converts the input string to a Int32 and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        public static long ToInt64(this string input) {
            return ToInt64(input, 0);
        }
        #endregion

        #region [ ToDouble ]
        /// <summary>
        /// Converts the input string to a double and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="defaultValue">A default value to return for a null input value.</param>
        public static double ToDouble(this string input, double defaultValue) {
            try {
                return Convert.ToDouble(input);
            }
            catch {
                return defaultValue;
            }
        }
        /// <summary>
        /// Converts the input string to a double and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        public static double ToDouble(this string input) {
            return ToDouble(input, 0);
        }
        #endregion

        #region [ ToGuid ]
        /// <summary>
        /// Converts the input string to a Guid and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        public static Guid ToGuid(this string input) {
            return ToGuid(input, Guid.Empty);
        }
        /// <summary>
        /// Converts the input string to a Guid and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="defaultValue">A default value to return for a null input value.</param>
        public static Guid ToGuid(this string input, Guid defaultValue) {
            try {
                return new Guid(input);
            }
            catch {
                return defaultValue;
            }
        }
        #endregion

        #region [ ToEnum ]
        public static T ToEnum<T>(this int enumValue) {
            if (false == typeof(T).IsEnum)
                throw new NotSupportedException(typeof(T).Name + " must be an Enum");

            return (T)Enum.ToObject(typeof(T), enumValue);
        }
        public static T ToEnum<T>(this byte enumValue) {
            if (false == typeof(T).IsEnum)
                throw new NotSupportedException(typeof(T).Name + " must be an Enum");

            return (T)Enum.ToObject(typeof(T), enumValue);
        }
        public static T ToEnum<T>(this string name) {
            if (false == typeof(T).IsEnum)
                throw new NotSupportedException(typeof(T).Name + " must be an Enum");

            if (false == Enum.IsDefined(typeof(T), name))
                throw new ArgumentException(string.Format("{0} is not defined in type of enum {1}", name, typeof(T).Name));

            return (T)Enum.Parse(typeof(T), name, true);
        }
        #endregion
    }
}