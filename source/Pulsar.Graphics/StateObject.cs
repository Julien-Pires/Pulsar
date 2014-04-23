using System;
using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a state object
    /// </summary>
    /// <typeparam name="T">State type</typeparam>
    public sealed class StateObject<T> : IDisposable where T : GraphicsResource
    {
        #region Fields

        private static readonly IndexPool IndexPool = new IndexPool();

        private readonly ushort _id;
        private bool _isDisposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of StateObject class
        /// </summary>
        /// <param name="state">State instance</param>
        internal StateObject(T state)
        {
            Debug.Assert(state != null);

            _id = (ushort)IndexPool.Get();
            State = state;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
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

        /// <summary>
        /// Gets the id of the state
        /// </summary>
        public ushort Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the underlying state object
        /// </summary>
        public T State { get; private set; }

        #endregion
    }
}
