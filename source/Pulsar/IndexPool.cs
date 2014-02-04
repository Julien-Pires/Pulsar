using System.Collections.Generic;

namespace Pulsar
{
    /// <summary>
    /// Manages a pool of index
    /// </summary>
    public sealed class IndexPool
    {
        #region Fields

        private readonly List<int> _indexes = new List<int>();
        private int _nextIndex;

        #endregion

        #region Methods

        /// <summary>
        /// Resets the pool
        /// </summary>
        public void Reset()
        {
            _indexes.Clear();
            _nextIndex = 0;
        }

        /// <summary>
        /// Gets an available index
        /// </summary>
        /// <returns>Returns a based-zero index</returns>
        public int Get()
        {
            if (_indexes.Count > 0)
                return _indexes[_indexes.Count - 1];

            return _nextIndex++;
        }

        /// <summary>
        /// Releases an index
        /// </summary>
        /// <param name="index">Index</param>
        public void Release(int index)
        {
            _indexes.Add(index);
        }

        #endregion
    }
}
