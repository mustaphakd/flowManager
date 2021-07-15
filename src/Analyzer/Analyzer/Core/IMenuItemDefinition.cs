using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Core
{
    public interface IMenuItemDefinition
    {
        int Order { get; }
        string NameTranslationKey { get; }
        string DescriptionTranslationKey { get; }

        string Image { get; }

        Type ViewType { get; }
    }
}
