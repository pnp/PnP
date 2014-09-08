using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Validates an object for not being null or not being the default value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The objec tto check</param>
        /// <param name="variableName">The name of the variable name to report in the error</param>
        /// <exception cref="System.ArgumentException">Thrown when variable is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when variable is null</exception>
        public static void ValidateNotNullOrEmpty<T>(this T input, string variableName)
        {
            if (typeof(T) == typeof(string))
            {
                if (string.IsNullOrEmpty(input as string))
                {
                    throw (input == null)
                      ? new ArgumentNullException(variableName)
                      : new ArgumentException(Constants.EXCEPTION_MSG_EMPTYSTRING_ARG, variableName);
                }
            }
            else if (typeof(T).IsSubclassOf(typeof(ClientObject)))
            {
                if (input == null || (input as ClientObject).ServerObjectIsNull == true)
                {
                    throw new ArgumentNullException(variableName);
                }
            }
            else
            {
                if (EqualityComparer<T>.Default.Equals(input, default(T)))
                {
                    throw new ArgumentException(variableName);
                }

            }
        }
    }
}
