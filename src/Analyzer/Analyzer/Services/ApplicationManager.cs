using Analyzer.Infrastructure;
using Analyzer.Services.Impl;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;

namespace Analyzer.Services
{
    public class ApplicationManager
    {
        Xamarin.Forms.Application _application;
        private bool _loading = true;
        private AuthModel _authModel;


        readonly DelegateWeakEventManager _loadingStateChangedEventManager = new DelegateWeakEventManager();
        readonly DelegateWeakEventManager _authenticationStateChangedEventManager = new DelegateWeakEventManager();
        readonly WeakEventManager<string> _messageNotificationEventManager = new WeakEventManager<string>();
        public ApplicationManager(Xamarin.Forms.Application application)
        {
            Core.Check.NotNull(application, nameof(application));
            _application = application;
        }

        public  Task Initialize()
        {
            this.RegisterServicesAndProviders();
            return Task.CompletedTask;
        }

        private void RegisterServicesAndProviders()
#pragma warning restore CC0091 // Use static method
        {

            //DependencyService.Register<MockDataStore>();
            DependencyService.Register<ILoggingService, DebugLoggingService>();

            DependencyService.Register<ILoggerFactory, Infrastructure.LoggerFactory>();
            DependencyService.Register<SettingsService>();
            DependencyService.Register<IDataStoreService, DataStoreService>();
            DependencyService.Register<Services.Impl.DialogService>();
            DependencyService.Register<IAuthenticationService, AuthenticationService>();
        }



        public event EventHandler LoadingStateChanged
        {
            add => _loadingStateChangedEventManager.AddEventHandler(value);
            remove => _loadingStateChangedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler AuthenticationStateChanged
        {
            add => _authenticationStateChangedEventManager.AddEventHandler(value);
            remove => _authenticationStateChangedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<string> NotificationMessageArrived
        {
            add => _messageNotificationEventManager.AddEventHandler(value);
            remove => _messageNotificationEventManager.RemoveEventHandler(value);
        }




        void OnLoadingStateChanged(bool isLoading)
        {
            _loading = isLoading;
            _loadingStateChangedEventManager.RaiseEvent(this, IsLoading, nameof(LoadingStateChanged));
        }
        void OnAuthenticationStateChanged(AuthModel model)
        {
            _authModel = model;
            _authenticationStateChangedEventManager.RaiseEvent(this, IsAuthenticated, nameof(AuthenticationStateChanged));
        }

        public void PostMessage(string message)
        {
            _messageNotificationEventManager.RaiseEvent(this, message, nameof(NotificationMessageArrived));
        }

        public bool IsLoading => _loading;
        public bool IsAuthenticated => _authModel != null;

        public Xamarin.Forms.Application Application => _application;
    }
}
