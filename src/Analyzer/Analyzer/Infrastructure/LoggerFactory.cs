namespace Analyzer.Infrastructure
{
    public class LoggerFactory : Microsoft.Extensions.Logging.LoggerFactory
    {
        public LoggerFactory()
        {
            this.AddProvider(new LogProvider());
        }
    }
}
