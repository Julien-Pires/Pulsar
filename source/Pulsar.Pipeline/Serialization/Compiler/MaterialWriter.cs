using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using Pulsar.Graphics;
using Pulsar.Pipeline.Graphics;

namespace Pulsar.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    public sealed class MaterialWriter : ContentTypeWriter<MaterialContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof (Material).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, MaterialContent value)
        {
            value.Write(output);
        }
    }
}
