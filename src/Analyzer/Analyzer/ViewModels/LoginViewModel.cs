using Analyzer.Framework;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Analyzer.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        public ICommand LoginCommand => new AsyncCommand(this.LoginAsync, () => this.CanLoginExecute);

        private async Task LoginAsync()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                return;

            IsBusy = true;
            CanLoginExecute = false;

            await Task.Delay(1000);

            var credentials = new Services.UserLogin(UserName, Password);

            var loginResult = await TryExecuteWithLoadingIndicatorsAsync(
                () => LoginViewModel.AuthenticationService.LoginAsync(credentials),
                exception =>
                {
                    return Task.FromResult(false);
                });

            if(loginResult.IsError)
            {
                await FailureCallback();
                return;
            }

            await SuccessCallback(loginResult.IsSuccess);
        }

        public ICommand CancelCommand => new AsyncCommand(this.CancelAsync);

        private async Task CancelAsync()
        {
            await FailureCallback ();
            Xamarin.Forms.Application.Current.Quit();
            System.Environment.Exit(0);
        }


        private string userName_;
        private string password_;
        //[Section = 1, Position= 1, ColumnOrder = 1, ColumnSpan=[1-4], Label = [null - TranslationKey], UIHint = UIHint.[Image, Input, checkbox, radio, select, button]]
        public String UserName
        {
            get => userName_;
            set
            {
                SetAndRaisePropertyChangedIfDifferentValues(ref userName_, value);
                CanLoginExecute = !String.IsNullOrEmpty(userName_);
                RaisePropertyChanged(nameof(LoginCommand));
            }
        }
        public String Password
        {
            get => password_;
            set
            {
                SetAndRaisePropertyChangedIfDifferentValues(ref password_, value);
                RaisePropertyChanged(nameof(LoginCommand));
            }
        }


        public Func<bool, Task> SuccessCallback { get; set; }
        public Func<Task> FailureCallback { get; set; }


        private bool canLoginExecute_;

        public bool CanLoginExecute
        {
            get => canLoginExecute_;

            private set
            {
                if (value == canLoginExecute_) { return; }

                canLoginExecute_ = value;
                RaisePropertyChanged(nameof(CanLoginExecute));
            }
        }
    }
}
