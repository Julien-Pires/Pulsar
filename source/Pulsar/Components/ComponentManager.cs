using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Components
{
    /// <summary>
    /// Manages all components
    /// </summary>
    public sealed class ComponentManager
    {
        #region Fields

        private readonly Dictionary<Type, ComponentGroup> _componentsMap = new Dictionary<Type, ComponentGroup>();

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a component is added
        /// </summary>
        public event ComponentHandler ComponentAdded;

        /// <summary>
        /// Occurs when a component is removed
        /// </summary>
        public event ComponentHandler ComponentRemoved;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ComponentManager class
        /// </summary>
        internal ComponentManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a component
        /// </summary>
        /// <param name="component">Component</param>
        internal void Add(Component component)
        {
            Debug.Assert(component != null, "Component cannot be null");

            ComponentGroup components = EnsureGroup(component.GetType());
            components.Add(component);

            ComponentHandler componentAdded = ComponentAdded;
            if (componentAdded != null)
                componentAdded(this, component);
        }

        /// <summary>
        /// Removes a component
        /// </summary>
        /// <param name="component">Components</param>
        /// <returns>Returns true if removed otherwise false</returns>
        internal bool Remove(Component component)
        {
            Debug.Assert(component != null, "Component cannot be null");
            Debug.Assert(component.Parent != null, "Component parent cannot be null");

            ComponentGroup components = GetGroup(component.GetType());
            if (components == null)
                return false;

            if (components.Remove(component))
            {
                ComponentHandler componentRemoved = ComponentRemoved;
                if (componentRemoved != null)
                    componentRemoved(this, component);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Estimates the number of components for a specified type
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="count">Number of component</param>
        public void Estimate<T>(int count) where T : Component
        {
            Estimate(typeof(T), count);   
        }

        /// <summary>
        /// Estimates the number of components for a specified type
        /// </summary>
        /// <param name="componentType">Type of component</param>
        /// <param name="count">Number of component</param>
        public void Estimate(Type componentType, int count)
        {
            ComponentGroup group = EnsureGroup(componentType);
            group.Resize(count);
        }

        /// <summary>
        /// Gets a group of component with a specified type
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <returns>Returns a group of component if found otherwise null</returns>
        public ComponentGroup GetGroup<T>() where T : Component
        {
            return GetGroup(typeof (T));
        }

        /// <summary>
        /// Gets a group of component with a specified type
        /// </summary>
        /// <param name="componentType">Type of component</param>
        /// <returns>Returns a group of component if found otherwise null</returns>
        public ComponentGroup GetGroup(Type componentType)
        {
            ComponentGroup components;
            _componentsMap.TryGetValue(componentType, out components);

            return components;
        }

        /// <summary>
        /// Gets a group of component, or create it if it doesn't exist, with a specified type
        /// </summary>
        /// <param name="componentType">Type of component</param>
        /// <returns>Returns a group of component</returns>
        private ComponentGroup EnsureGroup(Type componentType)
        {
            Debug.Assert(componentType != null, "Component type cannot be null");
            Debug.Assert(componentType.IsSubclassOf(typeof(Component)), 
                string.Format("{0} doesn't inherit Component", componentType));

            ComponentGroup components;
            if (_componentsMap.TryGetValue(componentType, out components)) 
                return components;

            components = new ComponentGroup(componentType);
            _componentsMap.Add(componentType, components);

            return components;
        }

        #endregion
    }
}
