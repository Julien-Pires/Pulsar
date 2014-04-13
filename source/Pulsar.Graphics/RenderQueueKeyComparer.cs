using System.Collections.Generic;

namespace Pulsar.Graphics
{
    public sealed class RenderQueueKeyComparer : IComparer<RenderQueueKey>
    {
        #region Methods

        public int Compare(RenderQueueKey x, RenderQueueKey y)
        {
            if (x.Id > y.Id)
                return 1;

            return (x.Id == y.Id) ? 0 : -1;
        }

        #endregion
    }
}
