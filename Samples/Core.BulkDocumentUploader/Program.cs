namespace Contoso.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Globalization;
    using Contoso.Core.BulkDocumentUploader;

    /// <summary>
    /// Main Program
    /// </summary>
    public class Program
    {
        #region Fields

        /// <summary>
        /// The base action type
        /// </summary>
        private static Type baseActionType = typeof(Contoso.Core.BaseAction);

        /// <summary>
        /// The base logger type
        /// </summary>
        private static Type baseLoggerType = typeof(Contoso.Core.LoggerBase);

        #endregion Fields

        #region Methods

        /// <summary>
        /// Deserializes the xml data into PropertyMapper
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="reader">The TextReader instance of the input XML.</param>
        /// <returns>
        /// An instance of the PropertyMapper
        /// </returns>
        private static T Deserialize<T>(TextReader reader)
        {
            XmlSerializer serialize = new XmlSerializer(typeof(PropertyMapper), GetPropertyMappingTypes());
            return (T)serialize.Deserialize(reader);
        }

        /// <summary>
        /// Gets the Exported types used to deserialize the data
        /// </summary>
        /// <returns>A collection of exported types</returns>
        private static Type[] GetPropertyMappingTypes()
        {
            return Assembly.GetExecutingAssembly().GetExportedTypes().Where(p => p.IsSubclassOf(baseActionType) || p.IsSubclassOf(baseLoggerType)).ToArray();
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            LogHelper logHelper = null;
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            // Time stamp current time for delta value 
            DateTime CurrentTime = DateTime.Now;

            try
            {
                if (args.Length == 0)
                {
                    throw new Exception("Please specify the input file...");
                }

                PropertyMapper mapper = null;

                using (TextReader reader = new StreamReader(args[0]))
                {
                    mapper = Deserialize<PropertyMapper>(reader);
                }

                if (mapper == null)
                {
                    throw new Exception("An error occurred whilst serializing the data. Ensure that the XML adheres to the schema");
                }

                logHelper = LogHelper.Instance(mapper.Loggers);
                
                RecurseActions(mapper.Actions, logHelper, CurrentTime);
            }
            catch (Exception ex)
            {
                if (logHelper != null)
                {
                    logHelper.LogException(string.Empty, ex);
                }
            }
            finally
            {
                sw.Stop();

                if (logHelper != null)
                {
                    logHelper.LogVerbose(string.Format(CultureInfo.CurrentCulture, "Operation completed. Execution time (s) {0}", sw.Elapsed.TotalSeconds));
                    logHelper.Dispose();
                }
            }
        }

        private static void RecurseActions( ActionCollection collection, LogHelper logHelper, DateTime CurrentTime, BaseAction parentAction = null)
        {
            foreach (BaseAction action in collection)
            {
                logHelper.LogVerbose(string.Format(CultureInfo.CurrentCulture, "Started '{0}'", action.GetType().FullName));
                action.Run(parentAction, CurrentTime, logHelper);

                if (action.Actions != null && action.Actions.Count > 0)
                {
                    RecurseActions(action.Actions, logHelper, CurrentTime, action);
                }

                logHelper.LogVerbose(string.Format(CultureInfo.CurrentCulture, "Completed '{0}'", action.GetType().FullName));
            }
        }

        #endregion Methods
    }
}