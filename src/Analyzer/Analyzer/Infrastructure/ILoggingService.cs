using Microsoft.Extensions.Logging;
using System;

namespace Analyzer.Infrastructure
{
    public interface ILoggingService : ILogger
    {
        void Debug(string message);

        void Warning(string message);

        void Error(Exception exception);
    }
}