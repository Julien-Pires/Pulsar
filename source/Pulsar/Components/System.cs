using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    /// <summary>
    /// Defines the core of components, they define how a component behave.
    /// Component contain only data, System contain all the logic. 
    /// A System can be associated to one or more component type.
    /// </summary>
    public abstract class System : IDisposable
    {
        #region Methods

        /// <summary>
        /// Initializes this System
        /// </summary>
        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">Indicates if called from Dispose method</param>
        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        /// <summary>
        /// Updates this System.
        /// </summary>
        /// <param name="time">Time</param>
        public abstract void Update(GameTime time);

        /// <summary>
        /// Registers a component
        /// </summary>
        /// <param name="compo">Component to register</param>
        public abstract void Register(Component compo);

        /// <summary>
        /// Unregisters a component
        /// </summary>
        /// <param name="compo">Component to unregister</param>
        /// <returns>Return true if the component is removed successfully otherwise false</returns>
        public abstract bool Unregister(Component compo);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the system manager that owns this system
        /// </summary>
        public SystemManager Owner { get; internal set; }

        /// <summary>
        /// Gets or sets a value that indicates to listen for all components
        /// </summary>
        public bool ListenAllComponents { get; set; }

        /// <summary>
        /// Gets the types of component associated
        /// </summary>
        public abstract Type[] ComponentTypes { get; }

        /// <summary>
        /// Gets a value indicating if this instance is disposed
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating if this instance is initialized
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// Gets a value indicating if this System is enabled
        /// </summary>
        public virtual bool IsEnabled
        {
            get { return IsInitialized && !IsDisposed; }
        }

        #endregion
    }
}
