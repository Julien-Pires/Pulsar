using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    /// <summary>
    /// A ComponentHandlerSystem is the master system for all ComponentHandler. This class is 
    /// responsible for intializing, updating and disposing ComponentHandler. 
    /// ComponentHandlerSystem listens to the add/remove event concerning component to dispatch them.
    /// </summary>
    public sealed class ComponentHandlerSystem : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private readonly GameObjectManager _goManager;
        private readonly Dictionary<Type, ComponentHandler> _handlersMap = new Dictionary<Type, ComponentHandler>();
        private readonly Dictionary<Type, List<ComponentHandler>> _handlersMapByComponent =
            new Dictionary<Type, List<ComponentHandler>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ComponentHandlerSystem class
        /// </summary>
        /// <param name="goManager">Associated game object manager</param>
        public ComponentHandlerSystem(GameObjectManager goManager)
        {
            _goManager = goManager;
            _goManager.ComponentAdded += ComponentAdded;
            _goManager.ComponentRemoved += ComponentRemoved;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance
        /// </summary>
        public void Initialize()
        {
            foreach (ComponentHandler handler in _handlersMap.Values) 
                handler.Initialize();
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) return;

            _goManager.ComponentAdded -= ComponentAdded;
            _goManager.ComponentRemoved -= ComponentRemoved;

            foreach (List<ComponentHandler> listeners in _handlersMapByComponent.Values)
                listeners.Clear();
            _handlersMapByComponent.Clear();

            foreach (ComponentHandler handler in _handlersMap.Values)
            {
                handler.Dispose();
                handler.Owner = null;
            }
            _handlersMap.Clear();
            _handlersMapByComponent.Clear();

            _isDisposed = true;
        }

        /// <summary>
        /// Adds a ComponentHandler
        /// </summary>
        /// <param name="handler">ComponentHandler to add</param>
        public void Add(ComponentHandler handler)
        {
            if(handler == null)
                throw new ArgumentNullException("handler");

            if (handler.Owner != null)
            {
                if (handler.Owner == this) return;
                throw new ArgumentException("Component handler already attached to another ComponentHandlerSystem", "handler");
            }

            Type handlerType = handler.GetType();
            if (_handlersMap.ContainsKey(handlerType))
                throw new Exception(string.Format("Failed to add, a {0} " +
                                                  "already attached to this ComponentHandlerSystem", handlerType));

            handler.Owner = this;
            _handlersMap.Add(handlerType, handler);
            if(handler.ListenAllComponents)
                RegisterListener(handler, typeof(Component));
            else
                RegisterListener(handler, handler.ComponentTypes); 
        }

        /// <summary>
        /// Adds a ComponentHandler to listen for a specific component
        /// </summary>
        /// <param name="handler">ComponentHandler as a listener</param>
        /// <param name="componentType">Type of component to listen for</param>
        private void RegisterListener(ComponentHandler handler, Type componentType)
        {
            if(componentType == null)
                throw new ArgumentNullException("componentType");
            
            List<ComponentHandler> listeners = EnsureListeners(componentType);
            listeners.Add(handler);
        }

        /// <summary>
        /// Adds a ComponentHandler to listen for multiple components
        /// </summary>
        /// <param name="handler">ComponentHandler as a listener</param>
        /// <param name="componentTypes">Array of type of components to listen for</param>
        private void RegisterListener(ComponentHandler handler, Type[] componentTypes)
        {
            if(componentTypes == null)
                throw new ArgumentNullException("componentTypes");

            for (int i = 0; i < componentTypes.Length; i++)
            {
                Type compoType = componentTypes[i];
                if(compoType == null)
                    throw new ArgumentException("null is not a valid type of component", "componentTypes");

                if (!compoType.IsSubclassOf(typeof(Component)))
                    throw new Exception("Provided type doesn't inherit Component class");

                List<ComponentHandler> listeners = EnsureListeners(compoType);
                listeners.Add(handler);
            }
        }

        /// <summary>
        /// Removes a ComponentHandler
        /// </summary>
        /// <param name="handler">ComponentHandler to remove</param>
        /// <returns>Return true if the ComponentHandler is removed successfully otherwise false</returns>
        public bool Remove(ComponentHandler handler)
        {
            if(handler == null)
                throw new ArgumentNullException("handler");

            return Remove(handler.GetType());
        }

        /// <summary>
        /// Removes a ComponentHandler from its type
        /// </summary>
        /// <typeparam name="T">Type of ComponentHandler to remove</typeparam>
        /// <returns>Return true if the ComponentHandler is removed successfully otherwise false</returns>
        public bool Remove<T>() where T : ComponentHandler
        {
            return Remove(typeof(T));
        }

        /// <summary>
        /// Removes a ComponentHandler from its type
        /// </summary>
        /// <param name="type">Type of ComponentHandler to remove</param>
        /// <returns>Return true if the ComponentHandler is removed successfully otherwise false</returns>
        public bool Remove(Type type)
        {
            if(type == null)
                throw new ArgumentNullException("type");

            ComponentHandler handler = GetComponentHandler(type);
            if (handler == null) return false;

            if(handler.ListenAllComponents)
                UnregisterListener(handler, typeof(Component));
            else
                UnregisterListener(handler, handler.ComponentTypes);
                
            handler.Owner = null;

            return true;
        }

        /// <summary>
        /// Removes a ComponentHandler to stop listening for a specific component
        /// </summary>
        /// <param name="handler">ComponentHandler as a listener</param>
        /// <param name="componentType">Type of component to stop listening for</param>
        private void UnregisterListener(ComponentHandler handler, Type componentType)
        {
            if(componentType == null)
                throw new ArgumentNullException("componentType");

            List<ComponentHandler> listeners = GetListeners(componentType);
            if(listeners == null) return;
            listeners.Remove(handler);
        }

        /// <summary>
        /// Removes a ComponentHandler to stop listening for multiple components
        /// </summary>
        /// <param name="handler">ComponentHandler as a listener</param>
        /// <param name="componentTypes">Array of type of components to listening for</param>
        private void UnregisterListener(ComponentHandler handler, Type[] componentTypes)
        {
            if (componentTypes == null)
                throw new ArgumentNullException("componentTypes");

            for (int i = 0; i < componentTypes.Length; i++)
            {
                Type compoType = componentTypes[i];
                if(compoType == null)
                    throw new ArgumentException("null is not a valid type of component", "componentTypes");

                if (!compoType.IsSubclassOf(typeof(Component)))
                    throw new Exception("Provided type doesn't inherit Component class");

                List<ComponentHandler> listeners = GetListeners(compoType);
                if(listeners == null) continue;
                listeners.Add(handler);
            }
        }

        /// <summary>
        /// Gets a component handler attached to this manager
        /// </summary>
        /// <param name="handlerType">Type of component handler</param>
        /// <returns>Returns a ComponentHandler instance if found otherwise null</returns>
        public ComponentHandler GetComponentHandler(Type handlerType)
        {
            if(handlerType == null)
                throw new ArgumentNullException("handlerType");

            ComponentHandler handler;
            _handlersMap.TryGetValue(handlerType, out handler);

            return handler;
        }

        /// <summary>
        /// Gets a list of listeners for a specific type of component
        /// </summary>
        /// <param name="componentType">Type of component</param>
        /// <returns>Returns a list of component handler that listen for the specified type of component otherwise null</returns>
        private List<ComponentHandler> GetListeners(Type componentType)
        {
            if(componentType == null)
                throw new ArgumentNullException("componentType");

            List<ComponentHandler> handlersList;
            _handlersMapByComponent.TryGetValue(componentType, out handlersList);

            return handlersList;
        }

        /// <summary>
        /// Gets a list of listener, or create it if it doesn't exist, for a specific type of component
        /// </summary>
        /// <param name="componentType">Type of component</param>
        /// <returns>Returns a list of listener otherwise null</returns>
        private List<ComponentHandler> EnsureListeners(Type componentType)
        {
            List<ComponentHandler> handlersList = GetListeners(componentType);
            if (handlersList != null) return handlersList;

            handlersList = new List<ComponentHandler>();
            _handlersMapByComponent.Add(componentType, handlersList);

            return handlersList;
        }

        /// <summary>
        /// Updates all component handler
        /// </summary>
        /// <param name="time">Elapsed time since the last frame</param>
        public void Tick(GameTime time)
        {
            foreach (ComponentHandler handler in _handlersMap.Values)
            {
                if (handler.IsEnabled) 
                    handler.Tick(time);
            }
        }

        /// <summary>
        /// Called when a component is added to the associated game object manager
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Argument for the event</param>
        private void ComponentAdded(object sender, ComponentEventArgs e)
        {
            Component component = e.Component;
            List<ComponentHandler> listeners = GetListeners(typeof(Component));
            if (listeners != null)
            {
                for(int i = 0; i < listeners.Count; i++) 
                    listeners[i].Register(component);
            }

            listeners = GetListeners(component.GetType());
            if (listeners != null)
            {
                for (int i = 0; i < listeners.Count; i++)
                    listeners[i].Register(component);
            }
        }

        /// <summary>
        /// Called when a component is removed to the associated game object manager
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Argument for the event</param>
        private void ComponentRemoved(object sender, ComponentEventArgs e)
        {
            Component component = e.Component;
            List<ComponentHandler> listeners = GetListeners(typeof(Component));
            if (listeners != null)
            {
                for (int i = 0; i < listeners.Count; i++)
                    listeners[i].Unregister(component);
            }

            listeners = GetListeners(component.GetType());
            if (listeners != null)
            {
                for (int i = 0; i < listeners.Count; i++)
                    listeners[i].Unregister(component);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating if this instance is disposed
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        #endregion
    }
}
