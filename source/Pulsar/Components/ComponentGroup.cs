using System;

using Pulsar.Collections;

namespace Pulsar.Components
{
    /// <summary>
    /// Defines a group of component for a specific type
    /// </summary>
    public sealed class ComponentGroup
    {
        #region Fields

        private readonly Type _componentType;
        private UnorderedList<Component> _components = new UnorderedList<Component>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of ComponentGroup class
        /// </summary>
        /// <param name="componentType">Type of component managed by this group</param>
        internal ComponentGroup(Type componentType)
        {
            _componentType = componentType;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resizes the internal list with a specific capacity
        /// </summary>
        /// <param name="capacity">New capacity</param>
        internal void Resize(int capacity)
        {
            if(capacity < _components.Count)
                throw new ArgumentException("Insufficient capacity", "capacity");

            UnorderedList<Component> components = new UnorderedList<Component>(capacity);
            for(int i = 0; i < _components.Count; i++)
                components.Set(i, _components[i]);

            _components = components;
        }

        /// <summary>
        /// Adds a component
        /// </summary>
        /// <param name="component">Component to add</param>
        internal void Add(Component component)
        {
            _components.Set(component.Parent.Index, component);
        }

        /// <summary>
        /// Removes a component for the specified game object
        /// </summary>
        /// <param name="gameObject">Game object that owns the component to remove</param>
        /// <returns>Returns true if the component is removed otherwise false</returns>
        internal bool Remove(GameObject gameObject)
        {
            return (gameObject != null) && RemoveAt(gameObject.Index);
        }

        /// <summary>
        /// Removes a component
        /// </summary>
        /// <param name="component">Component to remove</param>
        /// <returns>Returns true if the component is removed otherwise false</returns>
        internal bool Remove(Component component)
        {
            return (component != null) && RemoveAt(component.Parent.Index);
        }

        /// <summary>
        /// Removes a component at the specified index
        /// </summary>
        /// <param name="index">Index of the component</param>
        /// <returns>Returns true if the component is removed otherwise false</returns>
        private bool RemoveAt(int index)
        {
            if (!_components.IsIndexWithinBounds(index))
                return false;

            _components.Set(index, null);

            return true;
        }

        /// <summary>
        /// Gets a component for the specified game object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public Component Get(GameObject gameObject)
        {
            return !_components.IsIndexWithinBounds(gameObject.Index) ? null : _components[gameObject.Index];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of component managed by this group
        /// </summary>
        public Type ComponentType
        {
            get { return _componentType; }
        }

        #endregion
    }
}
