//using Data.Impl;
using Analyzer.Core;
using Analyzer.Infrastructure;
using Analyzer.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Analyzer.Services.Impl
{
    public class DataStoreService: IDataStoreService
    {
        private double _initialized = 0;
        private Object _sync = new object();
        private ILoggingService logger;
        public const string LoggingPrefix = "Mobile.Services.Impl.DataStoreService::";

       // private IEnumerable<Contact> _contacts;
        private Dictionary<string, List<Object>> _tempCache;
        public DataStoreService()
        {
            _tempCache = new Dictionary<string, List<object>>();
            var logger = Xamarin.Forms.DependencyService.Get<ILoggingService>();
            this.logger = logger;
            this.logger.Debug($"${LoggingPrefix}ctr() - Start");
            var settingsService = Xamarin.Forms.DependencyService.Get<ISettingsService>();
            var loggerFactory = Xamarin.Forms.DependencyService.Get<ILoggerFactory>();

            InitializeOperationService(settingsService, loggerFactory);
            this.logger.Debug($"${LoggingPrefix}ctr() - End");
        }

        public DataStoreService(ILoggingService loggingService, ISettingsService settingsService,
            ILoggerFactory loggerFactory)
        {
            if(loggingService == null) throw new ArgumentNullException(nameof(loggingService));
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            _tempCache = new Dictionary<string, List<object>>();
            this.logger = loggingService;
            this.logger.Debug($"${LoggingPrefix}ctr() - Start");
            InitializeOperationService(settingsService, loggerFactory);
            this.logger.Debug($"${LoggingPrefix}ctr() - End");
        }
        private void InitializeOperationService(ISettingsService settingsService, ILoggerFactory loggerFactory)
        {
            this.logger.Debug($"${LoggingPrefix}InitializeOperationService() - Start");
            this.logger.Debug($"${LoggingPrefix}InitializeOperationService() - settingsService : {settingsService}");
            this.logger.Debug($"${LoggingPrefix}InitializeOperationService() - loggerFactory: {loggerFactory}");

            var settings = settingsService.GetSettings();
            var storagePath = Framework.Files.FileSystemManager.ConstructFullPath("str", "db");
            var defaultConnectionString = Framework.Files.FileSystemManager.Combine(storagePath, settings.DefaultConnectionString);


            this.logger.Debug($"${LoggingPrefix}InitializeOperationService() - storagePath: {storagePath}");

            this.logger.Debug($"${LoggingPrefix}InitializeOperationService() - defaultConnectionString: {defaultConnectionString}");

            Framework.Files.FileSystemManager.EnsureDirectoryCreated(storagePath);
            Framework.Files.FileSystemManager.EnsureFileCreated(defaultConnectionString);


            this.logger.Debug($"${LoggingPrefix}InitializeOperationService() - will now try to instantiate OperationService");

            try
            {
               // OperationService = new DataOperationsService(defaultConnectionString, loggerFactory);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex);
                File.Delete(defaultConnectionString);
                //OperationService = new DataOperationsService(defaultConnectionString, loggerFactory);
            }
            this.logger.Debug($"${LoggingPrefix}InitializeOperationService() - End");
        }

        private void ProcessInitialization()
        {
            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Start - _initialized: ${_initialized}");
            if (this._initialized == 1L)
                return;

            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Load storage .....");
/*
            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Loading tracked devices");
            var trackedDevicesResults = OperationService.UpdateDevicesTracking(null);
            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Loading tracked devices results successfull? ${trackedDevicesResults.IsSuccess}");
            _trackedDevices = trackedDevicesResults.Results;
            this.logger.Debug(_trackedDevices);

            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Loading products");
            var productResults = OperationService.UpdateProducts(null);
            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Loading products results successfull? ${productResults.IsSuccess}");
            _products = productResults.Results;
            this.logger.Debug(_products); */

            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - process storage content");


            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Done initializing dataStore");
            Interlocked.Exchange(ref _initialized, 1);
            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - End");
        }
/*
        public IEnumerable<Contact> LoadContacts()
        {
            if (_contacts != null)
            {
                return _contacts;
            }

            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Loading contacts");
            var contactsResults = OperationService.UpdateContacts(null);
            this.logger.Debug($"${LoggingPrefix}ProcessInitialization() - Loading contacts results successfull? ${contactsResults.IsSuccess}");
            _contacts = contactsResults.Results;
            this.logger.Debug(_contacts);

            return _contacts;
        }*/

        private void Init()
        {
            this.logger.Debug($"${LoggingPrefix}Init() - Start - initialized: ${_initialized}");
            if (this._initialized == 1)
                return;

            lock (_sync)
            {
                ProcessInitialization();
            }

            this.logger.Debug($"${LoggingPrefix}Init() - End - initialized: ${_initialized}");
        }

        public void SaveTemporarily<T>(T value) where T: class, IModelDefinition
        {
            this.logger.Debug($"${LoggingPrefix}SaveTemporarily() - Start- value: ");
            this.logger.Debug(value);
            var typeName = typeof(T).Name.ToLowerInvariant();

            if (! _tempCache.ContainsKey(typeName))
            {
                _tempCache.Add(typeName, new List<Object>());
            }

            var list = _tempCache[typeName];
            ClearTemporaryCache(value);

            list.Add(value);
            this.logger.Debug($"${LoggingPrefix}SaveTemporarily() - End");
        }

        public void ClearTemporaryCache<T>(T value) where T : class, IModelDefinition
        {
            this.logger.Debug($"${LoggingPrefix}ClearTemporaryCache() - Start");
            var typeName = typeof(T).Name.ToLowerInvariant();

            if (!_tempCache.ContainsKey(typeName))
            {
                return;
            }

            var list = _tempCache[typeName];

            if (value == null)
            {
                list.Clear();
                return;
            }

            list.RemoveAll(item => {
                T convertedItem = item as T;
                var same = String.Equals(value.Id, convertedItem.Id);
                this.logger.Debug($"${LoggingPrefix}ClearTemporaryCache() - Comparing {value.Id} vs {convertedItem.Id} = {same}");
                return same;
            });

            this.logger.Debug($"${LoggingPrefix}ClearTemporaryCache() - End");
        }

        private IEnumerable<T> GetSavedTemporaries<T>()
        {
            this.logger.Debug($"${LoggingPrefix}GetSavedTemporaries() - Start");
            var list = new List<T>();
            var typeName = typeof(T).Name.ToLowerInvariant();

            if (! _tempCache.TryGetValue(typeName, out List<Object> objList))
            {
                return list;
            }

            foreach(var obj in objList)
            {
                list.Add((T)obj);
            }

            this.logger.Debug($"${LoggingPrefix}GetSavedTemporaries() - End");

            return list;
        }

        public OperationResult<IEnumerable<T>> SaveModels<T>(IEnumerable<Tuple<T, OperationKinds>> modelOperations) where T:  IModelDefinition
        {
            this.logger.Debug($"${LoggingPrefix}SaveModels() - Start - type: {nameof(T)} modelOperations: {modelOperations}");
            var actionableOperations = new List<ActionableOperations<T>>();
            OperationResult<IEnumerable<T>> result = null;

            foreach (var modelOperation in modelOperations)
            {
                var actionableOperation = new ActionableOperations<T>(modelOperation.Item1, modelOperation.Item2);
                actionableOperations.Add(actionableOperation);
                this.logger.Debug($"${LoggingPrefix}SaveModels() - added to queue: {actionableOperation}");
            }


            this.logger.Debug($"${LoggingPrefix}SaveModels() - Attempt to find proper store");

           /* if (EqualityComparer<Type>.Default.Equals(typeof(T), typeof(DeviceTracking)))
            {
                this.logger.Debug($"${LoggingPrefix}SaveModels() - Found store locator for type.....");
                var deviceTrackingActionableActions = actionableOperations.Cast<ActionableOperations<DeviceTracking>>();

                try
                {
                    this.logger.Debug($"${LoggingPrefix}SaveModels() - deviceTrackingActionableActions: {deviceTrackingActionableActions}");
                    var response = OperationService.UpdateDevicesTracking(deviceTrackingActionableActions);
                    this.logger.Debug($"${LoggingPrefix}SaveModels() - response: {response}");
                    result =  new OperationResult<IEnumerable<T>>( response.Results.Cast<T>(), response.ErrorMessage);
                }
                catch(Exception ex)
                {
                    this.logger.Debug($"${LoggingPrefix}SaveModels() - Exception thrown saving deviceTrackings:  ${ex}");
                    result = new OperationResult<IEnumerable<T>>(null, $"${LoggingPrefix}SaveModels() - Exception thrown saving deviceTrackings:  ${ex}");
                }
            } */
            /*else if (EqualityComparer<Type>.Default.Equals(typeof(T), typeof(Product)))
            {
                var productActionableActions = actionableOperations.Cast<ActionableOperations<Product>>();
                try
                {
                    var response = OperationService.UpdateProducts(productActionableActions);
                    result = new OperationResult<IEnumerable<T>>(response.Results.Cast<T>(), response.ErrorMessage);
                }
                catch (Exception ex)
                {
                    this.logger.Debug($"${LoggingPrefix}SaveModels() - Exception thrown saving products:  ${ex}");
                    result = new OperationResult<IEnumerable<T>>(null, $"${LoggingPrefix}SaveModels() - Exception thrown saving products:  ${ex}");
                }
            }
            else
            {
                throw new InvalidOperationException("Can't save operation having types of " + typeof(T));
            }

            this.logger.Debug(result);*/
            this.logger.Debug($"${LoggingPrefix}SaveModels() -  End - result: {result}");
            return result;
        }


        /*public IEnumerable<DeviceTracking> LoadTrackedDevices()
        {
            this.logger.Debug($"${LoggingPrefix}LoadTrackedDevices() - Start");
            _trackedDevices = OperationService.UpdateDevicesTracking(null).Results;
            this.logger.Debug(_trackedDevices);
            this.logger.Debug($"${LoggingPrefix}LoadTrackedDevices() - Loading from cache");
            var cache = GetSavedTemporaries<DeviceTracking>();
            this.logger.Debug($"${LoggingPrefix}LoadTrackedDevices() - End");
            return _trackedDevices.Union(cache).ToList();
        }*/

        public IEnumerable<T> LoadModels<T>()
        {
            throw new NotImplementedException();
        }

        public IOperationsService OperationService { get; protected set; }
    }
}
