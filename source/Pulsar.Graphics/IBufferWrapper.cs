using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Describes a wrapper that encapsulate a hardware level buffer
    /// </summary>
    public interface IBufferWrapper : IDisposable
    {
        #region Methods

        /// <summary>
        /// Gets elements in the buffer
        /// </summary>
        /// <typeparam name="T">Type of data stored in the wrapper</typeparam>
        /// <param name="bufferOffset">Offset in the buffer to the data</param>
        /// <param name="data">Array in which to store data</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to get</param>
        void GetData<T>(int bufferOffset, T[] data, int startIndex, int elementCount) where T : struct;

        /// <summary>
        /// Set data for this wrapper
        /// </summary>
        /// <typeparam name="T">Type of data stored in the wrapper</typeparam>
        /// <param name="bufferOffset">Offset from where data copy start</param>
        /// <param name="data">Data to set</param>
        /// <param name="startIndex">Starting index in data</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        void SetData<T>(int bufferOffset, T[] data, int startIndex, int elementCount, SetDataOptions option)
            where T : struct;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements in the buffer
        /// </summary>
        int ElementCount { get; }

        #endregion
    }

    /// <summary>
    /// Describes a wrapper that encapsulate an index buffer
    /// </summary>
    public interface IIndexBufferWrapper : IBufferWrapper
    {
        #region Properties

        /// <summary>
        /// Gets the underlying index buffer
        /// </summary>
        IndexBuffer IndexBuffer { get; }

        #endregion
    }

    /// <summary>
    /// Describes a wrapper that encapsulate a vertex buffer
    /// </summary>
    public interface IVertexBufferWrapper : IBufferWrapper
    {
        #region Properties

        /// <summary>
        /// Gets the underlying vertex buffer
        /// </summary>
        VertexBuffer VertexBuffer { get; }

        #endregion
    }
}
