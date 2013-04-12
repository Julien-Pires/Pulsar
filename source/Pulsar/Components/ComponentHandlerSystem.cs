using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    public sealed class ComponentHandlerSystem : IDisposable
    {
        #region Fields

        private bool isDisposed;
        private GameObjectManager goManager;
        private Dictionary<Type, ComponentHandler> handlersMap = new Dictionary<Type, ComponentHandler>();

        #endregion

        #region Constructors

        public ComponentHandlerSystem(GameObjectManager goMngr)
        {
            this.goManager = goMngr;
            this.goManager.ComponentAdded += this.OnComponentAdded;
            this.goManager.ComponentRemoved += this.OnComponentRemoved;
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            foreach (ComponentHandler hnd in this.handlersMap.Values)
            {
                hnd.Initialize();
            }
        }

        public void Dispose()
        {
            foreach (ComponentHandler hnd in this.handlersMap.Values)
            {
                hnd.Dispose();
                hnd.Owner = null;
            }

            this.goManager.ComponentAdded -= this.OnComponentAdded;
            this.goManager.ComponentRemoved -= this.OnComponentRemoved;
            this.handlersMap.Clear();
            this.handlersMap = null;
            this.isDisposed = true;
        }

        public void Add(ComponentHandler hnd)
        {
            if (hnd.Owner != null)
            {
                if (hnd.Owner != this)
                {
                    if (!hnd.Owner.Remove(hnd))
                    {
                        throw new Exception(string.Format("Failed to remove {0} from its current owner", hnd));
                    }
                }
                else
                {
                    return;
                }
            }
            if(this.handlersMap.ContainsKey(hnd.GetType()))
            {
                throw new Exception(string.Format("This system of component handler already have a handler of type {0}", hnd));
            }

            this.handlersMap.Add(hnd.GetType(), hnd);
            hnd.Owner = this;
        }

        public bool Remove(ComponentHandler hnd)
        {
            return this.Remove(hnd.GetType());
        }

        public bool Remove<T>() where T : ComponentHandler
        {
            return this.Remove(typeof(T));
        }

        public bool Remove(Type type)
        {
            ComponentHandler hnd;
            this.handlersMap.TryGetValue(type, out hnd);
            if (hnd == null)
            {
                return false;
            }

            bool result = this.handlersMap.Remove(type);
            if (result)
            {
                hnd.Owner = null;
            }

            return result;
        }

        public void Tick(GameTime time)
        {
            foreach (ComponentHandler hnd in this.handlersMap.Values)
            {
                if (hnd.IsEnabled)
                {
                    hnd.Tick(time);
                }
            }
        }

        private void OnComponentAdded(object sender, ComponentEventArgs e)
        {
            foreach (ComponentHandler hnd in this.handlersMap.Values)
            {
                hnd.Register(e.Component);
            }
        }

        private void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            foreach (ComponentHandler hnd in this.handlersMap.Values)
            {
                hnd.Unregister(e.Component);
            }
        }

        #endregion

        #region Properties

        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        #endregion
    }
}
