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

        private readonly static PassIdPool IdPool = new PassIdPool();

        private readonly ushort _firstPassId;
        private bool _isDisposed;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of TechniqueDefinition class
        /// </summary>
        /// <param name="technique">Effect technique</param>
        internal TechniqueDefinition(EffectTechnique technique)
        {
            Technique = technique;
            Passes = new PassDefinition[technique.Passes.Count];
            _firstPassId = IdPool.GetRange((ushort)Passes.Length);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed)
                return;

            try
            {
                IdPool.ReleaseRange(_firstPassId, (ushort) Passes.Length);
            }
            finally
            {
                Technique = null;
                Passes = null;

                _isDisposed = true;
            }
        }

        internal void SetPassDefinition(int index, PassDefinition passDefinition)
        {
            Debug.Assert(passDefinition != null);

            passDefinition.Index = (ushort) index;
            passDefinition.Id = (ushort)(_firstPassId + passDefinition.Index);
            Passes[index] = passDefinition;
        }

        internal void CreateMissingPass()
        {
            EffectPassCollection passCollection = Technique.Passes;
            for (int i = 0; i < passCollection.Count; i++)
            {
                if(Passes[i] != null)
                    continue;

                PassDefinition definition = new PassDefinition(passCollection[i], RenderState.Default);
                SetPassDefinition(i, definition);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying effect technique
        /// </summary>
        internal EffectTechnique Technique { get; private set; }

        internal PassDefinition[] Passes { get; private set; }

        /// <summary>
        /// Gets the name of the technique
        /// </summary>
        public string Name
        {
            get { return Technique.Name; }
        }

        public int PassCount
        {
            get { return Passes.Length; }
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
