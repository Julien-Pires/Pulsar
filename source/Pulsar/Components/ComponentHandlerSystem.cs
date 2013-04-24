﻿using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    using HandlersTypeMap = System.Collections.Generic.Dictionary<System.Type, Pulsar.Components.ComponentHandler>;

    /// <summary>
    /// A ComponentHandlerSystem is the master system for all ComponentHandler. This class is 
    /// responsible for intializing, updating and disposing ComponentHandler. 
    /// ComponentHandlerSystem listens to the add/remove event concerning component to dispatch them.
    /// </summary>
    public sealed class ComponentHandlerSystem : IDisposable
    {
        #region Fields

        private bool isDisposed;
        private GameObjectManager goManager;
        private HandlersTypeMap handlersMap = new HandlersTypeMap();
        private Dictionary<Type, HandlersTypeMap> handlersMapByComponent = new Dictionary<Type, HandlersTypeMap>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ComponentHandlerSystem class
        /// </summary>
        /// <param name="goMngr">Associated game object manager</param>
        public ComponentHandlerSystem(GameObjectManager goMngr)
        {
            this.goManager = goMngr;
            this.goManager.ComponentAdded += this.OnComponentAdded;
            this.goManager.ComponentRemoved += this.OnComponentRemoved;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize this instance
        /// </summary>
        public void Initialize()
        {
            foreach (ComponentHandler hnd in this.handlersMap.Values)
            {
                hnd.Initialize();
            }
        }

        /// <summary>
        /// Dispose this instance
        /// </summary>
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

        /// <summary>
        /// Add a ComponentHandler
        /// </summary>
        /// <param name="hnd">ComponentHandler to add</param>
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

        /// <summary>
        /// Add a component handler to listen for add component event
        /// </summary>
        /// <param name="hnd">ComponentHandler listening</param>
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

        /// <summary>
        /// Add a ComponentHandler to listen for add component event
        /// </summary>
        /// <param name="compoType">Type of component to listen for</param>
        /// <param name="hnd">ComponentHandler listening</param>
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

        /// <summary>
        /// Remove a ComponentHandler listener
        /// </summary>
        /// <param name="hnd">ComponentHandler which want to stop to listen</param>
        /// <returns>Return true if the ComponentHandler stops to listen otherwise false</returns>
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

        /// <summary>
        /// Remove a ComponentHandler listener for a specific component type
        /// </summary>
        /// <param name="compoType">Component type</param>
        /// <param name="hnd">ComponentHandler which want to stop to listen</param>
        /// <returns>Return true if the ComponentHandler stops to listen otherwise false</returns>
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

        /// <summary>
        /// Remove a ComponentHandler from this system
        /// </summary>
        /// <param name="hnd">ComponentHandler to remove</param>
        /// <returns>Return true if the ComponentHandler is removed successfully otherwise false</returns>
        public bool Remove(ComponentHandler hnd)
        {
            return this.Remove(hnd.GetType());
        }

        /// <summary>
        /// Remove a ComponentHandler from this system
        /// </summary>
        /// <typeparam name="T">Type of ComponentHandler to remove</typeparam>
        /// <returns>Return true if the ComponentHandler is removed successfully otherwise false</returns>
        public bool Remove<T>() where T : ComponentHandler
        {
            return this.Remove(typeof(T));
        }

        /// <summary>
        /// Remove a ComponentHandler from this system
        /// </summary>
        /// <param name="type">Type of ComponentHandler to remove</param>
        /// <returns>Return true if the ComponentHandler is removed successfully otherwise false</returns>
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

        /// <summary>
        /// Update all ComponentHandler
        /// </summary>
        /// <param name="time">Elapsed time since the last frame</param>
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

        /// <summary>
        /// Called when a component is added to the associated game object manager
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Argument for the event</param>
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

        /// <summary>
        /// Called when a component is removed to the associated game object manager
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Argument for the event</param>
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

        /// <summary>
        /// Get a value indicating if this instance is disposed
        /// </summary>
        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        #endregion
    }
}
