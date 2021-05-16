using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Analyzer.Framework
{
    public class AsyncCommand : Command
    {
        public AsyncCommand(Func<object, Task> execute)
            : base(param => execute?.Invoke(param).ConfigureAwait(false))
        {
        }

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
            : base(() => execute?.Invoke().ConfigureAwait(false), canExecute)
        {
        }
        public AsyncCommand(Func<Task> execute)
            : base(() => execute?.Invoke().ConfigureAwait(false))
        {
        }

        public AsyncCommand(Action<object> execute, Func<object, bool> canExecute)
            : base(execute, canExecute)
        {
        }

        public AsyncCommand(Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
        }
    }
}
