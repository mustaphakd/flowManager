using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Infrastructure
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILoggingService loggingService, Object payload)
        {
            loggingService.Debug(payload.ToString());
        }

        public static void Debug<T>(this ILoggingService logging, IEnumerable<T> set)
        {
            if(set == null)
            {
                logging.Debug($"Mobile::Infrastructure::LoggingExtensions::Debug() - enumerable set in null");
                return;
            }

            foreach(var item in set)
            {
                logging.Debug(item);
            }
        }
    }
}