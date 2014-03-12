using Microsoft.Xna.Framework.Content;

using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents a content type reader that convert a binary input to a shader asset
    /// </summary>
    internal sealed class ShaderReader : ContentTypeReader<Shader>
    {
        #region Methods

        /// <summary>
        /// Converts a binary input to a Shader
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="existingInstance">Existing instance</param>
        /// <returns>Returns a Shader</returns>
        protected override Shader Read(ContentReader input, Shader existingInstance)
        {
            return Shader.Read(input);
        }

        #endregion
    }
}
