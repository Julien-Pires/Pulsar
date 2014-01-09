using System;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics
{
    public interface ITextureWrapper : IDisposable
    {
        #region Methods

        void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct;

        void SetData<T>(T[] data, int startIndex, int elementCount, int left, int right, int top, int bottom,
            int front, int back, int mipmapLevel) where T : struct;

        #endregion

        #region Properties

        XnaTexture Texture { get; }

        int Width { get; }

        int Height { get; }

        int Depth { get; }

        #endregion
    }
}
