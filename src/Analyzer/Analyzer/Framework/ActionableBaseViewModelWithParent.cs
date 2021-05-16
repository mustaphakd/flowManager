using Analyzer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyzer.Framework
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T">Model type</typeparam>
    /// <typeparam name="P">Parent Type</typeparam>
    public abstract class ActionableBaseViewModelWithParent<VM, T, P>: ActionableBaseViewModel<VM, T>
        where VM : class
        where T : class, Core.IModelDefinition
        where P : class, Core.IModelDefinition, new()
    {
        private Func<IEnumerable<P>> _parentsRetriever;
        private P _emptyParent;
        private IEnumerable<P> _parents;
        public ActionableBaseViewModelWithParent(T item, Action refreshAction, ViewStates state, Func<IEnumerable<P>> parentRetriever) : base(item, refreshAction, state, true)
        {
            Core.CoreHelpers.ValidateDefined(parentRetriever, "Func<IEnumerable<T>> retriever is required");
            this._parents = new List<P>();
            this._parentsRetriever = parentRetriever;
            this._emptyParent = new P { Id = Guid.NewGuid().ToString()};
        }

        protected abstract P RetrieveParentParent(P item);
        protected abstract P RetrieveParent(T item);
        protected abstract void SetParent(T item, P parent);



        protected IEnumerable<P> CleanAccessibleCategories(IEnumerable<P> source)
        {
            Core.CoreHelpers.ValidateDefined(source, "source is required");

            var currentId = CurrentItem.Id;
            var excludeIds = FindDescandantInclusive(source, new[] { currentId });
            var notIncludingSelf = source.Where(item => !excludeIds.Contains(item.Id)).ToList();
            return notIncludingSelf;
        }

        protected override void OnStateUpdated(ViewStates state)
        {
            if (this._parentsRetriever == null) //this function may endups being called before instance is fully constructed
            {
                return;
            }

            var parent = this.RetrieveParent(this.CurrentItem);

            if (State == ViewStates.Creating ||
                State == ViewStates.UpdatingCreate ||
                State == ViewStates.UpdatingViewing ||
                State == ViewStates.Updating)
            {
                Parents = this._parentsRetriever();

            }

            if (parent != null && Parents.Count() > 0)
            {
                var itemParent = Parents.First(cat => cat.Id.Equals(parent.Id));
                this.SetParent(this.CurrentItem, itemParent);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        protected P EmptyParent => this._emptyParent;

        [System.Text.Json.Serialization.JsonIgnore]
        protected Func<IEnumerable<P>> ParentsRetriever => this._parentsRetriever;

        /// <summary>
        /// Value should be ignored when null
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<P> Parents
        {
            get
            {
                var parents = CleanAccessibleCategories(_parents);
                return parents;
            }
            set
            {
                var parent = this.RetrieveParent(this.CurrentItem);

                if ((value.Count() > 0) && _emptyParent != null && !value.Any(cat => cat.Id.Equals(_emptyParent.Id)))
                {
                    var newValues = new List<P>();
                    newValues.Add(_emptyParent);
                    newValues.AddRange(value);
                    SetAndRaisePropertyChanged(ref _parents, newValues);
                }
                else
                {
                    SetAndRaisePropertyChanged(ref _parents, value);
                }

                this.SetParent(this.CurrentItem,  parent);
            }
        }


        //todo: optimize with better algorithm
        private IEnumerable<string> FindDescandantInclusive(IEnumerable<P> source, IEnumerable<string> parentIds)
        {
            var inclusiveDescandants = parentIds.ToList();
            var newIds = new List<string>();

            foreach (var item in source)
            {
                var id = item.Id;
                P parent = this.RetrieveParentParent(item);

                if (inclusiveDescandants.Contains(id) || newIds.Contains(id) || parent == null)
                {
                    continue;
                }

                var parentId = parent.Id;
                Core.CoreHelpers.ValidateDefined(parentId, "parentId is missing from a parent.");

                if (!inclusiveDescandants.Contains(parentId))
                {
                    continue;
                }

                newIds.Add(id);
            }

            if (newIds.Count > 0)
            {
                newIds = FindDescandantInclusive(source, newIds).ToList();
                inclusiveDescandants.AddRange(newIds);
            }

            return inclusiveDescandants;
        }
    }
}
