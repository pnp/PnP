using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Debug.TracingWeb
{
    public static class ErrorLogger
    {
        private const int ErrorMessageBlockLength = 990;
        private const string ErrorLoggerAppProductId = "ErrorLoggerAppProductId";

        private static IEnumerable<string> GetMessageParts(string message)
        {
            int messageCounter = 1;
            for (var i = 0; i < message.Length; i += ErrorMessageBlockLength)
            {
                string messagePart = message.Substring(i, Math.Min(ErrorMessageBlockLength, message.Length - i));
                messagePart = string.Format("ERROR PART{0}: {1}", messageCounter, messagePart);
                messageCounter++;
                yield return messagePart;
            }
        }
 
        private static string GetAppProductIdFromConfigFile()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[ErrorLoggerAppProductId] ?? string.Empty;
            return result;
        }

        public static void LogError(string errorMessage) //HttpContextBase context, TraceContext traceContext, 
        {
            try
            {
                //perform logging to trace
                try
                {
                    HttpContext.Current.Trace.Write("ErrorLogger", errorMessage);
                }
                catch (Exception) { } //tracing should not prevent saving error
 
                //perform logging to Sharepoint
                var sharepointContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
 
                using (var clientContext = sharepointContext.CreateUserClientContextForSPHost())
                {
                    //if error is less then 1000 chars, log error, else split error in parts and log parts
                    if (errorMessage.Length < ErrorMessageBlockLength + 10)
                    {
                        Utility.LogCustomRemoteAppError(clientContext, new Guid(GetAppProductIdFromConfigFile()), errorMessage);
                        clientContext.ExecuteQuery();
                    }
                    else
                    {
                        foreach (var messagePart in GetMessageParts(errorMessage))
                        {
                            Utility.LogCustomRemoteAppError(clientContext, new Guid(GetAppProductIdFromConfigFile()), messagePart);
                            clientContext.ExecuteQuery();
                        }
                    }
                }
            }
            catch (Exception) { } // App should not crash due to crash in Errorlogger
        }
 
        public static void LogException(Exception eX)
        {
            try
            {
                if (eX != null)
                {   
                    LogError(eX.ToString());
                }
            }
            catch (Exception) { } //App should not crash due to crash in Errorlogger
        }
    }
}