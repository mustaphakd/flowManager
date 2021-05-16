using Microsoft.Extensions.Logging;

namespace Analyzer.Infrastructure
{
    public sealed class LogProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            //#if DEBUG
            //return new DebugLoggingService(categoryName);
            //#else
            return new FileLoggingService(categoryName);
//#endif
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}