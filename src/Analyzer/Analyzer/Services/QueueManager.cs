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
        private List<QueueJob> pendingTasks;
        private WeakReference<Task> monitoringTask_;

        private QueueManager()
        {
            pendingTasks = new List<QueueJob>();
        }

        static QueueManager()
        {
            manager_ = manager_ ?? new QueueManager();
        }

        internal static void Add<T>(Task task, string successMessage, string failureMessage, TaskCompletionSource<T> source, Action handler = null)
        {
            //todo: test to make source is not corrupt and houses correct data and does not throw exception
            manager_.QueueJob(
                new QueueJob(task, successMessage, failureMessage, (TaskCompletionSource<object>)(object)source, handler));

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

                        if (job.Task.IsCompleted)
                            this.OnTaskComplete(job);
                        else
                            this.OnTaskFailure(job);
                    }

                    await Task.Delay(700);
                }

                System.Threading.Interlocked.Exchange(ref manager_.monitoring_, 0);
            }));

        }

        private void OnTaskComplete(QueueJob job)
        {
            ((Analyzer.App)App.Current).Manager.PostMessage(job.SuccessMessage);
            job.TaskCompletion.SetResult(new object());

            if(job.Handler != null)
            {
                job.Handler();
            }
        }
        private void OnTaskFailure(QueueJob job)
        {
            ((Analyzer.App)App.Current).Manager.PostMessage(job.FailureMessage);

            if(job.Task.IsCanceled == true)
            {
                job.TaskCompletion.SetCanceled();
            }
            else
            {
                job.TaskCompletion.SetException(job.Task.Exception);
            }
        }

        private void QueueJob(QueueJob queueJob)
        {
            lock(syncObj)
            {
                pendingTasks.Add(queueJob);
            }
        }
    }

    public class QueueJob
    {
        public QueueJob(
            Task task,
            string successMessage,
            string failureMessage,
            TaskCompletionSource<object> taskCompletion,
            Action handler)
        {
            Task = task;
            SuccessMessage = successMessage;
            FailureMessage = failureMessage;
            TaskCompletion = taskCompletion;
            Handler = handler;
        }

        public Task Task { get; }
        public string SuccessMessage { get; }
        public string FailureMessage { get; }
        public TaskCompletionSource<object> TaskCompletion { get; }
        public Action Handler { get; }
    }
}
