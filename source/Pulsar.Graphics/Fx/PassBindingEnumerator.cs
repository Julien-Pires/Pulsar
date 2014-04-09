using System.Collections;
using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Enumerates a collection of pass
    /// </summary>
    internal struct PassBindingEnumerator : IEnumerator<PassBinding>
    {
        #region Fields

        private PassBinding _current;
        private PassBinding[] _passArray;
        private int _index;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of PassBindingEnumerator struct
        /// </summary>
        /// <param name="definition">Technique that contains pass to enumerate</param>
        internal PassBindingEnumerator(TechniqueBinding definition)
        {
            _passArray = definition.PassesBindings;
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
            _passArray = null;
            _current = null;
        }

        /// <summary>
        /// Moves to the next pass
        /// </summary>
        /// <returns>Returns true if there is a next pass otherwise false</returns>
        public bool MoveNext()
        {
            if (_index < _passArray.Length)
            {
                _current = _passArray[_index];
                _index++;

                return true;
            }

            _current = null;
            _index = _passArray.Length;

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
        public PassBinding Current
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
