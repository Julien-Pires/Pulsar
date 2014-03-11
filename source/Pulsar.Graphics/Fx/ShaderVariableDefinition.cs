using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed class ShaderVariableDefinition
    {
        #region Constructor

        internal ShaderVariableDefinition(string name, EffectParameter parameter, Type type)
        {
            Name = name;
            Parameter = parameter;
            Type = type;
        }

        #endregion

        #region Properties

        internal EffectParameter Parameter { get; private set; }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public string Semantic { get; internal set; }

        public ShaderVariableUsage Usage { get; internal set; }

        public ShaderVariableSource Source { get; internal set; }

        #endregion
    }
}