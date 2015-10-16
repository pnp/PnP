using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string DefaultDelimiter = ", ";

        /// <summary>
        /// Gets the <see cref="DescriptionAttribute"/> placed on an Enum as a string.
        /// </summary>
        /// <param name="value">Enum object</param>
        /// <returns>String attribute for the enum.</returns>
        public static string GetEnumDescription(this Enum value)
        {
            string description = string.Empty;

            if (value == null)
                return description;

            description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);

            var attribute = fieldInfo.GetCustomAttributes<DescriptionAttribute>(false).ToArray();

            if (attribute != null && attribute.Length > 0)
            {
                description = attribute[0].Description;
            }

            return description;
        }

        public static string TruncateString(this string text, int maxCharacters, string trailingText)
        {
            if (string.IsNullOrEmpty(text) || maxCharacters <= 0 || text.Length <= maxCharacters)
                return text;
            else
                return text.Substring(0, maxCharacters) + trailingText;
        }

    }
}
