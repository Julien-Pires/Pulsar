using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Enumerates a collection of pass
    /// </summary>
    internal struct ShaderPassEnumerator : IEnumerator<EffectPass>
    {
        #region Fields

        private EffectPass _current;
        private EffectPassCollection _passCollection;
        private int _index;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderPassEnumerator struct
        /// </summary>
        /// <param name="definition">Technique that contains pass to enumerate</param>
        internal ShaderPassEnumerator(ShaderTechniqueDefinition definition)
        {
            _passCollection = definition.Technique.Passes;
            _index = 0;
            _current = null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases resources
        /// </summary>
        public void Dispose()
        {
            _passCollection = null;
            _current = null;
        }

        /// <summary>
        /// Moves to the next pass
        /// </summary>
        /// <returns>Returns true if there is a next pass otherwise false</returns>
        public bool MoveNext()
        {
            if (_index < _passCollection.Count)
            {
                _current = _passCollection[_index];
                _index++;

                return true;
            }

            _current = null;
            _index = _passCollection.Count;

            return false;
        }

        /// <summary>
        /// Resets the enumerator
        /// </summary>
        public void Reset()
        {
            _current = null;
            _index = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current pass
        /// </summary>
        public EffectPass Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Gets the current pass
        /// </summary>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion
    }
}
