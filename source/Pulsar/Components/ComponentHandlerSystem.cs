using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    using HandlersTypeMap = System.Collections.Generic.Dictionary<System.Type, Pulsar.Components.ComponentHandler>;

    public sealed class ComponentHandlerSystem : IDisposable
    {
        #region Fields

        private bool isDisposed;
        private GameObjectManager goManager;
        private HandlersTypeMap handlersMap = new HandlersTypeMap();
        private Dictionary<Type, HandlersTypeMap> handlersMapByComponent = new Dictionary<Type, HandlersTypeMap>();

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

            Type handlerType = hnd.GetType();
            if (this.handlersMap.ContainsKey(handlerType))
            {
                throw new Exception(string.Format("This system of component handler already have a handler of type {0}", hnd));
            }
            this.handlersMap.Add(handlerType, hnd);
            this.AddComponentListener(hnd);
            hnd.Owner = this;
        }

        private void AddComponentListener(ComponentHandler hnd)
        {
            Type[] allCompoTypes = hnd.ComponentTypes;
            if (allCompoTypes != null)
            {
                for (int i = 0; i < allCompoTypes.Length; i++)
                {
                    Type compoType = allCompoTypes[i];
                    if (!compoType.IsSubclassOf(typeof(Component)))
                    {
                        throw new Exception("The type provided by ComponentHandler instance doesn't inherit Component class");
                    }
                    this.AddComponentListener(compoType, hnd);
                }
            }
            else
            {
                this.AddComponentListener(typeof(Component), hnd);
            }
        }

        private void AddComponentListener(Type compoType, ComponentHandler hnd)
        {
            HandlersTypeMap map;
            this.handlersMapByComponent.TryGetValue(compoType, out map);
            if (map == null)
            {
                map = new HandlersTypeMap();
                this.handlersMapByComponent.Add(compoType, map);
            }

            Type handlerType = hnd.GetType();
            if (!map.ContainsKey(handlerType))
            {
                map.Add(handlerType, hnd);
            }
        }

        private bool RemoveComponentListener(ComponentHandler hnd)
        {
            bool result = true;
            Type handlerType = hnd.GetType();
            Type[] allCompoTypes = hnd.ComponentTypes;
            if (allCompoTypes != null)
            {
                for (int i = 0; i < allCompoTypes.Length; i++)
                {
                    Type compoType = allCompoTypes[i];
                    result &= this.RemoveComponentListener(compoType, hnd);
                }
            }
            else
            {
                Type compoType = typeof(Component);
                result &= this.RemoveComponentListener(compoType, hnd);   
            }

            return result;
        }

        private bool RemoveComponentListener(Type compoType, ComponentHandler hnd)
        {
            HandlersTypeMap map;
            this.handlersMapByComponent.TryGetValue(compoType, out map);
            if (map != null)
            {
                return map.Remove(hnd.GetType());
            }

            return false;
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
                this.RemoveComponentListener(hnd);
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
            Component component = e.Component;
            Type compoType = typeof(Component);
            HandlersTypeMap map;
            this.handlersMapByComponent.TryGetValue(compoType, out map);
            if (map != null)
            {
                foreach (ComponentHandler hnd in map.Values)
                {
                    hnd.Register(component);
                }
            }

            compoType = component.GetType();
            this.handlersMapByComponent.TryGetValue(compoType, out map);
            if (map != null)
            {
                foreach (ComponentHandler hnd in map.Values)
                {
                    hnd.Register(component);
                }
            }
        }

        private void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            Component component = e.Component;
            Type compoType = typeof(Component);
            HandlersTypeMap map;
            this.handlersMapByComponent.TryGetValue(compoType, out map);
            if (map != null)
            {
                foreach (ComponentHandler hnd in map.Values)
                {
                    hnd.Unregister(component);
                }
            }

            compoType = component.GetType();
            this.handlersMapByComponent.TryGetValue(compoType, out map);
            if (map != null)
            {
                foreach (ComponentHandler hnd in map.Values)
                {
                    hnd.Unregister(component);
                }
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
