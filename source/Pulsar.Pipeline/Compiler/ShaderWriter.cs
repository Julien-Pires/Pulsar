using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using Pulsar.Graphics.Asset;
using Pulsar.Graphics.Fx;
using Pulsar.Pipeline.Graphics;

namespace Pulsar.Pipeline.Compiler
{
    /// <summary>
    /// Provides methods to compile a shader content into binary format
    /// </summary>
    [ContentTypeWriter]
    internal sealed class ShaderWriter : ContentTypeWriter<ShaderContent>
    {
        #region Methods

        /// <summary>
        /// Gets the shader runtime type
        /// </summary>
        /// <param name="targetPlatform">Target platform</param>
        /// <returns>Returns a fully qualified name of the runtime type</returns>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof (Shader).AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the reader type used to convert binary to shader
        /// </summary>
        /// <param name="targetPlatform">Target platform</param>
        /// <returns>Returns a fully qualified name of the binary reader</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof (ShaderReader).AssemblyQualifiedName;
        }

        /// <summary>
        /// Converts a shader content into binary
        /// </summary>
        /// <param name="output">Output</param>
        /// <param name="value">Shader content</param>
        protected override void Write(ContentWriter output, ShaderContent value)
        {
            value.Write(output);
        }

        #endregion
    }
}
