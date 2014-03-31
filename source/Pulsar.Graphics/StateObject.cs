using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    internal sealed class StateObject<T> where T : GraphicsResource
    {
        #region Fields

        internal readonly ushort Id;
        internal readonly T State;

        #endregion

        #region Constructors

        internal StateObject(ushort id, T state)
        {
            Debug.Assert(state != null);

            Id = id;
            State = state;
        }

        #endregion
    }
}
