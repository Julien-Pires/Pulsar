using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using Pulsar.Graphics.Asset;
using Pulsar.Graphics.Fx;
using Pulsar.Pipeline.Graphics;

namespace Pulsar.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    internal sealed class ShaderWriter : ContentTypeWriter<ShaderContent>
    {
        #region Methods

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof (Shader).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof (ShaderReader).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, ShaderContent value)
        {
            value.Write(output);
        }

        #endregion
    }
}
