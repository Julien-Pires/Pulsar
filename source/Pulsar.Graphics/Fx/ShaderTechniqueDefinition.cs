using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed class ShaderTechniqueDefinition
    {
        #region Constructor

        internal ShaderTechniqueDefinition(string name, EffectTechnique technique)
        {
            Name = name;
            Technique = technique;
        }

        #endregion

        #region Properties

        internal EffectTechnique Technique { get; private set; }

        public string Name { get; private set; }

        public bool IsFallback { get; internal set; }

        public bool IsTransparent { get; internal set; }

        public bool IsInstancing { get; internal set; }

        #endregion
    }
}
