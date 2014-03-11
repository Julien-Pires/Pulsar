using Microsoft.Xna.Framework.Content;

using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics.Asset
{
    internal sealed class ShaderReader : ContentTypeReader<Shader>
    {
        #region Methods

        protected override Shader Read(ContentReader input, Shader existingInstance)
        {
            return Shader.Read(input);
        }

        #endregion
    }
}
