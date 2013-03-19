using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    public class ComponentCollection
    {
        #region Fields

        protected Dictionary<Type, Component> componentsMap = new Dictionary<Type, Component>();

        #endregion

        #region Event

        public event EventHandler<ComponentEventArgs> ComponentAdded;
        public event EventHandler<ComponentEventArgs> ComponentRemoved;

        #endregion

        #region Methods

        public virtual void AddComponent(Component compo, bool overwrite)
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

        public virtual bool Remove<T>() where T : Component
        {
            return this.Remove(typeof(T));
        }

        public virtual bool Remove(Type compoType)
        {
            Component compo;
            this.componentsMap.TryGetValue(compoType, out compo);
            if (compo != null)
            {
                compo.Parent = null;
                bool result = this.componentsMap.Remove(compoType);

                if (result)
                {
                    if (this.ComponentRemoved != null)
                    {
                        this.ComponentRemoved(this, new ComponentEventArgs(compo));
                    }
                }

                return result;
            }

            return false;
        }

        public T Find<T>() where T : Component
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

        public bool Contains<T>() where T : Component
        {
            return this.Contains(typeof(T));
        }

        public bool Contains(Type t)
        {
            return this.componentsMap.ContainsKey(t);
        }

        #endregion
    }
}
