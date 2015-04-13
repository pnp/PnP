using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeDevPnP.Core.Extensions
{
    /// <summary>
    /// Extension methods to make working with IEnumerable<T> values easier.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Compares to instances of IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">The type of the enumerated item</typeparam>
        /// <param name="source">Source enumeration</param>
        /// <param name="target">Target enumeration</param>
        /// <returns>Wether the two enumerations are deep equal</returns>
        public static Boolean DeepEquals<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            return (source.Except(target).Count() == 0);
        }
    }
}
