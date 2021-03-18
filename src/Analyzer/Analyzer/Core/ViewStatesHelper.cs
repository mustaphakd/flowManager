using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Core
{
    public static class ViewStatesHelper
    {
        public static ViewStates MoveToState(ViewStates current, ViewStates next, ViewStates original = ViewStates.Viewing)
        {
            switch(next)
            {
                case ViewStates.Canceling:
                case ViewStates.Creating:
                case ViewStates.Deleting:
                case ViewStates.Pending:
                case ViewStates.Searching:
                case ViewStates.Saving:
                    return next;
                case ViewStates.Viewing:
                    if(current == ViewStates.Updating)
                    {
                        return ViewStates.UpdatingViewing;
                    }

                    if (current == ViewStates.Creating || current == ViewStates.UpdatingCreate)
                    {
                        return ViewStates.CreatingViewing;
                    }

                    return next;
                case ViewStates.Updating:
                    if ((current == ViewStates.Viewing  && original != ViewStates.Creating) ||
                        current == ViewStates.UpdatingViewing)
                        return ViewStates.Updating;
                    if (current == ViewStates.Creating || current == ViewStates.CreatingViewing)
                        return ViewStates.UpdatingCreate;

                    return ViewStates.Updating;
                default:
                    throw new Exception("Transitioning to view State not supported!"); //todo: translate
            }
        }
    }
}
