using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client.Utilities;

namespace System
{
    public static class StringExtensions
    {
        public static string DefaultDelimiter = ", ";

        /// <summary>
        /// Convert a sequence of items to a delimited string. By default, ToString() will be called on each item in the sequence to formulate the result. The default delimiter of ', ' will be used
        /// </summary>
        [Obsolete("Method deprecated")]
        public static string ToDelimitedString<T>(this IEnumerable<T> source)
        {
            return source.ToDelimitedString(x => x.ToString(), DefaultDelimiter);
        }

        /// <summary>
        /// Convert a sequence of items to a delimited string. By default, ToString() will be called on each item in the sequence to formulate the result
        /// </summary>
        /// <param name="source"></param>
        /// <param name="delimiter">The delimiter to separate each item with</param>
        [Obsolete("Method deprecated")]
        public static string ToDelimitedString<T>(this IEnumerable<T> source, string delimiter)
        {
            return source.ToDelimitedString(x => x.ToString(), delimiter);
        }

        /// <summary>
        /// Convert a sequence of items to a delimited string. The default delimiter of ', ' will be used
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector">A lambda expression to select a string property of <typeparamref name="T"/></param>
        [Obsolete("Method deprecated")]
        public static string ToDelimitedString<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            return source.ToDelimitedString(selector, DefaultDelimiter);
        }

        /// <summary>
        /// Convert a sequence of items to a delimited string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector">A lambda expression to select a string property of <typeparamref name="T"/></param>
        /// <param name="delimiter">The delimiter to separate each item with</param>
        [Obsolete("Method deprecated")]
        public static string ToDelimitedString<T>(this IEnumerable<T> source, Func<T, string> selector, string delimiter)
        {
            if (source == null || source.Count() == 0)
                return string.Empty;

            if (selector == null)
                throw new ArgumentNullException("selector", "Must provide a valid property selector");

            if (string.IsNullOrEmpty(delimiter))
                delimiter = DefaultDelimiter;

            var sb = new StringBuilder();
            foreach (var item in source.Select(selector))
            {
                sb.Append(item);
                sb.Append(delimiter);
            }
            sb.Remove(sb.Length - delimiter.Length, delimiter.Length);
            return sb.ToString();
        }

        /// <summary>
        /// Splits a comma-separated string into an array of strings.  Returns an empty array if the string is null or empty.
        /// </summary>
        [Obsolete("Method deprecated")]
        public static IEnumerable<string> SplitCsv(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return new string[0];

            return s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Strips any non-alphanumeric characters from a string
        /// </summary>
        [Obsolete("Method deprecated")]
        public static string StripSpecialCharacters(this string s)
        {
            return Regex.Replace(s, @"\W+", string.Empty);
        }

        /// <summary>
        /// Strips any non-alphanumeric characters from a string
        /// </summary>
        [Obsolete("Method deprecated")]
        public static string StripSpecialCharacters(this string s, string replacementChar)
        {
            return Regex.Replace(s, @"\W+", replacementChar);
        }

        /// <summary>
        /// This was migrated code that was copy/pasted multiple places.
        /// Refactored here, but this needs to be refactored.
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        [Obsolete("Method deprecated")]
        public static string NormalizePageName(this string pageName)
        {
            string strFilteredName = pageName;
            //Character array for all the special characters
            char[] chars = "!@#€¥$£%^&* ()+=-[]\\;/{}|\":<>?".ToCharArray();
            //Looping the page name entered by the user in the modal popup to match that is there any special character in it.
            foreach (var c in chars) {
                strFilteredName = strFilteredName.Replace(c.ToString(), string.Empty);
            }
            return strFilteredName;
        }

        /// <summary>
        /// Html encodes a string value with the option to simply replace &lt; and &gt; characters.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tagCharactersOnly"></param>
        /// <returns></returns>
        [Obsolete("Method deprecated")]
        public static string HtmlEncode(this string value, bool tagCharactersOnly = true) {
            if (tagCharactersOnly)
                return value.Replace("<", "&lt;").Replace(">", "&gt;");
            return HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Transforms a string to a <see cref="System.Security.SecureString"/>.
        /// </summary>
        /// <param name="input">Input string to be transformed</param>
        /// <returns>Secure string</returns>
        [Obsolete("Method deprecated")]
        public static SecureString ToSecureString(this string input) {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input string is empty and cannot be made into a SecureString", "input");

            var secureString = new SecureString();
            foreach (char c in input.ToCharArray())
                secureString.AppendChar(c);

            return secureString;
        }

        /// <summary>
        /// Gets the <see cref="DescriptionAttribute"/> placed on an Enum as a string.
        /// </summary>
        /// <param name="value">Enum object</param>
        /// <returns>String attribute for the enum.</returns>
        [Obsolete("Method deprecated")]
        public static string ToEnumDescription(this Enum value) {
            string description = string.Empty;

            if (value == null)
                return description;

            description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);

            var attribute = fieldInfo.GetCustomAttributes<DescriptionAttribute>(false).ToArray();

            if (attribute != null && attribute.Length > 0) {
                description = attribute[0].Description;
            }

            return description;
        }

        /// <summary>
        /// Gets an Enum from the string description.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="inputStringValue">String description used in the Enum.</param>
        /// <returns>Enum that corresponds to the given input.</returns>
        [Obsolete("Method deprecated")]
        public static T FromEnumDescription<T>(this string inputStringValue) where T : struct {
            try {
                var type = typeof(T);

                if (!type.IsEnum)
                    return default(T);

                foreach (var field in type.GetFields()) {
                    var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attribute != null) {
                        if (attribute.Description == inputStringValue)
                            return (T)field.GetValue(null);
                    }
                    else {
                        if (field.Name == inputStringValue)
                            return (T)field.GetValue(null);
                    }
                }

                return default(T);
            }
            catch {
                return default(T);
            }
        }
    }
}
