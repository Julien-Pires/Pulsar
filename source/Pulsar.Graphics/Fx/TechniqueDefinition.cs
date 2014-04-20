using System;
using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Describes a shader technique
    /// </summary>
    public sealed class TechniqueDefinition : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private PassDefinition[] _passes;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of TechniqueDefinition class
        /// </summary>
        /// <param name="technique">Effect technique</param>
        internal TechniqueDefinition(EffectTechnique technique)
        {
            Debug.Assert(technique != null);

            Technique = technique;

            EffectPassCollection passes = technique.Passes;
            _passes = new PassDefinition[passes.Count];
            for (int i = 0; i < passes.Count; i++)
            {
                PassDefinition definition = new PassDefinition(passes[i])
                {
                    Index = (ushort)i,
                    State = RenderState.Default
                };
                _passes[i] = definition;
            }
        }

        #endregion

        #region Operators

        public PassDefinition this[string name]
        {
            get { return GetPass(name); }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed)
                return;

            Technique = null;
            _passes = null;

            _isDisposed = true;
        }

        public PassDefinition GetPass(int index)
        {
            return _passes[index];
        }

        public PassDefinition GetPass(string name)
        {
            int index = GetPassIndex(name);

            return (index > -1) ? _passes[index] : null;
        }

        private int GetPassIndex(string name)
        {
            for (int i = 0; i < _passes.Length; i++)
            {
                if (string.Equals(name, _passes[i].Name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying effect technique
        /// </summary>
        internal EffectTechnique Technique { get; private set; }

        internal PassDefinition[] Passes
        {
            get { return _passes; }
        }

        /// <summary>
        /// Gets the name of the technique
        /// </summary>
        public string Name
        {
            get { return Technique.Name; }
        }

        public int PassCount
        {
            get { return _passes.Length; }
        }

        /// <summary>
        /// Gets a value that indicate if the technique is used as fallback
        /// </summary>
        public bool IsFallback { get; internal set; }

        /// <summary>
        /// Gets a value that indicates if the technique use transparency
        /// </summary>
        public bool IsTransparent { get; internal set; }

        /// <summary>
        /// Gets a value that indicates if the technique is used for instancing
        /// </summary>
        public bool IsInstancing { get; internal set; }

        #endregion
    }
}
