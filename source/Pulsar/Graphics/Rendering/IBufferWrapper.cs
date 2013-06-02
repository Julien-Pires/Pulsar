using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    public interface IBufferWrapper : IDisposable
    {
        #region Methods

        void GetData<T>(T[] data) where T : struct;

        void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option) where T : struct;

        #endregion
    }

    public interface IIndexBufferWrapper : IBufferWrapper
    {
        #region Properties

        IndexBuffer Buffer { get; }

        #endregion
    }

    public interface IVertexBufferWrapper : IBufferWrapper
    {
        #region Properties

        VertexBuffer Buffer { get; }

        #endregion
    }
}
