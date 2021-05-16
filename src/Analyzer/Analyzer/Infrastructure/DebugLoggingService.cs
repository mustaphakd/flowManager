using Analyzer.Core;
using Microsoft.Extensions.Logging;
using System;

namespace Analyzer.Infrastructure
{
    public class DebugLoggingService : ILoggingService
    {
        private string _name;
        public DebugLoggingService() : this(String.Empty)
        {

        }

        public DebugLoggingService(string  name)
        {
            _name = name;
        }
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Warning(string message)
        {
            Debug($"# {nameof(Warning)}");
            Debug(message);
        }

        public void Error(Exception exception)
        {
            Debug($"# {nameof(Error)}");
            Debug(exception.ToString());
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            string text = formatter(state, exception);
            if (!string.IsNullOrEmpty(text))
            {
                text = $"${_name}-{logLevel}: {text}";
                if (exception != null)
                {
                    text = text + Environment.NewLine + Environment.NewLine + exception?.ToString();
                }
                Debug(text);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }
    }
}
