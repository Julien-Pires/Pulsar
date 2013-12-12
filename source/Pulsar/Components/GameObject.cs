using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace Pulsar.Components
{
    /// <summary>
    /// Represents a game object
    /// </summary>
    public sealed class GameObject
    {
        #region Fields

        private readonly List<Component> _components = new List<Component>();

        #endregion

        #region Operators

        /// <summary>
        /// Gets the component at the specified index
        /// </summary>
        /// <param name="index">Index of the component</param>
        /// <returns>Returns a component instance</returns>
        internal Component this[int index]
        {
            get { return _components[index]; }
        }

        /// <summary>
        /// Gets the component with the specified type
        /// </summary>
        /// <param name="componentType">Type of the component</param>
        /// <returns>Returns a component instance</returns>
        public Component this[Type componentType]
        {
            get { return Get(componentType); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the component with the specified type
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <returns>Returns a component instance</returns>
        public T Get<T>() where T : Component
        {
            int index = GetIndexOfComponent(typeof(T));

            return (index == -1) ? null : _components[index] as T;
        }

        /// <summary>
        /// Gets the component with the specified type
        /// </summary>
        /// <param name="componentType">Type of the component</param>
        /// <returns>Returns a component instance</returns>
        public Component Get(Type componentType)
        {
            if(componentType == null)
                throw new ArgumentNullException("componentType");

            int index = GetIndexOfComponent(componentType);

            return (index == -1) ? null : _components[index];
        }

        /// <summary>
        /// Gets the index of a specific component
        /// </summary>
        /// <param name="componentType">Type of the component</param>
        /// <returns>Returns the zero-based index of the component if found otherwise -1</returns>
        private int GetIndexOfComponent(Type componentType)
        {
            Debug.Assert(componentType != null);

            int index = -1;
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].GetType() != componentType) continue;

                index = i;
                break;
            }

            return index;
        }

        /// <summary>
        /// Adds a component to the game object
        /// </summary>
        /// <param name="component">Component to add</param>
        public void Add(Component component)
        {
            if(component == null)
                throw new ArgumentNullException("component");

            Type componentType = component.GetType();
            if (GetIndexOfComponent(componentType) > -1)
                throw new ArgumentException(string.Format("GameObject already has a {0} component", componentType));

            _components.Add(component);
            component.Parent = this;

            if (Owner != null)
                Owner.AddComponent(component);
        }

        /// <summary>
        /// Removes a component with a specified type
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <returns>Returns true if the component is removed otherwise false</returns>
        public bool Remove<T>() where T : Component
        {
            return Remove(typeof (T));
        }

        /// <summary>
        /// Removes a component with a specified type
        /// </summary>
        /// <param name="componentType">Type of the component</param>
        /// <returns>Returns true if the component is removed otherwise false</returns>
        public bool Remove(Type componentType)
        {
            if(componentType == null)
                throw new ArgumentNullException("componentType");

            int index = GetIndexOfComponent(componentType);
            if (index == -1)
                return false;

            Component component = _components[index];
            RemoveComponent(index);

            if(Owner != null)
                Owner.RemoveComponent(component);

            return true;
        }

        /// <summary>
        /// Removes a component at a specific index
        /// </summary>
        /// <param name="index">Index of the component</param>
        private void RemoveComponent(int index)
        {
            Debug.Assert((index >= 0) || (index < _components.Count), "Invalid index");

            int lastItem = _components.Count - 1;
            if (lastItem != index)
            {
                Component temp = _components[lastItem];
                _components[lastItem] = _components[index];
                _components[index] = temp;
            }

            _components.RemoveAt(lastItem);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the index
        /// </summary>
        internal int Index { get; set; }

        /// <summary>
        /// Gets the owner of this game object
        /// </summary>
        public World Owner { get; internal set; }

        /// <summary>
        /// Gets the number of component
        /// </summary>
        public int Count
        {
            get { return _components.Count; }
        }

        #endregion
    }
}
