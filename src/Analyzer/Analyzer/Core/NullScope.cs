using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Core
{
    /// <summary>
    /// Required used by our logger
    /// </summary>
    internal class NullScope : IDisposable
    {
        public static NullScope Instance
        {
            get;
        } = new NullScope();


        private NullScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
