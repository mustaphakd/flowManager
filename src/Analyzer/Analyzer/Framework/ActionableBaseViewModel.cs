using Analyzer.Core;
using Analyzer.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Analyzer.Framework
{
    public abstract class ActionableBaseViewModel<VM,T> : BasePropertyChangedNotification, IEquatable<VM>, IViewModelState
        where VM: class
        where T : class, IModelDefinition
    {
        private ViewStates _initialState;
        private bool _isSelected;
        private ViewStates _state;
        private T _initialItem;
        private T _currentItem;
        private Action _refreshAction;

        public ActionableBaseViewModel(T item, Action refreshReference, ViewStates state = ViewStates.Viewing, bool externalCallRequiredBeforeStateUpdate = false)
        {
            // serialize and then deserialize into initial item.   for cancel all operation
            var serializedValue = System.Text.Json.JsonSerializer.Serialize(item);
            this._initialItem = System.Text.Json.JsonSerializer.Deserialize<T>(serializedValue);
            this._initialState = state;
            this.CurrentItem = item;

            if (string.IsNullOrEmpty(this.CurrentItem.Id))
            {
                this.CurrentItem.Id = Guid.NewGuid().ToString();
            }

            this.IsSelected = false;

            if( state == ViewStates.Creating && externalCallRequiredBeforeStateUpdate == false)
            {
                State = ViewStates.Creating;
            }

            this._refreshAction = refreshReference;
        }


        public Boolean IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                //this._isSelected = value;
                SetAndRaisePropertyChanged(ref _isSelected, value);
            }
        }
        public ViewStates State
        {
            get { return this._state; }
            set
            {
               // this._state = value;
                SetAndRaisePropertyChanged(ref _state, value);
                OnStateUpdated(_state);
            }
        }

        protected virtual void OnStateUpdated(ViewStates state) { }

        public ViewStates InitialState
        {
            get
            {
                return this._initialState;
            }
        }
        public T CurrentItem
        {
            get { return this._currentItem; }
            set { this.SetAndRaisePropertyChanged(ref this._currentItem, value); }
        }


        public ICommand EditCommand => new Command(this.EditAsync);

        private ViewStates _tempState ; // used exclusively by editAsync and CancelAsync to reset state to previous state.
        private void EditAsync(Object argument)
        {
            _tempState = this.State;
            this.State = ViewStatesHelper.MoveToState(this.State, ViewStates.Updating);

            if (this.InitialState != ViewStates.Creating)
            {
                this.RaisePropertyChanged(nameof(this.State));
            }

            this._refreshAction();
        }


        public ICommand DeleteCommand => new AsyncCommand(this.DeleteAsync);

        private async Task DeleteAsync(Object argument)
        {
            var canDeleteResult = this.CanDelete();

            if (canDeleteResult != null && canDeleteResult.Length > 0)
            {

                var dialogDeleteService = Xamarin.Forms.DependencyService.Get<IDialogService>();
                await dialogDeleteService.AlertInfo("Error", "DeleteRequirement", String.Join(",", canDeleteResult));
                return;
            }

            var dialogService = Xamarin.Forms.DependencyService.Get<IDialogService>();
            var answer = await dialogService.ConfirmDeletion(this.GetModelDisplayString(this.CurrentItem), true);

            if (answer != true)
                return;

            this.State = ViewStatesHelper.MoveToState(this.State, ViewStates.Deleting);
            this._refreshAction();
        }

        public ICommand CancelCommand => new Command(this.CancelAsync);

        private void CancelAsync(Object argument)
        {
            this.State =  _tempState;
            this._refreshAction();
        }


        public ICommand SaveCommand => new AsyncCommand(this.SaveAsync);

        private async Task SaveAsync(Object argument)
        {
            var canSaveResult = this.CanSave();

            if ( canSaveResult != null && canSaveResult.Length > 0 )
            {
                var translateExtension = new Analyzer.Extensions.TranslateExtension();
                translateExtension.Text = canSaveResult.Length == 1 ? "Is" : "Are";
                var verb = translateExtension.ProvideValue(null) as string;

                var dialogService = Xamarin.Forms.DependencyService.Get<IDialogService>();
                await dialogService.AlertInfo("Error", "SavingRequirement", String.Join(",", canSaveResult), verb);
                return;
            }

            this.State = ViewStatesHelper.MoveToState(this.State, ViewStates.Saving);
            var permanentlySaved = this.OnSaving();

            var storeService = Xamarin.Forms.DependencyService.Get<IDataStoreService>();
            storeService.SaveTemporarily<T>(CurrentItem);

            this.State = ViewStatesHelper.MoveToState(this.State, ViewStates.Viewing);

            if (this.InitialState == ViewStates.Viewing && permanentlySaved == false)
            {
                this._initialState = ViewStatesHelper.MoveToState(this.InitialState, ViewStates.Updating, this.InitialState);
            }

            RefreshParentContainer();
        }

        /// <summary>
        /// provide an opportunity to validate model before saving attempt.
        /// </summary>
        /// <returns></returns>
        public virtual string[] CanSave() { return null; }

        /// <summary>
        /// implement and return true if your viewModel will permanently persist its model.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnSaving() { return false; }
        public virtual string[] CanDelete() { return null; }


        public ICommand SelectCommand => new AsyncCommand(this.SelectAsync);

        private Task SelectAsync(Object argument)
        {
            return Task.CompletedTask;
        }

        public T GetInitialModel()
        {
            return this._initialItem;
        }

        /// <summary
        /// detects whether the viewmodel model should not be ignored when saving to disk. e.g the viewmodel represents model that happens to be created and then
        /// deleted. or an item was just viewed
        /// </summary>
        /// <returns></returns>
        public bool IsAccountableForBackendFlight()
        {
            return !(this.State == ViewStates.Viewing || this._initialState == ViewStates.Creating && this.State == ViewStates.Deleting);
        }

        public override int GetHashCode()
        {
            return this.CurrentItem.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as VM);
        }

        public bool Equals(VM other)
        {
            if (other == null)
                return false;

            return this.GetHashCode() == other.GetHashCode();
        }

        // eg: item.Name
        public abstract string GetModelDisplayString(T item);

        protected void RefreshParentContainer()
        {
            this._refreshAction();
        }
    }
}
