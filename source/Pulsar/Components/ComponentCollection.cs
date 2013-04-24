using System;

using System.Collections;
using System.Collections.Generic;

namespace Pulsar.Components
{
    /// <summary>
    /// Represents a collection of component
    /// </summary>
    public class ComponentCollection : ICollection<Component>
    {
        #region Fields

        protected Dictionary<Type, Component> componentsMap = new Dictionary<Type, Component>();

        #endregion

        #region Event

        /// <summary>
        /// Occurs when a component is added
        /// </summary>
        public event EventHandler<ComponentEventArgs> ComponentAdded;

        /// <summary>
        /// Occurs when a component is removed
        /// </summary>
        public event EventHandler<ComponentEventArgs> ComponentRemoved;

        #endregion

        #region Methods

        /// <summary>
        /// Add a component
        /// </summary>
        /// <param name="compo">Component to add</param>
        public virtual void Add(Component compo)
        {
            this.Add(compo, false);
        }

        /// <summary>
        /// Add a component
        /// </summary>
        /// <param name="compo">Component to add</param>
        /// <param name="overwrite">Indicates to remove a component if this collection already contains one of the same type</param>
        public virtual void Add(Component compo, bool overwrite)
        {
            Type compoType = compo.GetType();
            if (this.componentsMap.ContainsKey(compoType))
            {
                if (overwrite)
                {
                    this.Remove(compoType);
                }
                else
                {
                    throw new Exception(
                        string.Format("Already contains a component of type {0} and overwrite is false", compoType)
                    );
                }
            }

            if (!this.componentsMap.ContainsKey(compoType))
            {
                this.componentsMap.Add(compoType, compo);
                if (this.ComponentAdded != null)
                {
                    this.ComponentAdded(this, new ComponentEventArgs(compo));
                }
            }
        }

        /// <summary>
        /// Remove immediately a component
        /// </summary>
        /// <param name="type">Type of the component to remove</param>
        public virtual void RemoveNow(Type type)
        {
            Component compo;
            this.componentsMap.TryGetValue(type, out compo);
            if (compo == null)
            {
                throw new Exception(string.Format("Failed to remove a component, {0} not found", compo));
            }

            bool result = this.componentsMap.Remove(type);
            if (result)
            {
                compo.Parent = null;
                if (this.ComponentRemoved != null)
                {
                    this.ComponentRemoved(this, new ComponentEventArgs(compo));
                }
            }
        }

        /// <summary>
        /// Remove immediately a component
        /// </summary>
        /// <typeparam name="T">Type of the component to remove</typeparam>
        public virtual void RemoveNow<T>() where T : Component
        {
            this.RemoveNow(typeof(T));
        }

        /// <summary>
        /// Remove immediately a component
        /// </summary>
        /// <param name="compo">Component to remove</param>
        public virtual void RemoveNow(Component compo)
        {
            if (compo.Parent != this)
            {
                throw new Exception("Failed to remove a component, parents game object don't match");
            }

            this.RemoveNow(compo.GetType());
        }

        /// <summary>
        /// Remove a component. The component isn't remove immediately but at the next frame
        /// </summary>
        /// <param name="compo">Component to remove</param>
        /// <returns>Return true if the component is removed otherwise false</returns>
        public virtual bool Remove(Component compo)
        {
            if (compo.Parent != this)
            {
                throw new Exception("Failed to remove a component, parents game object don't match");
            }

            return this.Remove(compo.GetType());
        }

        /// <summary>
        /// Remove a component. The component isn't remove immediately but at the next frame
        /// </summary>
        /// <typeparam name="T">Type of the component to remove</typeparam>
        /// <returns>Return true if the component is removed otherwise false</returns>
        public virtual bool Remove<T>() where T : Component
        {
            return this.Remove(typeof(T));
        }

        /// <summary>
        /// Remove a component. The component isn't remove immediately but at the next frame
        /// </summary>
        /// <param name="compoType">Type of the component to remove</param>
        /// <returns>Return true if the component is removed otherwise false</returns>
        public virtual bool Remove(Type compoType)
        {
            Component compo;
            this.componentsMap.TryGetValue(compoType, out compo);
            if (compo == null)
            {
                throw new Exception(string.Format("Failed to remove a component, {0} not found", compoType));
            }

            return compo.Parent.Owner.AddToPendingList(compo);
        }

        /// <summary>
        /// Clear this collection
        /// </summary>
        public virtual void Clear()
        {
            foreach (Component compo in this.componentsMap.Values)
            {
                this.RemoveNow(compo.GetType());
            }
        }

        /// <summary>
        /// Find a component
        /// </summary>
        /// <typeparam name="T">Type of the component to find</typeparam>
        /// <returns>Return an instance of component otherwise null</returns>
        public virtual T Find<T>() where T : Component
        {
            Component compo;
            this.componentsMap.TryGetValue(typeof(T), out compo);
            if (compo == null)
            {
                return null;
            }

            T castCompo = compo as T;
            if (castCompo == null)
            {
                throw new Exception(string.Format("Failed to find component {0}", typeof(T)));
            }

            return castCompo;
        }

        /// <summary>
        /// Check if this collection contains a specific component
        /// </summary>
        /// <typeparam name="T">Type of the component to find</typeparam>
        /// <returns>Return true if the collection contains the specified component otherwise false</returns>
        public virtual bool Contains<T>() where T : Component
        {
            return this.Contains(typeof(T));
        }

        /// <summary>
        /// Check if this collection contains a specific component
        /// </summary>
        /// <param name="t">Type of the component to find</param>
        /// <returns>Return true if the collection contains the specified component otherwise false</returns>
        public virtual bool Contains(Type t)
        {
            return this.componentsMap.ContainsKey(t);
        }

        /// <summary>
        /// Check if this collection contains a specific component
        /// </summary>
        /// <param name="compo">Component to find</param>
        /// <returns>Return true if the collection contains the specified component otherwise false</returns>
        public virtual bool Contains(Component compo)
        {
            return this.Contains(compo.GetType());
        }

        /// <summary>
        /// Get an enumerator for this collection
        /// </summary>
        /// <returns>Return an enumerator instance</returns>
        public virtual IEnumerator<Component> GetEnumerator()
        {
            return this.componentsMap.Values.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator for this collection
        /// </summary>
        /// <returns>Return an enumerator instance</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.componentsMap.Values.GetEnumerator();
        }

        /// <summary>
        /// Copy this collection into an array
        /// </summary>
        /// <param name="compoArr">Array to copy component in</param>
        /// <param name="startIndex">Starting index for the copy</param>
        public virtual void CopyTo(Component[] compoArr, int startIndex)
        {
            this.componentsMap.Values.CopyTo(compoArr, startIndex);
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Get the number of component
        /// </summary>
        public int Count
        {
            get { return this.componentsMap.Count; }
        }

        /// <summary>
        /// Get a value indicating if this collection is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}
