using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    public sealed class GameObjectManager
    {
        #region Fields

        public event EventHandler<GameObjectEventArgs> GameObjectAdded;
        public event EventHandler<GameObjectEventArgs> GameObjectRemoved;

        private Dictionary<ulong, GameObject> objectsMap = new Dictionary<ulong, GameObject>();

        #endregion

        #region Methods

        public void Add(GameObject obj)
        {
            if (this.objectsMap.ContainsKey(obj.ID))
            {
                throw new Exception(string.Format("Already contains a game object with ID {0}", obj.ID));
            }

            this.objectsMap.Add(obj.ID, obj);
            if (this.GameObjectAdded != null)
            {
                this.GameObjectAdded(this, new GameObjectEventArgs(obj));
            }
        }

        public bool Remove(ulong id)
        {
            GameObject obj;
            this.objectsMap.TryGetValue(id, out obj);
            if (obj == null)
            {
                return false;
            }

            bool result = this.objectsMap.Remove(id);
            if (this.GameObjectRemoved != null)
            {
                this.GameObjectRemoved(this, new GameObjectEventArgs(obj));
            }

            return result;
        }

        #endregion
    }
}
