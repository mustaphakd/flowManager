using Refit;
using System.Threading;
using System.Threading.Tasks;
//using Refit.Insane.PowerPack.Retry;

//[assembly: RefitRetryAttribute(3)]
namespace Analyzer.Services
{
    public interface IAuthenticationEndpoint
    {
        //[RefitRetry]
        [Post("")] //RestEndpoints.Login
        Task<AuthModel> LoginAsync(UserLogin credentials, CancellationToken cancellationToken = default(CancellationToken));

        //[RefitRetry]
        [Post("")] //RestEndpoints.Registation
        Task RegisterNewUserAsync(UserRegistration details, CancellationToken cancellationToken = default(CancellationToken));

        [Post("")] //<System.Net.HttpStatusCode> //RestEndpoints.Logout
        Task Logout(CancellationToken cancellationToken = default(CancellationToken));
    }
}
