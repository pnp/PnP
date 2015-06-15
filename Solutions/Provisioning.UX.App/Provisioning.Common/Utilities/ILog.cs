using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Utilities
{
    /// <summary>
    /// An Interface for working with the logging component.
    /// </summary>
    public interface ILog
    {
        #region Information Logging
        /// <summary>
        /// Writes an informational message
        /// </summary>
        /// <param name="message"></param>
        void Information(string message);
        /// <summary>
        /// Writes an informational message using the specified array of objects and formatting information
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args arra</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void Information(string format, params object[] args);
        /// <summary>
        /// Writes an informational message using the Exception that occured, specified array of objects and formatting information
        /// </summary>
        /// <param name="exception">A Exception that has occured</param>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args arra</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void Information(Exception exception, string format, params object[] args);
        #endregion

        #region Warning Logging
        /// <summary>
        /// Writes an Warning message
        /// </summary>
        /// <param name="message"></param>
        void Warning(string message);
        /// <summary>
        /// Writes an Warning message using the specified array of objects and formatting information
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args arra</param>
   
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void Warning(string format, params object[] args);
        /// <summary>
        /// Writes an Warning message using the Exception that occured, specified array of objects and formatting information
        /// </summary>
        /// <param name="exception">A Exception that has occured</param>/// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args arra</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void Warning(Exception exception, string format, params object[] args);
        #endregion

        #region Error Logging
        /// <summary>
        /// Writes an Error message
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);
        /// <summary>
        /// Writes an informational message using the specified array of objects and formatting information
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args arra</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void Error(string format, params object[] args);
        /// <summary>
        /// Writes an Error message using the Exception that occured, specified array of objects and formatting information
        /// </summary>
        /// <param name="exception">A Exception that has occured</param>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args arra</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void Error(Exception exception, string format, params object[] args);
        #endregion

        #region External Service Logging 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="method"></param>
        /// <param name="timespan"></param>
        void TraceApi(string componentName, string method, TimeSpan timespan);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="method"></param>
        /// <param name="timespan"></param>
        /// <param name="properties"></param>
        void TraceApi(string componentName, string method, TimeSpan timespan, string properties);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="method"></param>
        /// <param name="timespan"></param>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args arra</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void TraceApi(string componentName, string method, TimeSpan timespan, string format, params object[] args);
        #endregion

    }
}
