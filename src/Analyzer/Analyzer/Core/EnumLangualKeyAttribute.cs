using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Core
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class EnumLangualKeyAttribute: Attribute
    {
        public EnumLangualKeyAttribute(string name)
        {
            LanguageKey = name;
        }

        public string LanguageKey { get; }



    }
}
