using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Core;

namespace Pulsar.Components
{
    /// <summary>
    /// Defines a game world
    /// </summary>
    public sealed class World
    {
        #region Fields

        private readonly IndexPool _indexPool = new IndexPool();
        private readonly GameObjectManager _gameObjectManager = new GameObjectManager();
        private readonly ComponentManager _componentManager = new ComponentManager();
        private readonly SystemManager _systemManager = new SystemManager();
        private readonly List<GameObject> _addedGameObjects = new List<GameObject>();
        private readonly List<GameObject> _removedGameObjects = new List<GameObject>();

        #endregion

        #region Methods
        /// <summary>
        /// Updates the world
        /// </summary>
        /// <param name="time">Elapsed time</param>
        public void Update(GameTime time)
        {
            _systemManager.Update(time);
        }

        /// <summary>
        /// Processes changes in the world
        /// </summary>
        public void Process()
        {
            DispatchAdded();
            DispatchRemoved();
        }

        /// <summary>
        /// Dispatches added game object
        /// </summary>
        private void DispatchAdded()
        {
            for (int i = 0; i < _addedGameObjects.Count; i++)
            {
                GameObject gameObject = _addedGameObjects[i];
                if (gameObject.Owner != null)
                    throw new Exception(string.Format("Failed to add, game object {0} already attached to another world",
                        gameObject.Id));

                gameObject.Owner = this;
                gameObject.Index = _indexPool.Get();
                _gameObjectManager.Add(gameObject);

                for (int j = 0; j < gameObject.Count; j++)
                {
                    Component component = gameObject[j];
                    _componentManager.Add(component);
                    _systemManager.ComponentAdded(component);
                }
            }

            _addedGameObjects.Clear();
        }

        /// <summary>
        /// Dispatches removed game object
        /// </summary>
        private void DispatchRemoved()
        {
            for (int i = 0; i < _removedGameObjects.Count; i++)
            {
                GameObject gameObject = _removedGameObjects[i];
                gameObject.Owner = null;

                _gameObjectManager.Remove(gameObject.Id);
                for (int j = 0; j < gameObject.Count; j++)
                {
                    Component component = gameObject[j];
                    _systemManager.ComponentRemoved(component);
                    _componentManager.Remove(component);
                }

                _indexPool.Release(gameObject.Index);
                gameObject.Index = -1;
            }

            _removedGameObjects.Clear();
        }

        /// <summary>
        /// Adds a game object in the world
        /// </summary>
        /// <param name="gameObject">Game object</param>
        public void Add(GameObject gameObject)
        {
            if(gameObject == null)
                throw new ArgumentNullException("gameObject");

            _addedGameObjects.Add(gameObject);
        }

        /// <summary>
        /// Removes a game object from the world
        /// </summary>
        /// <param name="gameObject">Game object</param>
        /// <returns>Returns true if removed otherwise false</returns>
        public bool Remove(GameObject gameObject)
        {
            if(gameObject == null)
                throw new ArgumentNullException("gameObject");

            if ((gameObject.Owner == null) || (gameObject.Owner != this))
                return false;

            _removedGameObjects.Add(gameObject);

            return true;
        }

        /// <summary>
        /// Adds a component
        /// </summary>
        /// <param name="component">Component</param>
        internal void AddComponent(Component component)
        {
            _componentManager.Add(component);
            _systemManager.ComponentAdded(component);
        }

        /// <summary>
        /// Removes a component
        /// </summary>
        /// <param name="component">Component</param>
        internal void RemoveComponent(Component component)
        {
            _systemManager.ComponentRemoved(component);
            _componentManager.Remove(component);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the game object manager
        /// </summary>
        public GameObjectManager GameObjectManager
        {
            get { return _gameObjectManager; }
        }

        /// <summary>
        /// Gets the component manager
        /// </summary>
        public ComponentManager ComponentManager
        {
            get { return _componentManager; }
        }

        /// <summary>
        /// Gets the system manager
        /// </summary>
        public SystemManager SystemManager
        {
            get { return _systemManager; }
        }

        #endregion
    }
}
