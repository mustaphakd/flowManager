using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Core
{
    public enum ViewStates
    {
        Viewing, // default for existing elements
        Creating,
        Deleting,
        Saving,
        Updating,
        Canceling,
        Searching,
        Pending,
        UpdatingCreate,
        UpdatingViewing,
        CreatingViewing
    }
}
