using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    public abstract class ComponentHandler : IDisposable
    {
        #region Fields

        protected ComponentHandlerSystem owner;
        protected Type[] componentTypes;

        private bool isInitialized;
        private bool isDisposed;

        #endregion

        #region Methods

        public virtual void Initialize()
        {
            this.isInitialized = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

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

        public abstract void Tick(GameTime time);

        public abstract void Register(Component compo);

        public abstract bool Unregister(Component compo);

        #endregion

        #region Properties

        public ComponentHandlerSystem Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }
        }

        public Type[] ComponentTypes
        {
            get { return this.componentTypes; }
        }

        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        public bool IsInitialized
        {
            get { return this.isInitialized; }
        }

        public virtual bool IsEnabled
        {
            get { return this.isInitialized && !this.isDisposed; }
        }

        #endregion
    }
}
