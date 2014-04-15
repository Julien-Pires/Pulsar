using System.Collections.Generic;

namespace Pulsar.Graphics
{
    internal sealed class RenderQueueElementComparer : IComparer<RenderQueueElement>
    {
        #region Methods

        public int Compare(RenderQueueElement x, RenderQueueElement y)
        {
            if (x.Key.Id > y.Key.Id)
                return 1;

            return (x.Key.Id == y.Key.Id) ? 0 : -1;
        }

        #endregion
    }
}
