using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    internal struct ShaderPassEnumerator : IEnumerator<EffectPass>
    {
        #region Fields

        private EffectPass _current;
        private EffectPassCollection _passCollection;
        private int _index;

        #endregion

        #region Constructors

        internal ShaderPassEnumerator(ShaderTechniqueDefinition definition)
        {
            _passCollection = definition.Technique.Passes;
            _index = 0;
            _current = null;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            _passCollection = null;
            _current = null;
        }

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

        public void Reset()
        {
            _current = null;
            _index = 0;
        }

        #endregion

        #region Properties

        public EffectPass Current
        {
            get { return _current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion
    }
}
