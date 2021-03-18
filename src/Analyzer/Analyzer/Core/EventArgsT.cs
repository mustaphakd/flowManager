using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Core
{
    public class EventArgsT<T> : EventArgs
    {
        public T Value { get; }

#pragma warning disable CC0057 // Unused parameters
        public EventArgsT(T val) => Value = val;
#pragma warning restore CC0057 // Unused parameters
    }
}