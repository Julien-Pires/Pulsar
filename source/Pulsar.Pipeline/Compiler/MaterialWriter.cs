using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using Pulsar.Graphics;
using Pulsar.Graphics.Asset;
using Pulsar.Pipeline.Graphics;

namespace Pulsar.Pipeline.Compiler
{
    [ContentTypeWriter]
    public sealed class MaterialWriter : ContentTypeWriter<MaterialContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof (MaterialReader).AssemblyQualifiedName;
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof (Material).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, MaterialContent value)
        {
            value.Write(output);
        }
    }
}
