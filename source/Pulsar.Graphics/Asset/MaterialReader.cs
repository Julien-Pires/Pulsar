using Microsoft.Xna.Framework.Content;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents a reader that convert a binary input to a Material
    /// </summary>
    public sealed class MaterialReader : ContentTypeReader<Material>
    {
        /// <summary>
        /// Converts a binary input to a material
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="existingInstance">Existing instance</param>
        /// <returns>Returns a material instance</returns>
        protected override Material Read(ContentReader input, Material existingInstance)
        {
            return Material.Read(input);
        }
    }
}
