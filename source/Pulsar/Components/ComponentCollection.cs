using System;

using System.Collections;
using System.Collections.Generic;

namespace Pulsar.Components
{
    public class ComponentCollection : ICollection<Component>
    {
        #region Fields

        protected Dictionary<Type, Component> componentsMap = new Dictionary<Type, Component>();

        #endregion

        #region Event

        public event EventHandler<ComponentEventArgs> ComponentAdded;
        public event EventHandler<ComponentEventArgs> ComponentRemoved;

        #endregion

        #region Methods

        public virtual void Add(Component compo)
        {
            this.Add(compo, false);
        }

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

        public virtual void RemoveNow<T>() where T : Component
        {
            this.RemoveNow(typeof(T));
        }

        public virtual void RemoveNow(Component compo)
        {
            if (compo.Parent != this)
            {
                throw new Exception("Failed to remove a component, parents game object don't match");
            }

            this.RemoveNow(compo.GetType());
        }

        public virtual bool Remove(Component compo)
        {
            if (compo.Parent != this)
            {
                throw new Exception("Failed to remove a component, parents game object don't match");
            }

            return this.Remove(compo.GetType());
        }

        public virtual bool Remove<T>() where T : Component
        {
            return this.Remove(typeof(T));
        }

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

        public virtual void Clear()
        {
            foreach (Component compo in this.componentsMap.Values)
            {
                this.RemoveNow(compo.GetType());
            }
        }

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

        public virtual bool Contains<T>() where T : Component
        {
            return this.Contains(typeof(T));
        }

        public virtual bool Contains(Type t)
        {
            return this.componentsMap.ContainsKey(t);
        }

        public virtual bool Contains(Component compo)
        {
            return this.Contains(compo.GetType());
        }

        public virtual IEnumerator<Component> GetEnumerator()
        {
            return this.componentsMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.componentsMap.Values.GetEnumerator();
        }

        public virtual void CopyTo(Component[] compoArr, int startIndex)
        {
            this.componentsMap.Values.CopyTo(compoArr, startIndex);
        }

        #endregion

        #region Properties
        
        public int Count
        {
            get { return this.componentsMap.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}
