using System;
using System.Threading.Tasks;

namespace Analyzer.UI
{
    public interface ILoginView
    {
        void SetLoginCompleted(Func<bool, Task> successCallback, Func<Task> failureCallback);
    }
}
