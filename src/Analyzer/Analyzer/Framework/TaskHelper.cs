namespace Analyzer.Framework
{
    using Analyzer.Infrastructure;
    using OperationResult;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using static OperationResult.Helpers;

    public class TaskHelper
    {
        private readonly IConnectivityService connectivityService;
        private readonly ILoggingService loggingService;

        private Action whenStarting;
        private Action whenFinished;

        private TaskHelper()
        {
            connectivityService = DependencyService.Get<IConnectivityService>();
            loggingService = DependencyService.Get<ILoggingService>();
        }

        public static TaskHelper Create()
        {
            return new TaskHelper();
        }

        public TaskHelper WhenStarting(Action action)
        {
            whenStarting = action;

            return this;
        }

        public TaskHelper WhenFinished(Action action)
        {
            whenFinished = action;

            return this;
        }

        /// <summary>
        /// When invoking remote services, wrap them into this call
        /// </summary>
        /// <param name="task"></param>
        /// <param name="customErrorHandler"></param>
        /// <returns></returns>
        public async Task<Status> TryWithErrorHandlingAsync(
            Task task,
            Func<Exception, Task<bool>> customErrorHandler = null)
        {
            var taskWrapper = new Func<Task<object>>(() => WrapTaskAsync(task));
            var result = await TryWithErrorHandlingAsync(taskWrapper(), customErrorHandler);

            if (result)
            {
                return Ok();
            }

            return Error();
        }

        /// <summary>
        /// When invoking remote services, wrap them into this call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="customErrorHandler"></param>
        /// <returns></returns>
        public async Task<Result<T>> TryWithErrorHandlingAsync<T>(
            Task<T> task,
            Func<Exception, Task<bool>> customErrorHandler = null)
        {
            whenStarting?.Invoke();

            if (!connectivityService.IsThereInternet)
            {
                loggingService?.Warning("There's no Internet access");
                return Error();
            }

            try
            {
                T actualResult = await task;
                return Ok(actualResult);
            }
            catch (HttpRequestException exception)
            {
                loggingService?.Warning($"{exception}");

                if (customErrorHandler == null || !await customErrorHandler?.Invoke(exception))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Unexpected Error", //Resources.Alert_Title_UnexpectedError,
                        "Internet Error", //Resources.Alert_Message_InternetError,
                        "OK..."); //Resources.Alert_OK_OKEllipsis);
                }
            }
            catch (TaskCanceledException exception)
            {
                loggingService?.Debug($"{exception}");
            }
            catch (Exception exception)
            {
                loggingService?.Error(exception);

                if (customErrorHandler == null || !await customErrorHandler?.Invoke(exception))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Unexpected Error", //Resources.Alert_Title_UnexpectedError,
                        "Internet Error", //Resources.Alert_Message_InternetError,
                        "OK..."); //Resources.Alert_OK_OKEllipsis);
                }
            }
            finally
            {
                whenFinished?.Invoke();
            }

            return Error();
        }

#pragma warning disable CC0091 // Use static method
        private async Task<object> WrapTaskAsync(Task innerTask)
#pragma warning restore CC0091 // Use static method
        {
            await innerTask;

            return new object();
        }
    }
}
