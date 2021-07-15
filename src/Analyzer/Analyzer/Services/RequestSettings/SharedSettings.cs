using Refit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer.Services.RequestSettings
{
    public static class SharedSettings
    {

        static SharedSettings()
        {
            RefitSettings = new RefitSettings();
        }

        public static RefitSettings RefitSettings
        {
            get
            {
                return Worosoft.Xamarin.HttpClientExtensions.Configuration.RefitSettings;
            }
            private set
            {
                Worosoft.Xamarin.HttpClientExtensions.Configuration.RefitSettings = value;
            }
        }

        public static Task<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<TResult>> Execute<T1, TResult>(Func<T1, Task<TResult>> handler, string successMessage = "Succeeded making remote API call", string failureMessage = "Failed making remote API call")
        {
            // get singleInstance of httpCLient via injected facory
            // when constructing httpClient pass in list of MessageProcessingHandler
            // -- auth, logging, RetryHandler, stopWatchHandler etc...
            //pass httpClient to .For<T1> method
            var taskSource = new TaskCompletionSource<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<TResult>>(); // todo: does accessing its task property obj raise an access violation ??
            var instance = Worosoft.Xamarin.HttpClientExtensions.Builder.Build<T1>(RefitSettings);


            QueueManager.Add<TResult>(Task.Run(async () =>
            {
                var part = await handler(instance).ConfigureAwait(false);
                return part;

            }), successMessage, failureMessage, taskSource);

            Task<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<TResult>> result = taskSource.Task;    //handler(concrete); // move task construction to QueueManager Add method and return task from that method.
            return result; // here return task of operationResult
        }

    }
}
