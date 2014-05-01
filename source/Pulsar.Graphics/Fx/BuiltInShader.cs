using Microsoft.Xna.Framework.Content;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Contains informations about a built-in shader
    /// </summary>
    internal sealed class BuiltInShader
    {
        #region Properties

        /// <summary>
        /// Gets the name of the shader
        /// </summary>
        [ContentSerializer]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the path to the shader definition file
        /// </summary>
        [ContentSerializer]
        public string Path { get; internal set; }

        #endregion
    }
}
