using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Describe a buffer wrapper
    /// </summary>
    public interface IBufferWrapper : IDisposable
    {
        #region Methods

        /// <summary>
        /// Get the data stored by this wrapper
        /// </summary>
        /// <typeparam name="T">Type of data stored in the wrapper</typeparam>
        /// <param name="data">Array in which to store data</param>
        void GetData<T>(T[] data) where T : struct;

        /// <summary>
        /// Set data for this wrapper
        /// </summary>
        /// <typeparam name="T">Type of data stored in the wrapper</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option) where T : struct;

        #endregion
    }

    public interface IIndexBufferWrapper : IBufferWrapper
    {
        #region Properties

        /// <summary>
        /// Get the underlying index buffer
        /// </summary>
        IndexBuffer Buffer { get; }

        #endregion
    }

    public interface IVertexBufferWrapper : IBufferWrapper
    {
        #region Properties

        /// <summary>
        /// Get the underlying vertex buffer
        /// </summary>
        VertexBuffer Buffer { get; }

        #endregion
    }
}
