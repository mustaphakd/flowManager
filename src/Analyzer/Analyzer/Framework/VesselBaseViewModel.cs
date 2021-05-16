using Analyzer.Core;
using Analyzer.Framework;
using Analyzer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Mobile.Framework
{
    public abstract class VesselBaseViewModel<AVM,  T> : BaseStateAwareViewModel<Analyzer.Core.ViewStates>
        where T : class, IModelDefinition
        where AVM: ActionableBaseViewModel<AVM, T>
    {
        public const string LoggingPrefix = "Mobile.Feature.Mobile.Framework.VesselBaseViewModel<AVM,  T>::";
        private Object syncObj = new Object();

        public VesselBaseViewModel()
        {

            LoggingService.Debug($"${LoggingPrefix}ctor() - ");
            ItemViewModels = new ObservableCollection<AVM>();
        }

        ObservableCollection<AVM> itemViewModels;
        public ObservableCollection<AVM> ItemViewModels
        {
            get
            {
                return itemViewModels;
            }
            set
            {
                this.SetAndRaisePropertyChanged(ref itemViewModels, value);
                this.RaisePropertyChanged(nameof(WrappedItemViewModels));
            }
        }

        public ObservableCollection<AVM> WrappedItemViewModels
        {
            get
            {
                return new ObservableCollection<AVM>(ItemViewModels.Where(vm => vm.State != ViewStates.Deleting).ToList());
            }
        }

        public ObservableCollection<AVM> SelectedItemViewModels
        {
            get
            {
                return new ObservableCollection<AVM>(itemViewModels.Where(vm => vm.IsSelected == true).ToList());
            }
        }
        public ICommand SaveCommand => new AsyncCommand(this.SaveAsync);
        public ICommand CancelCommand => new AsyncCommand(this.CancelAsync);

        public ICommand AddCommand => new Command(this.AddItemAsync);
        public ICommand DeleteCommand => new AsyncCommand(this.DeleteItemsAsync);

        private async Task SaveAsync()
        {
            LoggingService.Debug($"{LoggingPrefix}SaveAsync() - Start ");
            await this.OnSave();
            LoggingService.Debug($"{LoggingPrefix}SaveAsync() - End ");
        }

        protected virtual Task OnSave() {
            return Task.CompletedTask;
        }

        private async Task CancelAsync()
        {
            LoggingService.Debug($"{LoggingPrefix}CancelAsync() - Start ");
            LoggingService.Debug($"{LoggingPrefix}CancelAsync() - End ");
            await Task.CompletedTask;  // App.NavigateBackAsync();  // not regiested Xamarin.Forms.Shell.Current.GoToAsync(Core.Routes.Home, false);
        }
        private void AddItemAsync()
        {
            LoggingService.Debug($"{LoggingPrefix}AddItemAsync() - Start ");
             var viewModel = this.CreateNewViewModel();

            LoggingService.Debug($"{LoggingPrefix}AddItemAsync() - new viewModel: ");

            var serialized = System.Text.Json.JsonSerializer.Serialize(viewModel, new System.Text.Json.JsonSerializerOptions { });
            LoggingService.Debug(serialized);

            this.ItemViewModels.Add(viewModel);
            this.RaisePropertyChanged(nameof(WrappedItemViewModels));
            LoggingService.Debug($"{LoggingPrefix}AddItemAsync() - End ");
        }

        public abstract AVM CreateNewViewModel();

        protected void RefreshCollection()
        {
            LoggingService.Debug($"{LoggingPrefix}RefreshCollection() - Start ");
            this.RaisePropertyChanged(nameof(ItemViewModels));
            this.RaisePropertyChanged(nameof(SelectedItemViewModels));
            this.RaisePropertyChanged(nameof(WrappedItemViewModels));
            LoggingService.Debug($"{LoggingPrefix}RefreshCollection() - End ");
        }

        /// <summary>
        /// highly related to src\Ebtiq\Mobile\Mobile\Core\ViewStatesHelper.cs
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<AVM> getViewModelsWithUpdates()
        {
            LoggingService.Debug($"{LoggingPrefix}getViewModelsWithUpdates() - Start ");
            var requiredUpdates = this.ItemViewModels.Where(
                viewModel =>
                {
                    if (viewModel.State == ViewStates.Deleting && viewModel.InitialState == ViewStates.Creating)
                    {
                        return false;
                    }

                    if(viewModel.InitialState == ViewStates.Viewing && viewModel.State == ViewStates.Viewing)
                    {
                        return false;
                    }

                    if(viewModel.State == ViewStates.Canceling && viewModel.State == ViewStates.Pending
                        && viewModel.State == ViewStates.Saving && viewModel.State == ViewStates.Searching)
                    {
                        return false;
                    }

                    return true;
                 }).ToList();

            LoggingService.Debug($"{LoggingPrefix}getViewModelsWithUpdates() - End - requiredUpdates length: " + requiredUpdates.Count);
            return requiredUpdates;
        }

        private async Task DeleteItemsAsync()
        {
            LoggingService.Debug($"{LoggingPrefix}DeleteItemsAsync() - Start ");

            var selectedItemViewModelsList = SelectedItemViewModels.ToList();

            var deletionCount = selectedItemViewModelsList.Count;

            if (deletionCount < 1)
            {
                return;
            }

            var dialogService = Xamarin.Forms.DependencyService.Get<IDialogService>();
            var deleteStringContent = deletionCount > 1 ? $"{deletionCount}" : selectedItemViewModelsList.First().GetModelDisplayString(selectedItemViewModelsList.First().CurrentItem);
            var answer = await dialogService.ConfirmDeletion(deleteStringContent, deletionCount == 1);

            if (answer != true)
                return;

            await Task.Delay(100);

            lock (this.syncObj)
            {
                foreach (var viewModel in selectedItemViewModelsList)
                {
                    var state = viewModel.InitialState;
                    var newState = ViewStatesHelper.MoveToState(viewModel.State, ViewStates.Deleting);
                    LoggingService.Debug($"{LoggingPrefix}DeleteItemsAsync() - InitialState: {state}; newState: {newState}. ");
                    LoggingService.Debug($"{LoggingPrefix}DeleteItemsAsync() -  Removing from selectedItemViewModels");

                    if (state != ViewStates.Creating && state != ViewStates.UpdatingCreate)
                    {
                        LoggingService.Debug($"{LoggingPrefix}DeleteItemsAsync() - this is an existing item. continuing. ");
                        viewModel.IsSelected = false;
                        viewModel.State = newState;
                        continue;
                    }

                    LoggingService.Debug($"{LoggingPrefix}DeleteItemsAsync() - newly created item. can be easely discarded.. Removing from ItemViewModels. ");
                    ItemViewModels.Remove(viewModel);
                }

                LoggingService.Debug($"{LoggingPrefix}DeleteItemsAsync() - End ");
                this.RaisePropertyChanged(nameof(WrappedItemViewModels));
            }
        }
    }
}
