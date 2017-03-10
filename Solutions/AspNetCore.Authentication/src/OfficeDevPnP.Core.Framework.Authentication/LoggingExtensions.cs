namespace OfficeDevPnP.Core.Framework.Authentication
{
    using Microsoft.Extensions.Logging;
    using System;

    internal static class LoggingExtensions
    {
        private static Action<ILogger, string, Exception> _tokenValidationFailed;
        private static Action<ILogger, Exception> _tokenValidationSucceeded;
        private static Action<ILogger, Exception> _errorProcessingMessage;
        private static Action<ILogger, Exception> _cannotRedirect;

        static LoggingExtensions()
        {
            _tokenValidationFailed = LoggerMessage.Define<string>(
                logLevel: LogLevel.Information,
                eventId: 1,
                formatString: "Failed to validate the token {0}.");
            _tokenValidationSucceeded = LoggerMessage.Define(
                logLevel: LogLevel.Information, 
                eventId: 2,
                formatString: "Successfully validated the token.");
            _errorProcessingMessage = LoggerMessage.Define(
                logLevel: LogLevel.Error,
                eventId: 3,
                formatString: "Exception occurred while processing message.");
            _cannotRedirect = LoggerMessage.Define(
                logLevel: LogLevel.Information,
                eventId: 4,
                formatString: "Cannot find redirect information.");
        }

        public static void TokenValidationFailed(this ILogger logger, string token, Exception ex)
        {
            _tokenValidationFailed(logger, token, ex);
        }

        public static void TokenValidationSucceeded(this ILogger logger)
        {
            _tokenValidationSucceeded(logger, null);
        }

        public static void ErrorProcessingMessage(this ILogger logger, Exception ex)
        {
            _errorProcessingMessage(logger, ex);
        }

        public static void CannotRedirect(this ILogger logger)
        {
            _cannotRedirect(logger, null);
        }
    }
}