using System;
using System.Collections.Specialized;
using System.Web;

namespace System.Web
{
    /// <summary>
    /// Used to retrieve HttpVariables
    /// </summary>
    [Obsolete("Method deprecated")]
    public static class HttpVariablesExtensions
    {
        #region [ Definition ]
        /// <summary>
        /// Gets a value from the query string and provides a means to translate the item to another type.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <param name="operation">Operation to convert the query string value to the target type.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A query string value returned as the target type.</returns>
        public static T GetQueryString<T>(this NameValueCollection queryString, string parameterName, Func<string, T> operation, T defaultValue)
        {
            T returnValue = defaultValue;
            if (!string.IsNullOrEmpty(queryString[parameterName]))
            {
                return operation(queryString[parameterName]);
            }
            return returnValue;
        }
        #endregion

        #region [ HasVariable ]
        /// <summary>
        /// Determines whether a query string variable is present in the current request.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        [Obsolete("Method deprecated")]
        public static bool HasVariable(this NameValueCollection queryString, string variable)
        {
            return !string.IsNullOrEmpty(queryString[variable]);
        }
        #endregion

        #region [ AsString ]
        /// <summary>
        /// Gets a value as a string.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static string AsString(this NameValueCollection queryString, string parameterName, string defaultValue)
        {
            return GetQueryString(queryString, parameterName, value => value, defaultValue);
        }
        /// <summary>
        /// Gets a value as a string.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static string AsString(this NameValueCollection queryString, string parameterName)
        {
            return AsString(queryString, parameterName, string.Empty);
        }
        #endregion

        #region [ AsInt ]
        /// <summary>
        /// Gets a value as an integer.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static int AsInt(this NameValueCollection queryString, string parameterName, int defaultValue)
        {
            return GetQueryString(queryString, parameterName, value => value.ToInt32(), defaultValue);
        }

        /// <summary>
        /// Gets a value as an integer.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static int AsInt(this NameValueCollection queryString, string parameterName)
        {
            return AsInt(queryString, parameterName, 0);
        }
        #endregion

        #region [ AsLong ]
        /// <summary>
        /// Gets a value as a long.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static long AsLong(this NameValueCollection queryString, string parameterName, long defaultValue)
        {
            return GetQueryString(queryString, parameterName, value => value.ToInt64(), defaultValue);
        }

        /// <summary>
        /// Gets a value as a long.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static long AsLong(this NameValueCollection queryString, string parameterName)
        {
            return AsLong(queryString, parameterName, (long)0);
        }
        #endregion

        #region [ AsBool ]
        /// <summary>
        /// Gets a value as a Boolean.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static bool AsBool(this NameValueCollection queryString, string parameterName, bool defaultValue)
        {
            return GetQueryString(queryString, parameterName, value => value.ToBoolean(), defaultValue);
        }

        /// <summary>
        /// Gets a value as a Boolean.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static bool AsBool(this NameValueCollection queryString, string parameterName)
        {
            return AsBool(queryString, parameterName, false);
        }
        #endregion

        #region [ AsGuid ]
        /// <summary>
        /// Gets a value as a GUID.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static Guid AsGuid(this NameValueCollection queryString, string parameterName, Guid defaultValue)
        {
            return GetQueryString(queryString, parameterName, value => value.ToGuid(), defaultValue);
        }

        /// <summary>
        /// Gets a value as a GUID.
        /// </summary>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static Guid AsGuid(this NameValueCollection queryString, string parameterName)
        {
            return AsGuid(queryString, parameterName, Guid.Empty);
        }
        #endregion

        #region [ AsEnum ]
        /// <summary>
        /// 
        /// Gets a value as a Enum.
        /// </summary>
        /// <typeparam name="T">The enum type to return.</typeparam>
        /// <param name="queryString">This appears off of the Request.QueryString and Request.Form properties as an extension.</param>
        /// <param name="parameterName">The parameter to get from the collection.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A query string value returned as the target type.</returns>
        [Obsolete("Method deprecated")]
        public static T AsEnum<T>(this NameValueCollection queryString, string parameterName, T defaultValue)
        {
            try
            {
                return GetQueryString(queryString, parameterName, value => value.ToEnum<T>(), defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion
    }
}
