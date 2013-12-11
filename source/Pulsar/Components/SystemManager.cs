using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    /// <summary>
    /// Manages system and dispatches component to them
    /// </summary>
    public sealed class SystemManager : IDisposable
    {
        #region Fields

        private readonly static Type BaseComponentType = typeof (Component);

        private readonly Dictionary<Type, System> _systemMap = new Dictionary<Type, System>();
        private readonly Dictionary<Type, List<System>> _listenersMap = new Dictionary<Type, List<System>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SystemManager class
        /// </summary>
        internal SystemManager()
        {
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Checks if the specified type is a Component or is a sub class of Component
        /// </summary>
        /// <param name="componentType">Type instances</param>
        /// <returns>Returns true if it's a valid type otherwise false</returns>
        private static bool ValidateComponentType(Type componentType)
        {
            if (componentType == BaseComponentType)
                return true;

            return (componentType != null) && (componentType.IsSubclassOf(BaseComponentType));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes systems
        /// </summary>
        public void Initialize()
        {
            foreach (System system in _systemMap.Values) 
                system.Initialize();
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            if(IsDisposed) return;

            foreach (List<System> listeners in _listenersMap.Values)
                listeners.Clear();

            foreach (System system in _systemMap.Values)
            {
                system.Dispose();
                system.Owner = null;
            }
            _systemMap.Clear();
            _listenersMap.Clear();

            IsDisposed = true;
        }

        /// <summary>
        /// Adds a System
        /// </summary>
        /// <param name="system">System to add</param>
        public void Add(System system)
        {
            if(system == null)
                throw new ArgumentNullException("system");

            if (system.Owner != null)
            {
                if (system.Owner == this) return;
                throw new ArgumentException("System already attached to another SystemManager", "system");
            }

            Type systemType = system.GetType();
            if (_systemMap.ContainsKey(systemType))
                throw new Exception(string.Format("Failed to add, a {0} is " +
                                                  "already attached to this SystemManager", systemType));

            system.Owner = this;
            _systemMap.Add(systemType, system);
            if(system.ListenAllComponents)
                RegisterListener(system, BaseComponentType);
            else
                RegisterListener(system, system.ComponentTypes);
        }

        /// <summary>
        /// Adds a System to listen for a specific component
        /// </summary>
        /// <param name="system">System as a listener</param>
        /// <param name="componentType">Type of component to listen for</param>
        private void RegisterListener(System system, Type componentType)
        {
            if(!ValidateComponentType(componentType))
                throw new ArgumentException(string.Format("{0} is not a valid component type", componentType));
            
            List<System> listeners = EnsureListeners(componentType);
            listeners.Add(system);
        }

        /// <summary>
        /// Adds a System to listen for multiple components
        /// </summary>
        /// <param name="system">System as a listener</param>
        /// <param name="componentTypes">Array of type of components to listen for</param>
        private void RegisterListener(System system, Type[] componentTypes)
        {
            if(componentTypes == null)
                throw new ArgumentNullException("componentTypes");

            for (int i = 0; i < componentTypes.Length; i++)
            {
                Type compoType = componentTypes[i];
                if(!ValidateComponentType(compoType))
                    throw new ArgumentException(string.Format("{0} is not a valid component type", compoType));

                List<System> listeners = EnsureListeners(compoType);
                listeners.Add(system);
            }
        }

        /// <summary>
        /// Removes a System
        /// </summary>
        /// <param name="system">System to remove</param>
        /// <returns>Return true if the System is removed successfully otherwise false</returns>
        public bool Remove(System system)
        {
            if(system == null)
                throw new ArgumentNullException("system");

            return Remove(system.GetType());
        }

        /// <summary>
        /// Removes a System with a specified type
        /// </summary>
        /// <typeparam name="T">Type of System to remove</typeparam>
        /// <returns>Return true if the System is removed successfully otherwise false</returns>
        public bool Remove<T>() where T : System
        {
            return Remove(typeof(T));
        }

        /// <summary>
        /// Removes a System with a specified type
        /// </summary>
        /// <param name="type">Type of System to remove</param>
        /// <returns>Return true if the System is removed successfully otherwise false</returns>
        public bool Remove(Type type)
        {
            if(type == null)
                throw new ArgumentNullException("type");

            System system = GetSystem(type);
            if (system == null) 
                return false;

            UnregisterListener(system);
            _systemMap.Remove(type);
            system.Owner = null;

            return true;
        }

        /// <summary>
        /// Removes a System to stop listening for components
        /// </summary>
        /// <param name="system">System as a listener</param>
        private void UnregisterListener(System system)
        {
            foreach (List<System> listeners in _listenersMap.Values)
                listeners.Remove(system);
        }

        /// <summary>
        /// Gets a system with a specified type
        /// </summary>
        /// <typeparam name="T">Type of the system</typeparam>
        /// <returns>Returns a system instance if found otherwise null</returns>
        public T GetSystem<T>() where T : System
        {
            return GetSystem(typeof (T)) as T;
        }

        /// <summary>
        /// Gets a system with a specified type
        /// </summary>
        /// <param name="systemType">Type of component handler</param>
        /// <returns>Returns a System instance if found otherwise null</returns>
        public System GetSystem(Type systemType)
        {
            if(systemType == null)
                throw new ArgumentNullException("systemType");

            System system;
            _systemMap.TryGetValue(systemType, out system);

            return system;
        }

        /// <summary>
        /// Gets a list of listeners for a specific type of component
        /// </summary>
        /// <param name="componentType">Type of component</param>
        /// <returns>Returns a list of listeners if found otherwise null</returns>
        private List<System> GetListeners(Type componentType)
        {
            Debug.Assert(componentType != null);

            List<System> handlersList;
            _listenersMap.TryGetValue(componentType, out handlersList);

            return handlersList;
        }

        /// <summary>
        /// Gets a list of listener, or create it if it doesn't exist, for a specific type of component
        /// </summary>
        /// <param name="componentType">Type of component</param>
        /// <returns>Returns a list of listeners if found otherwise null</returns>
        private List<System> EnsureListeners(Type componentType)
        {
            Debug.Assert(componentType != null);

            List<System> listeners = GetListeners(componentType);
            if (listeners != null) 
                return listeners;

            listeners = new List<System>();
            _listenersMap.Add(componentType, listeners);

            return listeners;
        }

        /// <summary>
        /// Updates all systems
        /// </summary>
        /// <param name="time">Elapsed time since the last frame</param>
        internal void Update(GameTime time)
        {
            foreach (System system in _systemMap.Values)
            {
                if (system.IsEnabled) 
                    system.Update(time);
            }
        }

        /// <summary>
        /// Notifies appropriate listeners when a component is added
        /// </summary>
        /// <param name="component">Added component</param>
        internal void ComponentAdded(Component component)
        {
            Debug.Assert(component != null, "Cannot add null component");

            List<System> listeners = GetListeners(BaseComponentType);
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
        /// Notifies appropriate listeners when a component is removed
        /// </summary>
        /// <param name="component">Removed component</param>
        internal void ComponentRemoved(Component component)
        {
            Debug.Assert(component != null, "Cannot add null component");

            List<System> listeners = GetListeners(BaseComponentType);
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
        public bool IsDisposed { get; private set; }

        #endregion
    }
}
