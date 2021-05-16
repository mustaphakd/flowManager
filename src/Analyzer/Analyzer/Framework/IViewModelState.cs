using Analyzer.Core;

namespace Analyzer.Framework
{
    public interface IViewModelState
    {
        ViewStates State{ get; set; }
    }
}
