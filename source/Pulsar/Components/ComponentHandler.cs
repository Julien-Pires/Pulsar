using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    /// <summary>
    /// ComponentHandler are the core of components, they define how a component behave.
    /// Component contain only data, ComponentHandler contain all the logic. A ComponentHandler can
    /// be associated to one or more component type.
    /// </summary>
    public abstract class ComponentHandler : IDisposable
    {
        #region Fields

        protected ComponentHandlerSystem owner;
        protected Type[] componentTypes;

        private bool isInitialized;
        private bool isDisposed;

        #endregion

        #region Methods

        /// <summary>
        /// Initialize this ComponentHandler
        /// </summary>
        public virtual void Initialize()
        {
            this.isInitialized = true;
        }

        /// <summary>
        /// Dispose this ComponentHandler
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose this ComponentHandler
        /// </summary>
        /// <param name="disposing">Indicate to dispose</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (this.owner != null)
                {
                    this.owner.Remove(this);
                }
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Update this ComponentHandler.
        /// </summary>
        /// <param name="time"></param>
        public abstract void Tick(GameTime time);

        /// <summary>
        /// Register a component
        /// </summary>
        /// <param name="compo">Component to register</param>
        public abstract void Register(Component compo);

        /// <summary>
        /// Unregister a component
        /// </summary>
        /// <param name="compo">Component to unregister</param>
        /// <returns>Return true if the component is removed successfully otherwise false</returns>
        public abstract bool Unregister(Component compo);

        #endregion

        #region Properties

        /// <summary>
        /// Get the owner of this ComponentHandler
        /// </summary>
        public ComponentHandlerSystem Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates to listen for all components
        /// </summary>
        public bool ListenAllComponents { get; set; }

        /// <summary>
        /// Get the types of component associated
        /// </summary>
        public Type[] ComponentTypes
        {
            get { return this.componentTypes; }
        }

        /// <summary>
        /// Get a value indicating if this instance is disposed
        /// </summary>
        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        /// <summary>
        /// Get a value indicating if this instance is initialized
        /// </summary>
        public bool IsInitialized
        {
            get { return this.isInitialized; }
        }

        /// <summary>
        /// Get a value indicating if this ComponentHandler is enabled
        /// </summary>
        public virtual bool IsEnabled
        {
            get { return this.isInitialized && !this.isDisposed; }
        }

        #endregion
    }
}
