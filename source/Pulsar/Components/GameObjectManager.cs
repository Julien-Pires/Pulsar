using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pulsar.Components
{
    /// <summary>
    /// Manages all game objects
    /// </summary>
    public sealed class GameObjectManager
    {
        #region Fields

        private Dictionary<int, GameObject> _gameObjectsMap = new Dictionary<int, GameObject>();

        #endregion

        #region Event

        /// <summary>
        /// Occurs when a game object is added
        /// </summary>
        public event GameObjectHandler GameObjectAdded;

        /// <summary>
        /// Occurs when a game object is removed
        /// </summary>
        public event GameObjectHandler GameObjectRemoved;

        #endregion

        #region Operators

        /// <summary>
        /// Gets a game object with a specified id
        /// </summary>
        /// <param name="id">ID of the game object</param>
        /// <returns>Returns a game object instance if found otherwise null</returns>
        public GameObject this[int id]
        {
            get
            {
                GameObject gameObject;
                _gameObjectsMap.TryGetValue(id, out gameObject);

                return gameObject;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a game object
        /// </summary>
        /// <param name="gameObject">Game object</param>
        internal void Add(GameObject gameObject)
        {
            Debug.Assert(gameObject != null, "Game object cannot be null");

            if(_gameObjectsMap.ContainsKey(gameObject.Id))
                throw new ArgumentException(string.Format("Failed to add, a game object with ID {0} already exists", 
                    gameObject.Id));

            _gameObjectsMap.Add(gameObject.Id, gameObject);
            GameObjectHandler gameObjectAdded = GameObjectAdded;
            if (gameObjectAdded != null)
                gameObjectAdded(this, gameObject);
        }

        /// <summary>
        /// Removes a game object with a specified ID
        /// </summary>
        /// <param name="id">Id of the game object</param>
        /// <returns>Returns true if removed otherwise false</returns>
        internal bool Remove(int id)
        {
            GameObject gameObject;
            _gameObjectsMap.TryGetValue(id, out gameObject);
            if (!_gameObjectsMap.Remove(id)) 
                return false;

            GameObjectHandler gameObjectRemoved = GameObjectRemoved;
            if (gameObjectRemoved != null)
                gameObjectRemoved(this, gameObject);

            return true;
        }

        /// <summary>
        /// Gets a game object with a specified ID
        /// </summary>
        /// <param name="id">Id of the game object</param>
        /// <returns>Returns a game object if found otherwise null</returns>
        public GameObject Get(int id)
        {
            GameObject gameObject;
            _gameObjectsMap.TryGetValue(id, out gameObject);

            return gameObject;
        }

        /// <summary>
        /// Estimates the number of game object
        /// </summary>
        /// <param name="count">Number of game object</param>
        public void Estimate(int count)
        {
            if(count < _gameObjectsMap.Count)
                throw new ArgumentException("Insufficient capacity", "count");

            Dictionary<int, GameObject> gameObjectsMap = new Dictionary<int, GameObject>(count);
            foreach (KeyValuePair<int, GameObject>  gameObject in _gameObjectsMap)
                gameObjectsMap.Add(gameObject.Key, gameObject.Value);

            _gameObjectsMap = gameObjectsMap;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of game object
        /// </summary>
        public int Count
        {
            get { return _gameObjectsMap.Count; }
        }

        #endregion
    }
}
