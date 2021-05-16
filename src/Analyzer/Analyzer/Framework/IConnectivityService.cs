namespace Analyzer.Framework
{
    public interface IConnectivityService
    {
        bool IsThereInternet { get; }
    }
}