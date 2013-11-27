using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    /// <summary>
    /// Manager of all game objects.
    /// GameObjectManager is responsible for dispatching component event for game objects contained
    /// in this manager.
    /// </summary>
    public sealed class GameObjectManager
    {
        #region Fields

        private Dictionary<ulong, GameObject> objectsMap = new Dictionary<ulong, GameObject>();
        private List<Component> componentsList = new List<Component>();
        private List<Component> pendingDeletion = new List<Component>();

        #endregion

        #region Event

        /// <summary>
        /// Occurs when a game object is added
        /// </summary>
        public event EventHandler<GameObjectEventArgs> GameObjectAdded;

        /// <summary>
        /// Occurs when a game object is removed
        /// </summary>
        public event EventHandler<GameObjectEventArgs> GameObjectRemoved;

        /// <summary>
        /// Occurs when a component is added to a game object in this manager
        /// </summary>
        public event EventHandler<ComponentEventArgs> ComponentAdded;

        /// <summary>
        /// Occurs when a component is removed from a game object in this manager
        /// </summary>
        public event EventHandler<ComponentEventArgs> ComponentRemoved;

        #endregion

        #region Methods

        /// <summary>
        /// Update the game object manager
        /// </summary>
        public void Update()
        {
            if (this.pendingDeletion.Count > 0)
            {
                for (int i = 0; i < this.pendingDeletion.Count; i++)
                {
                    ComponentEventArgs e = new ComponentEventArgs(this.pendingDeletion[i]);
                    this.OnComponentRemoved(this, e);
                }

                this.pendingDeletion.Clear();
            }
        }

        /// <summary>
        /// Add a game object
        /// </summary>
        /// <param name="obj">Game object to add</param>
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

        /// <summary>
        /// Remove game object. Components of the game object are removed immediately
        /// </summary>
        /// <param name="id">Id of the game object to remove</param>
        /// <returns>Return true if the game object is removed successfully otherwise false</returns>
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

        /// <summary>
        /// Remove a game object. The components aren't removed immediately but at the next frame
        /// </summary>
        /// <param name="id">Id of the game object to remove</param>
        /// <returns>Return true if the game object is removed successfully otherwise false</returns>
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

        /// <summary>
        /// Remove a game object
        /// </summary>
        /// <param name="go">Game object to remove</param>
        /// <returns>Return true if the game object is removed successfully otherwise false</returns>
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

        /// <summary>
        /// Add a component
        /// </summary>
        /// <param name="compo">Component to add</param>
        internal void AddComponent(Component compo)
        {
            if (this.componentsList.Contains(compo))
            {
                throw new Exception(string.Format("Component {0} already added", compo));
            }
            this.componentsList.Add(compo);
        }

        /// <summary>
        /// Remove a component
        /// </summary>
        /// <param name="compo">Component to remove</param>
        /// <returns>Returns true if the component is removed successfully otherwise false</returns>
        internal bool RemoveComponent(Component compo)
        {
            if (this.componentsList.Contains(compo))
            {
                throw new Exception(string.Format("No component {0} exists in this game object manager", compo));
            }

            return this.componentsList.Remove(compo);
        }

        /// <summary>
        /// Add a component to the waiting list for deletion
        /// </summary>
        /// <param name="compo">Component to add</param>
        /// <returns>Returns true if the component is added successfully otherwise false</returns>
        internal bool AddToPendingList(Component compo)
        {
            if (this.pendingDeletion.Contains(compo))
            {
                return false;
            }
            this.pendingDeletion.Add(compo);

            return true;
        }

        /// <summary>
        /// Called when a component is added to a game object managed by this instance
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Argument of the event</param>
        private void OnComponentAdded(object sender, ComponentEventArgs e)
        {
            this.AddComponent(e.Component);
            if (this.ComponentAdded != null)
            {
                this.ComponentAdded(this, e);
            }
        }

        /// <summary>
        /// Called when a component is removed from a game object managed by this instance
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Argument of the event</param>
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
