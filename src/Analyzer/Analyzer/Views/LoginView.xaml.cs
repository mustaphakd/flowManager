using Analyzer.Framework;
using Analyzer.UI;
using Analyzer.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace Analyzer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : BaseContentPage<LoginViewModel>, ILoginView
    {
        public LoginView()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }

        public void SetLoginCompleted(Func<bool, Task> successCallback, Func<Task> failureCallback)
        {
            Core.Check.NotNull(successCallback, nameof(successCallback));
            Core.Check.NotNull(failureCallback, nameof(failureCallback));

            ViewModel.SuccessCallback = successCallback;
            ViewModel.FailureCallback = failureCallback;
        }
    }
}