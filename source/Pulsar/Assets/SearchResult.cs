using System;

namespace Pulsar.Assets
{
    /// <summary>
    /// Contains information about a search of an asset
    /// </summary>
    internal struct SearchResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the state of the search
        /// </summary>
        public SearchState State { get; set; }

        /// <summary>
        /// Gets or sets an error
        /// </summary>
        public Exception Error { get; set; }

        #endregion
    }
}
