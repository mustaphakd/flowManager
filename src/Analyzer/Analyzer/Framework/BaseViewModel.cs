using Analyzer.Infrastructure;
using Analyzer.Services;
using OperationResult;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Analyzer.Framework
{
    public abstract class BaseViewModel : BasePropertyChangedNotification
    {
        protected static readonly ILoggingService LoggingService;
        protected static readonly ISettingsService SettingsService;
        //protected static readonly IXSnack XSnackService;

        private bool isBusy;

        static BaseViewModel()
        {
            LoggingService = DependencyService.Get<ILoggingService>();
            //XSnackService = DependencyService.Get<IXSnack>();
            SettingsService = DependencyService.Get<ISettingsService>();
        }


        public bool IsBusy
        {
            get => isBusy;
            set => SetAndRaisePropertyChanged(ref isBusy, value);
        }

        public bool Authenticated => false; // AuthenticationService.IsUserAuthenticated;

        public ICommand FeatureNotAvailableCommand { get; } = new AsyncCommand(ShowFeatureNotAvailableAsync);

        public virtual Task InitializeAsync() => Task.CompletedTask;
        public virtual Task RenderedAsync() => Task.CompletedTask;

        public virtual Task UninitializeAsync() => Task.CompletedTask;

        protected static async Task ShowFeatureNotAvailableAsync()
        {
            await Application.Current.MainPage.DisplayAlert(
               "Feature Not Available",
                "Welcome",
                "OK");
        }

        protected async Task<Status> TryExecuteWithLoadingIndicatorsAsync(
            Task operation,
            Func<Exception, Task<bool>> onError = null) =>
            await TaskHelper.Create()
                .WhenStarting(() => IsBusy = true)
                .WhenFinished(() => IsBusy = false)
                .TryWithErrorHandlingAsync(operation, onError);

        protected async Task<Result<T>> TryExecuteWithLoadingIndicatorsAsync<T>(
            Task<T> operation,
            Func<Exception, Task<bool>> onError = null) =>
            await TaskHelper.Create()
                .WhenStarting(() => IsBusy = true)
                .WhenFinished(() => IsBusy = false)
                .TryWithErrorHandlingAsync(operation, onError);
    }
}