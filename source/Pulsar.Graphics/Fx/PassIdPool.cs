using System;
using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    internal sealed partial class PassIdPool
    {
        #region Fields

        private readonly List<PassIdRange> _availables = new List<PassIdRange>();
        private ushort _nextId;

        #endregion

        #region Methods

        public void Reset()
        {
            _availables.Clear();
            _nextId = 0;
        }

        public ushort GetRange(ushort length)
        {
            if (_availables.Count > 0)
            {
                for (int i = 0; i < _availables.Count; i++)
                {
                    PassIdRange range = _availables[i];
                    if (range.Length != length)
                        continue;

                    _availables.RemoveAt(i);

                    return range.FirstId;
                }
            }

            if((_nextId + length) > ushort.MaxValue)
                throw new Exception("");

            ushort result = _nextId;
            _nextId += length;

            return result;
        }

        public void ReleaseRange(ushort firstId, ushort length)
        {
            _availables.Add(new PassIdRange(firstId, length));
        }

        #endregion
    }
}
