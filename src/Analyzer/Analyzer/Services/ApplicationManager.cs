using Analyzer.Framework.Files;
using Analyzer.Infrastructure;
using Analyzer.Models;
using Analyzer.Services.Impl;
using Analyzer.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Analyzer.Services
{
    public class ApplicationManager
    {
        Xamarin.Forms.Application _application;
        private bool _loading = true;
        private bool _connected = true;
        private UpdateModel _updateModel;
        private AuthModel _authModel = null;
        private bool _isFirstUsage = true;
        private bool _persistAuthenticationAcrossReload = false;
        private bool _authTimerRunning = false;
        private bool _authContinueRunningTimer = false;
        private string _appUpdateUrl;
        private string _appVersion;

        private int _allowDuration = 20;

        const string ISFIRSTUSAGE = "is_first_usage";
        const string LASTAPPUSAGE = "last_app_usage";
        const string APPVERSION = "application_version";


        readonly DelegateWeakEventManager _loadingStateChangedEventManager = new DelegateWeakEventManager();
        readonly DelegateWeakEventManager _authenticationStateChangedEventManager = new DelegateWeakEventManager();
        readonly WeakEventManager<string> _messageNotificationEventManager = new WeakEventManager<string>();
        readonly WeakEventManager<bool> _networkConnectionEventManager = new WeakEventManager<bool>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="application"></param>
        /// <param name="appVersion">Must be in format Major.Minor.Revision</param>
        /// <param name="updateChecks">How often in days should we check for updates</param>
        public ApplicationManager(Xamarin.Forms.Application application, string appVersion, int updateChecks)
        {
            Core.Check.NotNull(application, nameof(application));
            _application = application;
            Version = appVersion;
            UpdateChecks = updateChecks;
        }

        public  async Task Initialize()
        {
            this.RegisterServicesAndProviders();
            var firstUsageTask = FileSystemManager.ReadLocalSettingAsync<bool>(ApplicationManager.ISFIRSTUSAGE);

            _connected = Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet ||
                Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.ConstrainedInternet;
            Xamarin.Essentials.Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            _isFirstUsage = await firstUsageTask;
            Updater updater = null;


            if (_isFirstUsage == true)
            {
                ConfigureFirstTimeUsage();
            }
            else if(! _isFirstUsage && _connected)
            {
                //get updateInfo and  then ask user to update
                updater = new Updater(this.AppUpdateUrl, this.UpdateChecks);
                await updater.CheckforUpdate();
                _updateModel = await updater.GetInfo();
            }


            if(_persistAuthenticationAcrossReload)
            {
                //load authModel
                var authModelTaskConfigured = FileSystemManager.ReadLocalSettingAsync<AuthModel>(LASTAPPUSAGE).ConfigureAwait(true);
                var authModelTaskAwaiter = authModelTaskConfigured.GetAwaiter();
                var authModel = authModelTaskAwaiter.GetResult();

                //if null authenticated is false.
                var isAuthUsageWindowValid = IsAuthUsageWindowValid(authModel);

                _authModel = isAuthUsageWindowValid ? authModel : null;
                MonitorAuthentionHeartbeat();
            }
        }

        private static void ConfigureFirstTimeUsage()
        {
            var resultFirstUsage = Convert.ToString(false);
            var configuredUsage = FileSystemManager.WriteLocalSetting(ApplicationManager.ISFIRSTUSAGE, resultFirstUsage).ConfigureAwait(true);
            var awaiterUsage = configuredUsage.GetAwaiter();
            awaiterUsage.GetResult();

            //get default device language
            var local = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var settings = DependencyService.Get<ISettingsService>();
            var settingsModel = settings.GetSettings();
            var language = SettingsModel.Languages.FirstOrDefault(lang => local.StartsWith(lang.ShortName, StringComparison.InvariantCultureIgnoreCase));

            if(language == null)
            {
                language = SettingsModel.Languages.First(lang => lang.ShortName.StartsWith("en", StringComparison.InvariantCultureIgnoreCase));
            }

            settingsModel.Language = language;
            settings.SetSettings(settingsModel);
        }

        private void Connectivity_ConnectivityChanged(object sender, Xamarin.Essentials.ConnectivityChangedEventArgs e)
        {
            var connectivity = e.NetworkAccess;

            _connected = connectivity == Xamarin.Essentials.NetworkAccess.Internet ||
                 connectivity == Xamarin.Essentials.NetworkAccess.ConstrainedInternet;

            OnNetworkConnectionChanged(_connected);
        }

        private void RegisterServicesAndProviders()
#pragma warning restore CC0091 // Use static method
        {

            //DependencyService.Register<MockDataStore>();
            DependencyService.Register<ILoggingService, DebugLoggingService>();

            DependencyService.Register<ILoggerFactory, Infrastructure.LoggerFactory>();
            DependencyService.Register<ISettingsService, SettingsService>();
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

        public event EventHandler<bool> NetworkConnectionChanged
        {
            add => _networkConnectionEventManager.AddEventHandler(value);
            remove => _networkConnectionEventManager.RemoveEventHandler(value);
        }




        void OnLoadingStateChanged(bool isLoading)
        {
            _loading = isLoading;
            _loadingStateChangedEventManager.RaiseEvent(this, IsLoading, nameof(LoadingStateChanged));
        }
        void OnNetworkConnectionChanged(bool isConnected)
        {
            _networkConnectionEventManager.RaiseEvent(this, isConnected, nameof(NetworkConnectionChanged));
        }

        /// <summary>
        /// When logging out, call this method passing in null
        /// </summary>
        /// <param name="model"></param>
        void OnAuthenticationStateChanged(AuthModel model)
        {
            _authModel = model;
            MonitorAuthentionHeartbeat();
            _authenticationStateChangedEventManager.RaiseEvent(this, IsAuthenticated, nameof(AuthenticationStateChanged));
        }

        public void PostMessage(string message)
        {
            _messageNotificationEventManager.RaiseEvent(this, message, nameof(NotificationMessageArrived));
        }

        public void AppUsed()
        {
            if (!IsAuthenticated) return;

            this._authModel.LastTimeUsage = DateTime.Now;

            if (!PersistAuthenticationAcrossReload) return;

            var configuredUsage = FileSystemManager.WriteLocalSetting(ApplicationManager.LASTAPPUSAGE, this._authModel).ConfigureAwait(true);
            var awaiterUsage = configuredUsage.GetAwaiter();
            awaiterUsage.GetResult();
        }

        /// <summary>
        /// present info to user.dialog : updates available, would you like to update ?? prior to calling this method
        /// </summary>
        /// <returns></returns>
        public async Task RequestApplicationUpdate()
        {
            //present info to user.
            //xxx
            // dialog : updates available, would you like to update ??
            var updater = new Updater(this.AppUpdateUrl, this.UpdateChecks);
            await updater.PerformUpdate();
        }

        public bool IsLoading => _loading;
        public bool IsConnected => _connected;

        public bool IsAuthenticated
        {
            get
            {
                if (_authModel == null)
                    return false;

                return true;
            }
        }

        public bool IsFirstUsage => _isFirstUsage;


        /// <summary>
        ///  Should be set  when the app first load in App constructor before calling ApplicationManager.Initialize();
        /// </summary>
        public bool PersistAuthenticationAcrossReload
        {
            get => _persistAuthenticationAcrossReload;
            set => _persistAuthenticationAcrossReload = value;
        }

        /// <summary>
        ///  Should be set  when the app first load in App constructor before calling ApplicationManager.Initialize();
        /// </summary>
        public string AppUpdateUrl
        {
            get => _appUpdateUrl;
            set => _appUpdateUrl = value;
        }

        public bool UpdatesAvailble { get => _updateModel != null; }

        public Xamarin.Forms.Application Application => _application;

        public string Version
        {
            get
            {
                return _appVersion;
            }
            set
            {
                _appVersion = value;
                Preferences.Set(APPVERSION, _appVersion);
            }
        }

        public int UpdateChecks { get; private set; }


        private void MonitorAuthentionHeartbeat()
        {
            if (PersistAuthenticationAcrossReload == false) return;

            if(!IsAuthenticated)
            {
                //stop timer
                _authContinueRunningTimer = false;

                //set to null in settings
                var tsk = FileSystemManager.WriteLocalSetting<AuthModel>(ApplicationManager.LASTAPPUSAGE, null);

                tsk.ConfigureAwait(true).GetAwaiter().GetResult();
                return;
            }

            //if authenticated is true.
            _authContinueRunningTimer = true;

            //if timer already started don't start.
            if (_authTimerRunning) return;

            //start timer.
            Device.StartTimer(TimeSpan.FromMinutes(3), MonitorAuthenticationHeartbeatCallback);

        }

        private bool MonitorAuthenticationHeartbeatCallback()
        {
            if (!IsAuthenticated || _authContinueRunningTimer == false)
            {
                MonitorAuthentionHeartbeat();
                _authContinueRunningTimer = false;
                _authTimerRunning = false;
                return _authContinueRunningTimer;
            }


            _authContinueRunningTimer = true;
            _authTimerRunning = true;

            var usageWindowValid = IsAuthUsageWindowValid(_authModel);

            if (usageWindowValid) return _authContinueRunningTimer;

            OnAuthenticationStateChanged(null);
            _authContinueRunningTimer = false;
            return _authContinueRunningTimer;
        }

        private bool IsAuthUsageWindowValid(AuthModel model)
        {
            if (model == null || model.LastTimeUsage == null) return false;

            var currentTime = DateTime.Now;
            var timeSinceLastUsage = model.LastTimeUsage;
            var lastUsageWindow = currentTime - timeSinceLastUsage;
            var usageWindowValid = lastUsageWindow < TimeSpan.FromMinutes(_allowDuration);
            return usageWindowValid;
        }
    }
}
