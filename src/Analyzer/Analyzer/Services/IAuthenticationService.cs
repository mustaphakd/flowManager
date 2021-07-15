using System;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface IAuthenticationService
    {

        Task<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<bool>> LoginAsync(IUserLogin credentials);

        Task<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<bool>> RegisterNewUserAsync(IUserRegistration details);

        Task<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<bool>> Logout();

        bool IsAuthenticated { get; }

        string UserName { get; }

        event EventHandler<AuthModel> Authenticated;
    }
}
