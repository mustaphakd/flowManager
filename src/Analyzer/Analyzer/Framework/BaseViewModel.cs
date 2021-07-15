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
        protected static readonly IAuthenticationService AuthenticationService;


        private bool isBusy;

        static BaseViewModel()
        {
            //todo: validate that ApplicationManager contructor is called before this static method
            LoggingService = DependencyService.Get<ILoggingService>();
            SettingsService = DependencyService.Get<ISettingsService>();
            AuthenticationService = DependencyService.Get<IAuthenticationService>();
        }

        /// <summary>
        /// View consuming this viewmodel should have a spinner or progressBar for this property
        /// </summary>
        public bool IsBusy
        {
            get => isBusy;
            set => SetAndRaisePropertyChanged(ref isBusy, value);
        }

        public bool Authenticated => AuthenticationService.IsAuthenticated;

        public ICommand FeatureNotAvailableCommand { get; } = new AsyncCommand(ShowFeatureNotAvailableAsync);

        public virtual Task InitializeAsync() => Task.CompletedTask;
        public virtual Task RenderedAsync() => Task.CompletedTask;

        public virtual Task UninitializeAsync() => Task.CompletedTask;

        protected static async Task ShowFeatureNotAvailableAsync()
        {
            //Todo: translate
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
            Func<Task<T>> operation,
            Func<Exception, Task<bool>> onError = null) =>
            await TaskHelper.Create()
                .WhenStarting(() => IsBusy = true)
                .WhenFinished(() => IsBusy = false)
                .TryWithErrorHandlingAsync(operation, onError);
    }
}