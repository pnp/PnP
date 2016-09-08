using NLog;
using System;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    public static class LoggerExtensions
    {
        #region Public Methods

        // ==============================================================================================================
        /// <summary>
        /// Logs the specified message as a more visible "section" 
        /// </summary>
        /// <param name="logger">The current Logger</param>
        /// <param name="message">The message that needs to be logged as a "section"</param>
        /// <param name="logLevel">The <b>LogLevel</b> in which the section needs to be logged</param>
        // ==============================================================================================================
        public static void Section(this Logger logger, String message, LogLevel logLevel)
        {
            logger.Log(logLevel, "================================================");
            logger.Log(logLevel, message);
            logger.Log(logLevel, "================================================");
        }


        // ==============================================================================================================
        /// <summary>
        /// Logs the specified message as a more visible "section" 
        /// </summary>
        /// <param name="logger">The current Logger</param>
        /// <param name="message">The message that needs to be logged as a "section"</param>
        /// <param name="logLevel">The <b>LogLevel</b> in which the section needs to be logged</param>
        /// <param name="args">An object array that contains zero or more objects to format</param>
        // ==============================================================================================================
        public static void Section(this Logger logger, String message, LogLevel logLevel, params object[] args)
        {
            logger.Section(String.Format(message, args), logLevel);
        }


        // ==============================================================================================================
        /// <summary>
        /// Logs the specified message as a more visible "section" 
        /// </summary>
        /// <param name="logger">The current Logger</param>
        /// <param name="message">The message that needs to be logged as a "section"</param>
        /// <param name="logLevel">The <b>LogLevel</b> in which the sub section needs to be logged</param>
        // ==============================================================================================================
        public static void SubSection(this Logger logger, String message, LogLevel logLevel)
        {
            logger.Log(logLevel, "-----------------------------------");
            logger.Log(logLevel, message);
            logger.Log(logLevel, "-----------------------------------");
        }


        // ==============================================================================================================
        /// <summary>
        /// Logs the specified message as a more visible "section" 
        /// </summary>
        /// <param name="logger">The current Logger</param>
        /// <param name="message">The message that needs to be logged as a "section"</param>
        /// <param name="logLevel">The <b>LogLevel</b> in which the sub section needs to be logged</param>
        /// <param name="args">An object array that contains zero or more objects to format</param>
        // ==============================================================================================================
        public static void SubSection(this Logger logger, String message, LogLevel logLevel, params object[] args)
        {
            logger.SubSection(String.Format(message,args), logLevel);
        }

        #endregion
    }
}
