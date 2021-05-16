using Analyzer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Analyzer.Framework
{
    public class BaseService
    {
        protected static readonly ILoggingService LoggingService;

        static BaseService()
        {
            LoggingService = DependencyService.Get<ILoggingService>();
        }
    }
}

//Infrastructure => Framework => Services => Features