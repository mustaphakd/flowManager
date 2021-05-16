using Analyzer.Framework;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ConnectivityService))]

namespace Analyzer.Framework
{
    internal class ConnectivityService : IConnectivityService
    {
        public bool IsThereInternet => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
