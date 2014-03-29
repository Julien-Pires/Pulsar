using Microsoft.Xna.Framework.Content;

namespace Pulsar.Graphics.Asset
{
    public sealed class MaterialReader : ContentTypeReader<Material>
    {
        protected override Material Read(ContentReader input, Material existingInstance)
        {
            return Material.Read(input);
        }
    }
}
