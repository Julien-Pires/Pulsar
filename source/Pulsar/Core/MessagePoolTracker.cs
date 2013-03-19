using System;

using System.Collections.Generic;

namespace Pulsar.Core
{
    /// <summary>
    /// Manage a message pool
    /// Message can be acquired in the pool as many as needed but you can only
    /// free message's instance all at once and not one by one
    /// This can be used, for example, in component system where you need to send many message 
    /// and free them all at once when they have been processed by the mediator and handlers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MessagePoolTracker<T> where T : Message, new()
    {
        #region Fields

        private Pool<T> pool;
        private List<Pool<T>.Accessor> usedAccessor = new List<Pool<T>.Accessor>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MessagePoolTracker class
        /// </summary>
        /// <param name="capacity">Capacity of the pool</param>
        public MessagePoolTracker(int capacity)
        {
            this.pool = new Pool<T>(capacity);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a free message instance
        /// </summary>
        /// <returns>Return a message instance</returns>
        public T Get()
        {
            Pool<T>.Accessor access = this.pool.Get();
            this.usedAccessor.Add(access);

            return this.pool.Elements[access.Index];
        }

        /// <summary>
        /// Free all the instance in the pool
        /// </summary>
        public void Free()
        {
            for (int i = 0; i < this.usedAccessor.Count; i++)
            {
                this.pool.Release(this.usedAccessor[i]);
            }

            this.usedAccessor.Clear();
        }

        #endregion
    }
}
