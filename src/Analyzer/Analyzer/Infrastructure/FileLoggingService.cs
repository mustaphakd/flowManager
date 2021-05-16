using Analyzer.Core;
using Analyzer.Framework.Files;
using Microsoft.Extensions.Logging;
using System;

namespace Analyzer.Infrastructure
{
    public class FileLoggingService : ILoggingService
    {
        private string _name;
        private const string encryptKey = "Mobile.Infrastructure.FileLoggingService : ILoggingService";
        private const string fileName = "logFile_";
        private string _logFile;
        private string _folderPath;
        public FileLoggingService() : this(String.Empty)
        {

        }

        public FileLoggingService(string name)
        {
            _name = name;
            _folderPath = FileSystemManager.ConstructFullPath(new[] { "logs", "records" });
            _logFile = fileName + DateTime.Now.ToString("yyyy_MM_dd") + ".rec";

            Framework.Files.FileSystemManager.EnsureDirectoryCreated(_folderPath);
            Framework.Files.FileSystemManager.EnsureFileCreated(Framework.Files.FileSystemManager.Combine(_folderPath, _logFile));

        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message); // check thread
            FileSystemManager.WriteLocalFileTextAsync(message, _logFile, _folderPath).GetAwaiter().GetResult();
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

