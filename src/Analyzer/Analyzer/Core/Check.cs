using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Analyzer.Core
{

    public static class Check
    {
        public static void NotNull<T>(T obj, string parameterName, [Optional]string errorMessage)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName, errorMessage ?? $"{parameterName} can not be null");
            }

        }

        public static void CallerLog<T>(
            ILogger<T> logger,
            LoggerExecutionPositions executioningPosition,
            string message = null,
            LogLevel logLevel = LogLevel.Debug,
            [CallerMemberName] string callerName = null)
        {
            var messageBuilder = new StringBuilder();

            switch (executioningPosition)
            {
                case LoggerExecutionPositions.Entrance:
                    messageBuilder.Append($"Entering [[{callerName}]] execution.");
                    break;

                case LoggerExecutionPositions.Exit:
                    messageBuilder.Append($"Exiting [[{callerName}]] execution.");
                    break;

                case LoggerExecutionPositions.Body:
                    messageBuilder.Append($"Executing logic inside[[{callerName}]] body.");
                    break;

                default:
                    throw new InvalidOperationException("Feed.Web.Helpers::Check::CallerLog() - Unkowned LoggerExecutionPositions type: " + Enum.GetName(typeof(LoggerExecutionPositions), executioningPosition));
            }

            if (!String.IsNullOrEmpty(message))
            {
                messageBuilder.Append($"   {message}");
                return;
            }

            var builderMessage = messageBuilder.ToString();

            switch(logLevel)
            {
                case LogLevel.None:
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(builderMessage);
                    break;
                case LogLevel.Information:
                    logger.LogInformation(builderMessage);
                    break;
                case LogLevel.Critical:
                    logger.LogCritical(builderMessage);
                    break;
                case LogLevel.Error:
                    logger.LogError(builderMessage);
                    break;
                default: //defaults to debug
                    logger.LogDebug(builderMessage);
                    //logger.LogInformation(builderMessage);
                    break;
            }
        }
    }
}
