using System.Collections.Generic;

namespace Pulsar.Graphics
{
    internal sealed class RenderQueueElementComparer : IComparer<RenderQueueElement>
    {
        #region Methods

        public int Compare(RenderQueueElement x, RenderQueueElement y)
        {
            if (x.Key > y.Key)
                return 1;

            return (x.Key == y.Key) ? 0 : -1;
        }

        #endregion
    }
}
