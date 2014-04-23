using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents the method that will handle material changes
    /// </summary>
    /// <param name="material">Material</param>
    /// <param name="definition">Technique</param>
    public delegate void MaterialTechniqueChangedHandler(Material material, TechniqueDefinition definition);
}