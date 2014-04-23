using System.Collections.Generic;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Provides mechanism to compare elements in a render queue
    /// </summary>
    internal sealed class RenderQueueElementComparer : IComparer<RenderQueueElement>
    {
        #region Methods

        /// <summary>
        /// Compares two element of a render queue
        /// </summary>
        /// <param name="x">First element</param>
        /// <param name="y">Second element</param>
        /// <returns>Returns zero if x equal y, 1 if x superior to y otherwise -1</returns>
        public int Compare(RenderQueueElement x, RenderQueueElement y)
        {
            if (x.Key > y.Key)
                return 1;

            return (x.Key == y.Key) ? 0 : -1;
        }

        #endregion
    }
}
