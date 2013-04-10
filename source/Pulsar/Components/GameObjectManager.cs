using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    public sealed class GameObjectManager
    {
        #region Fields

        private Dictionary<ulong, GameObject> objectsMap = new Dictionary<ulong, GameObject>();
        private List<Component> componentsList = new List<Component>();
        private List<Component> pendingDeletion = new List<Component>();

        #endregion

        #region Event

        public event EventHandler<GameObjectEventArgs> GameObjectAdded;
        public event EventHandler<GameObjectEventArgs> GameObjectRemoved;
        public event EventHandler<ComponentEventArgs> ComponentAdded;
        public event EventHandler<ComponentEventArgs> ComponentRemoved;

        #endregion

        #region Methods

        public void Update()
        {
            if (this.pendingDeletion.Count > 0)
            {
                for (int i = 0; i < this.pendingDeletion.Count; i++)
                {
                    Component compo = this.pendingDeletion[i];
                    if (compo.Parent != null)
                    {
                        compo.Parent.RemoveNow(compo.GetType());
                    }
                }

                this.pendingDeletion.Clear();
            }
        }

        public void Add(GameObject obj)
        {
            if (this.objectsMap.ContainsKey(obj.ID))
            {
                throw new Exception(string.Format("Already contains a game object with ID {0}", obj.ID));
            }

            this.objectsMap.Add(obj.ID, obj);
            obj.Owner = this;
            if (obj.Count > 0)
            {
                foreach (Component compo in obj)
                {
                    ComponentEventArgs e = new ComponentEventArgs(compo);
                    this.OnComponentAdded(this, e);
                }
            }

            obj.ComponentAdded += this.OnComponentAdded;
            obj.ComponentRemoved += this.OnComponentRemoved;
            if (this.GameObjectAdded != null)
            {
                this.GameObjectAdded(this, new GameObjectEventArgs(obj));
            }
        }

        public bool RemoveNow(ulong id)
        {
            GameObject obj;
            this.objectsMap.TryGetValue(id, out obj);
            if (obj == null)
            {
                return false;
            }
            obj.Clear();

            return this.RemoveGameObject(obj);
        }

        public bool Remove(ulong id)
        {
            GameObject obj;
            this.objectsMap.TryGetValue(id, out obj);
            if (obj == null)
            {
                return false;
            }

            foreach (Component compo in obj)
            {
                this.AddToPendingList(compo);
            }

            return this.RemoveGameObject(obj);
        }

        private bool RemoveGameObject(GameObject go)
        {
            bool result = this.objectsMap.Remove(go.ID);

            if (result)
            {
                go.Owner = null;
                go.ComponentAdded -= this.OnComponentAdded;
                go.ComponentRemoved -= this.OnComponentRemoved;

                if (this.GameObjectRemoved != null)
                {
                    this.GameObjectRemoved(this, new GameObjectEventArgs(go));
                }
            }

            return result;
        }

        internal void AddComponent(Component compo)
        {
            if (this.componentsList.Contains(compo))
            {
                throw new Exception(string.Format("Component {0} already added", compo));
            }
            this.componentsList.Add(compo);
        }

        internal bool RemoveComponent(Component compo)
        {
            if (this.componentsList.Contains(compo))
            {
                throw new Exception(string.Format("No component {0} exists in this game object manager", compo));
            }

            return this.componentsList.Remove(compo);
        }

        internal bool AddToPendingList(Component compo)
        {
            if (this.pendingDeletion.Contains(compo))
            {
                return false;
            }
            this.pendingDeletion.Add(compo);

            return true;
        }

        private void OnComponentAdded(object sender, ComponentEventArgs e)
        {
            this.AddComponent(e.Component);
            if (this.ComponentAdded != null)
            {
                this.ComponentAdded(this, e);
            }
        }

        private void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            this.RemoveComponent(e.Component);
            if (this.ComponentRemoved != null)
            {
                this.ComponentRemoved(this, e);
            }
        }

        #endregion
    }
}
