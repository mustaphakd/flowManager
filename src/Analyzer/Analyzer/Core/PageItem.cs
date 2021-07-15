using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Core
{
    public class PageItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }
}
