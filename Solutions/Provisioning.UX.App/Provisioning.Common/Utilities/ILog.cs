using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Utilities
{
    public interface ILog
    {
        #region Information Logging
        /// <summary>
        /// Writes an informational message
        /// </summary>
        /// <param name="message"></param>
        void Information(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="vars"></param>
        void Information(string fmt, params object[] vars);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="fmt"></param>
        /// <param name="vars"></param>
        void Information(Exception exception, string fmt, params object[] vars);
        #endregion

        #region Warning Logging
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Warning(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="vars"></param>
        void Warning(string fmt, params object[] vars);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="fmt"></param>
        /// <param name="vars"></param>
        void Warning(Exception exception, string fmt, params object[] vars);
        #endregion

        #region Error Logging
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="vars"></param>
        void Error(string fmt, params object[] vars);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="fmt"></param>
        /// <param name="vars"></param>
        void Error(Exception exception, string fmt, params object[] vars);
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
        /// <param name="fmt"></param>
        /// <param name="vars"></param>
        void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars);
        #endregion

    }
}
