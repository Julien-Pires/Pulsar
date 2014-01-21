using System;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Defines a wrapper for an Xna texture
    /// </summary>
    public interface ITextureWrapper : IDisposable
    {
        #region Methods

        /// <summary>
        /// Sets data of the texture
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct;

        /// <summary>
        /// Sets data of the texture for a specific area
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="mipmapLevel">MipMap level</param>
        /// <param name="top">Top position</param>
        /// <param name="bottom">Bottom position</param>
        /// <param name="left">Left position</param>
        /// <param name="right">Right position</param>
        /// <param name="front">Front position</param>
        /// <param name="back">Back position</param>
        void SetData<T>(T[] data, int startIndex, int elementCount, int top, int bottom,
            int left, int right, int front, int back, int mipmapLevel) where T : struct;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying texture
        /// </summary>
        XnaTexture Texture { get; }

        /// <summary>
        /// Gets the width of the texture
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the texture
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the depth of the texture
        /// </summary>
        int Depth { get; }

        #endregion
    }
}
