using Xamarin.Forms;

namespace Analyzer.Framework
{
    public abstract class BaseContentPage<T> : ContentPage
        where T : BaseViewModel
    {
        private bool isAlreadyInitialized;
        private bool isAlreadyUninitialized;

        protected virtual T ViewModel => BindingContext as T;

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //NavigationProxy.Inner = App.NavigationRoot?.NavigationProxy;

            if (!isAlreadyInitialized)
            {
                await ViewModel.InitializeAsync();
                isAlreadyInitialized = true;
                await ViewModel.RenderedAsync();
            }
            else
            {
                await ViewModel.RenderedAsync();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (!isAlreadyUninitialized)
            {
                ViewModel.UninitializeAsync();
                isAlreadyUninitialized = true;
            }
        }
    }
}
