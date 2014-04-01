using System;
using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public sealed class StateObject<T> : IDisposable where T : GraphicsResource
    {
        #region Fields

        private static readonly IndexPool IndexPool = new IndexPool();

        private readonly ushort _id;
        private bool _isDisposed;

        #endregion

        #region Constructors

        internal StateObject(T state)
        {
            Debug.Assert(state != null);

            _id = (ushort)IndexPool.Get();
            State = state;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed) 
                return;

            try
            {
                State.Dispose();
            }
            finally
            {
                State = null;
                IndexPool.Release(_id);

                _isDisposed = true;
            }
        }

        #endregion

        #region Properties

        public ushort Id
        {
            get { return _id; }
        }

        public T State { get; private set; }

        #endregion
    }
}
