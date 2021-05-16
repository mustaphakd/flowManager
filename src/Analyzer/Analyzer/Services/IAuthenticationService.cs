using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface IAuthenticationService
    {

        Task<Operations.OperationResult<bool>> LoginAsync(UserLogin credentials);

        Task<Operations.OperationResult<bool>> RegisterNewUserAsync(UserRegistration details);

        Task<Operations.OperationResult<bool>> Logout();

        bool IsAuthenticated { get; }

        string UserName { get; }
    }

    public interface UserLogin {
        string identifier { get; set; }
        string Password { get; }
        string Email { get; }
    }
    public interface UserRegistration { string identifier { get; set; } string Email { get; } string Password { get; set; } };
}
