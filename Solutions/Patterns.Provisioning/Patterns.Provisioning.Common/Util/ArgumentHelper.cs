using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Provisioning.Common.Util
{
    public static class ArgumentHelper
    {
        /// <summary>
        /// Validates if the string value is null or empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="argumentName"></param>
        /// <exception cref="System.ArgumentException">The value parameter is null or Empty.</exception>
        public static void RequireNotNullOrEmpty(this string value, string argumentName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("The value can't be null or empty", argumentName);
        }

        /// <summary>
        /// Validate if the Object is null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="argumentName"></param>
        /// <exception cref="System.ArgumentNullException">The value parameter is null.</exception>
        public static void RequireObjectNotNull(this object value, string argumentName)
        {
            if (value == null)
                throw new ArgumentNullException("The value can't be null", argumentName);
        }
    }
}
