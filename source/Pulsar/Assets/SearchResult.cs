using System;

namespace Pulsar.Assets
{
    internal struct SearchResult
    {
        #region Properties

        public SearchState State { get; set; }

        public Exception Error { get; set; }

        #endregion
    }
}
