using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public class QueueManager
    {
        private int monitoring_ = 0;
        private static QueueManager manager_;
        private object syncObj = new object();
        private List<IQueueJob> pendingTasks;
        private WeakReference<Task> monitoringTask_;

        private QueueManager()
        {
            pendingTasks = new List<IQueueJob>();
        }

        static QueueManager()
        {
            manager_ = manager_ ?? new QueueManager();
        }

        internal static void Add<T>(Task<T> task, string successMessage, string failureMessage, TaskCompletionSource<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<T>> source, Action handler = null)
        {
            manager_.QueueJob(
                new QueueJob<T>(task, successMessage, failureMessage, source, handler));

            manager_.StartMonitoring();
        }

        private void StartMonitoring()
        {
            if (manager_.monitoring_ == 1) return;

            System.Threading.Interlocked.Exchange(ref manager_.monitoring_, 1);
            //inside monitoring handler pause for  a few seconds

            monitoringTask_ = new WeakReference<Task>(Task.Run(async () => {

                while(true)
                {
                    var pendingTaskCount = pendingTasks.Count;

                    if (pendingTaskCount < 1) break;

                    for (var i = pendingTaskCount - 1; i >= 0; i--)
                    {
                        var job = pendingTasks[i];

                        if (job.Task.IsCompleted == false && job.Task.IsFaulted == false && job.Task.IsCanceled == false) continue;

                        pendingTasks.RemoveAt(i);

                        if (job.Task.IsCompleted && job.Task.IsFaulted == false)
                            this.OnTaskComplete(job);
                        else
                            this.OnTaskFailure(job);
                    }

                    await Task.Delay(700);
                    System.Threading.Thread.Sleep(2000);
                }

                System.Threading.Interlocked.Exchange(ref manager_.monitoring_, 0);
            }));

        }

        private void OnTaskComplete(IQueueJob job)
        {
            ((Analyzer.App)App.Current).Manager.PostMessage(job.SuccessMessage);
            job.SetComplete();

            if(job.Handler != null)
            {
                job.Handler();
            }
        }
        private void OnTaskFailure(IQueueJob job)
        {
            ((Analyzer.App)App.Current).Manager.PostMessage(job.FailureMessage);

            if(job.Task.IsCanceled == true)
            {
                job.SetCancel();
            }
            else
            {
                job.SetException();
            }
        }

        private void QueueJob<T>(QueueJob<T> queueJob)
        {
            lock(syncObj)
            {
                pendingTasks.Add(queueJob);
            }
        }
    }

    public interface IQueueJob
    {
        Task Task { get; }
        string SuccessMessage { get; }
        string FailureMessage { get; }
        object TaskCompletion { get; }
        Action Handler { get; }

        void SetComplete();
        void SetCancel();
        void SetException();
    }

    public class QueueJob<T> : IQueueJob
    {
        public QueueJob(
            Task<T> task,
            string successMessage,
            string failureMessage,
            TaskCompletionSource<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<T>> taskCompletion,
            Action handler)
        {
            Task = task;
            SuccessMessage = successMessage;
            FailureMessage = failureMessage;
            TaskCompletion = taskCompletion;
            Handler = handler;
        }

        public Task<T> Task { get; }
        public string SuccessMessage { get; }
        public string FailureMessage { get; }
        public TaskCompletionSource<Worosoft.Xamarin.CommonTypes.Operations.OperationResult<T>> TaskCompletion { get; }
        public Action Handler { get; }

        Task IQueueJob.Task => Task;

        object IQueueJob.TaskCompletion => TaskCompletion;

        public void SetCancel()
        {
            TaskCompletion.SetCanceled();
        }

        public void SetComplete()
        {
            TaskCompletion.SetResult(Task.Result);
        }

        public void SetException()
        {
            TaskCompletion.SetException(Task.Exception);
        }
    }
}
