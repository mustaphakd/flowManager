using Xamarin.Forms;

namespace Analyzer.Framework
{
    public class CreateEditViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EditMode { get; set; }
        public DataTemplate ViewMode { get; set; }
        public DataTemplate CreateMode { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var state = ((IViewModelState)item).State;

            switch (state)
            {
                case Core.ViewStates.Deleting:
                    return null;
                case Core.ViewStates.Creating:
                case Core.ViewStates.UpdatingCreate:
                    return CreateMode;
                case Core.ViewStates.Updating:
                    return EditMode;
                case Core.ViewStates.UpdatingViewing:
                case Core.ViewStates.CreatingViewing:
                case Core.ViewStates.Viewing:
                default:
                    return ViewMode;
            }
        }
    }
}
