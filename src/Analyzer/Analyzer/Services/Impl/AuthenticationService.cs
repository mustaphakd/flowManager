using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akavache;
using Analyzer.Core;
using Microsoft.Extensions.Logging;
using Analyzer.Infrastructure;
using Xamarin.Forms;
using System.Security.Cryptography;
using System.Reactive.Linq;
using Analyzer.Services.RequestSettings;
using Analyzer.Operations;

namespace Analyzer.Services.Impl
{
    public class AuthenticationService : IAuthenticationService
    {
        private ILoggingService logger_;
        public AuthenticationService()
        {
            var logger = DependencyService.Get<ILoggingService>();
            Check.NotNull(logger, nameof(logger));
            this.logger_ = logger;
        }

        private AuthModel Model { get; set; }

        public bool IsAuthenticated => this.Model != null;

        public string UserName { get; set; } = "XXXX";

        public Task<OperationResult<bool>> LoginAsync(UserLogin credentials)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> RegisterNewUserAsync(UserRegistration details)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> Logout()
        {
            throw new NotImplementedException();
        }
        /*

       public async Task<Operations.OperationResult<bool>> LoginAsync(UserLogin credentials)
       {
           Check.NotNull(credentials, nameof(credentials));
           AuthenticatedUser authenticatedUser = null;

           if(string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
           {
               return "error";
           }

           var settingsService = DependencyService.Get<ISettingsService>();
           var isDebugMode = settingsService.GetSettings().EnableDebugMode;

           if(isDebugMode == true)
           {
               UserName = credentials.Email;
               return true;
           }

           BaseApiConfiguration.ApiUri = new Uri(settingsService.GetSettings().ServerEndpoint, UriKind.Absolute);

           try
           {
               //pull from cache
               authenticatedUser = await BlobCache.UserAccount.GetObject<AuthenticatedUser>(CacheKeys.User_Auth);
           }
           catch (KeyNotFoundException)
           {
               logger_.LogInformation($"AuthenticationService::LoginAsync() Failed to retrieve authmodel from cache.");
           }

           var hasher = System.Security.Cryptography.HMACSHA1.Create();
           var authenticatedUserPwd = authenticatedUser?.PwdHsh;
           hasher.Key = System.Text.Encoding.Unicode.GetBytes("app_code*");
           var pwdHshr = hasher.ComputeHash(Encoding.Unicode.GetBytes(credentials.Password));
           var pwdHshStr = Encoding.Unicode.GetString(pwdHshr);

           //ok return that
           if (authenticatedUser != null && authenticatedUser.UserName.Equals(credentials.Email) &&
               String.Equals(pwdHshStr, authenticatedUser.PwdHsh))
           {
               RegisterAuthTokenForRequests( new AuthModel(authenticatedUser.AuthToken));
               authenticatedUser.LastLogin = DateTime.Now.ToString();
               await BlobCache.UserAccount.InsertObject<AuthenticatedUser>(CacheKeys.User_Auth, authenticatedUser);
               return true;
           }

           // using polly
           //try to authenticate no more than 3 times
           //onFailure notify user via pop up
           var response = await SharedSettings.RestService.Execute<IAuthenticationEndpoint, AuthModel>(api => api.LoginAsync(credentials, default(System.Threading.CancellationToken)));

           if(response.IsSuccess == false)
           {
               return response.FormattedErrorMessages ?? "An error occured trying to log user in";
           }

           var authModel = response.Results;
           authenticatedUser = new AuthenticatedUser
           {
               AuthToken = authModel.Token,
               LastLogin = DateTime.Now.ToString(),
               PwdHsh = pwdHshStr,
               UserName = credentials.Email
           };
           await BlobCache.UserAccount.InsertObject<AuthenticatedUser>(CacheKeys.User_Auth, authenticatedUser);
           RegisterAuthTokenForRequests(authModel);

           return true;
       }

       private void RegisterAuthTokenForRequests(AuthModel authModel)
       {
           logger_.LogInformation($"AuthenticationService::RegisterAuthTokenForRequests() - Start Registering authTOken: ", authModel);

           var authenticatedHandler = new Func<Task<string>>(() =>
           {
               var localToken = authModel.Token;
               return Task.FromResult(localToken);
           });

           SharedSettings.RefitSettings.AuthorizationHeaderValueGetter = authenticatedHandler;

           //todo: add it also for graphQl bearer
       }

       public async Task<Operations.OperationResult<bool>> Logout()
       {
           // clearCache
           await BlobCache.UserAccount.InvalidateObject<AuthModel>(CacheKeys.User_Auth);
           //sent request to server
           //using polly 3 times max internal to powerpack
           var response = await SharedSettings.RestService.Execute<IAuthenticationEndpoint>(api => api.Logout(default(System.Threading.CancellationToken)));

           if (response.IsSuccess == false)
           {
               return response.FormattedErrorMessages ?? "An error occured trying to log user in";
           }

           return true;
       }

       public async Task<Operations.OperationResult<bool>> RegisterNewUserAsync(UserRegistration details)
       {
           //check cache for existing
           //currently update not enabled.
           //if exist return error message
           UserRegistration userDetails = null;
           Check.NotNull(details, nameof(details));


           if (string.IsNullOrEmpty(details.Email) || string.IsNullOrEmpty(details.Password))
           {
               return "Error";
           }

           var settingsService = DependencyService.Get<ISettingsService>();
           var isDebugMode = settingsService.GetSettings().EnableDebugMode;

           if (isDebugMode == true)
           {
               return true;
           }

           BaseApiConfiguration.ApiUri = new Uri(settingsService.GetSettings().ServerEndpoint, UriKind.Absolute);


           try
           {
               //pull from cache
               userDetails = await BlobCache.Secure.GetObject<UserRegistration>(CacheKeys.User_Profile);
           }
           catch (KeyNotFoundException)
           {
               logger_.LogInformation($"AuthenticationService::LoginAsync() Failed to retrieve authmodel from cache.");
           }

           //ok return that
           if (userDetails != null)
           {
               return "$We currently do not supporting updating your registration details";
           }

           //using polly max 5 times send request to server
           //if succeed, add to cache
           // else return errormessage
           var response = await SharedSettings.Execute<IAuthenticationEndpoint>(api => api.RegisterNewUserAsync(details, default(System.Threading.CancellationToken)));

           if (response.IsSuccess == false)
           {
               return response.FormattedErrorMessages ?? "An error occured trying to register user";
           }

           await BlobCache.Secure.InsertObject<UserRegistration>(CacheKeys.User_Profile, details);
           return true;

       }*/
    }


}
